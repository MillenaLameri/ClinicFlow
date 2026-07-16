import { apiClient } from "@/api/apiClient";

import type {
  CreateSpecialtyRequest,
  Specialty,
  UpdateSpecialtyRequest,
} from "../types/specialty.types";

export const specialtyService = {
  async getAll(includeInactive = false): Promise<Specialty[]> {
    const response = await apiClient.get<Specialty[]>("/api/specialties", {
      params: {
        includeInactive,
      },
    });

    return response.data;
  },

  async create(request: CreateSpecialtyRequest): Promise<Specialty> {
    const response = await apiClient.post<Specialty>(
      "/api/specialties",
      request,
    );

    return response.data;
  },

  async update(
    id: string,
    request: UpdateSpecialtyRequest,
  ): Promise<Specialty> {
    const response = await apiClient.put<Specialty>(
      `/api/specialties/${id}`,
      request,
    );

    return response.data;
  },

  async deactivate(id: string): Promise<void> {
    await apiClient.delete(`/api/specialties/${id}`);
  },
};
