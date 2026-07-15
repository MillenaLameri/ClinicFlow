import { apiClient } from "@/api/apiClient";
import type { AdminDashboard } from "../types/adminDashboard.types";

export const adminDashboardService = {
  async getDashboard(): Promise<AdminDashboard> {
    const response = await apiClient.get<AdminDashboard>(
      "/api/dashboard/admin",
    );

    return response.data;
  },
};
