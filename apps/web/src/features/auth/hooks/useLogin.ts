import { useMutation } from "@tanstack/react-query";

import { authService } from "../services/authService";

import { useAuth } from "./useAuth";

export function useLogin() {
  const { signIn } = useAuth();

  return useMutation({
    mutationFn: authService.login,

    onSuccess: (authentication) => {
      signIn(authentication);
    },
  });
}
