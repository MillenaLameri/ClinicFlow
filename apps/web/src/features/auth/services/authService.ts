import { apiClient } from "@/api/apiClient";

import type { AuthenticationResponse, LoginRequest } from "../types/auth.types";

export const authService = {
  async login(request: LoginRequest): Promise<AuthenticationResponse> {
    const response = await apiClient.post<AuthenticationResponse>(
      "/api/auth/login",
      request,
    );

    return response.data;
  },

  async me(): Promise<AuthenticationResponse["user"]> {
    const response =
      await apiClient.get<AuthenticationResponse["user"]>("/api/auth/me");

    return response.data;
  },
};
