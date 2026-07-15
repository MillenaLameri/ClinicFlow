import {
  createContext,
  useCallback,
  useMemo,
  useState,
} from 'react';

import type {
  ReactNode,
} from 'react';

import {
  authStorage,
} from '../services/authStorage';

import type {
  AuthenticatedUser,
  AuthenticationResponse,
  AuthSession,
} from "../types/auth.types";

type AuthContextValue = {
  user: AuthenticatedUser | null;
  isAuthenticated: boolean;
  signIn: (authentication: AuthenticationResponse) => void;
  signOut: () => void;
};

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
);

type AuthProviderProps = {
  children: ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {
  const storedSession = authStorage.get();

  const [session, setSession] = useState<AuthSession | null>(storedSession);

  const signIn = useCallback((authentication: AuthenticationResponse) => {
    const newSession: AuthSession = {
      accessToken: authentication.accessToken,

      refreshToken: authentication.refreshToken,

      user: authentication.user,
    };

    authStorage.save(newSession);

    setSession(newSession);
  }, []);

  const signOut = useCallback(() => {
    authStorage.clear();

    setSession(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user: session?.user ?? null,

      isAuthenticated: Boolean(session?.accessToken),

      signIn,
      signOut,
    }),
    [session, signIn, signOut],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
