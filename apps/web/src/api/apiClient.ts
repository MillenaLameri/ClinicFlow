import axios from "axios";

import { authStorage } from "@/features/auth/services/authStorage";

const apiUrl = import.meta.env.VITE_API_URL;

if (!apiUrl) {
  throw new Error("A variável VITE_API_URL não foi configurada.");
}

export const apiClient = axios.create({
  baseURL: apiUrl,

  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const session = authStorage.get();

  if (session?.accessToken) {
    config.headers.Authorization = `Bearer ${session.accessToken}`;
  }

  return config;
});
