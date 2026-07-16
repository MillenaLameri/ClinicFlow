import { useMutation, useQueryClient } from "@tanstack/react-query";
import { specialtyService } from "../services/specialtyService";
import type { UpdateSpecialtyRequest } from "../types/specialty.types";

type UpdateSpecialtyMutation = {
  id: string;
  data: UpdateSpecialtyRequest;
};

export function useUpdateSpecialty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: UpdateSpecialtyMutation) =>
      specialtyService.update(id, data),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["specialties"],
      });
    },
  });
}
