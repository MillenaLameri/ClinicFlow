import { useMutation, useQueryClient } from "@tanstack/react-query";
import { specialtyService } from "../services/specialtyService";

export function useDeleteSpecialty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: specialtyService.deactivate,

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["specialties"],
      });
    },
  });
}
