import { useQuery } from "@tanstack/react-query";
import { doctorService } from "../services/doctorService";
import type { GetDoctorsParams } from "../types/doctor.types";

export function useDoctors(params: GetDoctorsParams) {
  return useQuery({
    queryKey: ["doctors", params],

    queryFn: () => doctorService.getAll(params),
  });
}
