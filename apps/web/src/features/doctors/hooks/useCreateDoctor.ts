import { useMutation, useQueryClient } from "@tanstack/react-query";
import { doctorService } from "../services/doctorService";

export function useCreateDoctor() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: doctorService.create,

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["doctors"],
        }),

        queryClient.invalidateQueries({
          queryKey: ["admin-dashboard"],
        }),
      ]);
    },
  });
}
