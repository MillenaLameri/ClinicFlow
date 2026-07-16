import { useMutation, useQueryClient } from "@tanstack/react-query";
import { doctorService } from "../services/doctorService";
import type { UpdateDoctorRequest } from "../types/doctor.types";

type UpdateDoctorMutation = {
  id: string;
  data: UpdateDoctorRequest;
};

export function useUpdateDoctor() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: UpdateDoctorMutation) =>
      doctorService.update(id, data),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["doctors"],
      });
    },
  });
}
