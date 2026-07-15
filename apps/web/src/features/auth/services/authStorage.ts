import type {
  AuthSession,
} from '../types/auth.types';

const STORAGE_KEY =
  '@clinicflow:auth';

export const authStorage = {
  save(
    session: AuthSession,
  ): void {
    localStorage.setItem(
      STORAGE_KEY,
      JSON.stringify(session),
    );
  },

  get(): AuthSession | null {
    const storedSession =
      localStorage.getItem(
        STORAGE_KEY,
      );

    if (!storedSession) {
      return null;
    }

    try {
      return JSON.parse(
        storedSession,
      ) as AuthSession;
    } catch {
      localStorage.removeItem(
        STORAGE_KEY,
      );

      return null;
    }
  },

  clear(): void {
    localStorage.removeItem(
      STORAGE_KEY,
    );
  },
};