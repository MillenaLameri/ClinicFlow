import { z } from "zod";

export const doctorFormSchema = z.object({
  fullName: z
    .string()
    .trim()
    .min(3, "Informe o nome completo.")
    .max(150, "O nome deve ter no máximo 150 caracteres."),

  crmNumber: z
    .string()
    .trim()
    .min(1, "Informe o número do CRM.")
    .max(20, "O CRM deve ter no máximo 20 caracteres."),

  crmState: z.string().trim().length(2, "Informe a UF com 2 caracteres."),

  email: z
    .string()
    .trim()
    .email("Informe um e-mail válido.")
    .max(150, "O e-mail deve ter no máximo 150 caracteres."),

  phone: z
    .string()
    .trim()
    .max(20, "O telefone deve ter no máximo 20 caracteres."),

  specialtyId: z.string().min(1, "Selecione uma especialidade."),
});

export type DoctorFormValues = z.infer<typeof doctorFormSchema>;
