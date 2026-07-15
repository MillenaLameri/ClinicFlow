import { apiClient } from "@/api/apiClient";
import type { PagedResponse } from "@/types/pagination.types";
import type { Doctor, GetDoctorsParams } from "../types/doctor.types";

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
};
