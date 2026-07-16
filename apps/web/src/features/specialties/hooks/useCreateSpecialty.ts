import {
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";

import {
  specialtyService,
} from "../services/specialtyService";

export function useCreateSpecialty() {
  const queryClient =
    useQueryClient();

  return useMutation({
    mutationFn:
      specialtyService.create,

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: [
          "specialties",
        ],
      });
    },
  });
}