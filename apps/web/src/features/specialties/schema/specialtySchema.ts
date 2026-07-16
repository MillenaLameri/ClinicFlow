import { z } from "zod";

export const specialtyFormSchema = z.object({
  name: z
    .string()
    .trim()
    .min(2, "Informe o nome da especialidade.")
    .max(100, "O nome deve ter no máximo 100 caracteres."),
});

export type SpecialtyFormValues = z.infer<typeof specialtyFormSchema>;
