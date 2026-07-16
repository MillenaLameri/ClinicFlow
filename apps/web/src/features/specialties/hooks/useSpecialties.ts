import { useQuery } from "@tanstack/react-query";
import { specialtyService } from "../services/specialtyService";

export function useSpecialties(includeInactive = false) {
  return useQuery({
    queryKey: [
      "specialties",
      {
        includeInactive,
      },
    ],

    queryFn: () => specialtyService.getAll(includeInactive),

    staleTime: 5 * 60 * 1000,
  });
}
