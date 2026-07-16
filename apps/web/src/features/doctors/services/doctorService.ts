import { apiClient } from "@/api/apiClient";
import type { PagedResponse } from "@/types/pagination.types";

import type {
  CreateDoctorRequest,
  Doctor,
  GetDoctorsParams,
  UpdateDoctorRequest,
} from "../types/doctor.types";

export const doctorService = {
  async getAll(params: GetDoctorsParams): Promise<PagedResponse<Doctor>> {
    const response = await apiClient.get<PagedResponse<Doctor>>(
      "/api/doctors",
      {
        params: {
          page: params.page,
          pageSize: params.pageSize,
          search: params.search || undefined,
          specialtyId: params.specialtyId || undefined,
          includeInactive: params.includeInactive,
        },
      },
    );

    return response.data;
  },

  async create(request: CreateDoctorRequest): Promise<Doctor> {
    const response = await apiClient.post<Doctor>("/api/doctors", request);

    return response.data;
  },

  async update(id: string, request: UpdateDoctorRequest): Promise<Doctor> {
    const response = await apiClient.put<Doctor>(`/api/doctors/${id}`, request);

    return response.data;
  },

  async deactivate(id: string): Promise<void> {
    await apiClient.delete(`/api/doctors/${id}`);
  },
};
