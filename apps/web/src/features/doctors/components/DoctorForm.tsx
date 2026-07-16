import type { ReactNode } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { doctorFormSchema } from "../schemas/doctorSchema";
import type { DoctorFormValues } from "../schemas/doctorSchema";
import type { Doctor } from "../types/doctor.types";
import type { Specialty } from "@/features/specialties/types/specialty.types";

type DoctorFormMode = "create" | "edit";

type DoctorFormProps = {
  mode: DoctorFormMode;
  doctor: Doctor | null;
  specialties: Specialty[];
  isSubmitting: boolean;
  onCancel: () => void;
  onSubmit: (values: DoctorFormValues) => Promise<void>;
};

export function DoctorForm({
  mode,
  doctor,
  specialties,
  isSubmitting,
  onCancel,
  onSubmit,
}: DoctorFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<DoctorFormValues>({
    resolver: zodResolver(doctorFormSchema),

    defaultValues: {
      fullName: "",
      crmNumber: "",
      crmState: "",
      email: "",
      phone: "",
      specialtyId: "",
    },
  });

  useEffect(() => {
    if (mode === "edit" && doctor) {
      reset({
        fullName: doctor.fullName,

        crmNumber: doctor.crmNumber,

        crmState: doctor.crmState,

        email: doctor.email,

        phone: doctor.phone ?? "",

        specialtyId: doctor.specialty.id,
      });

      return;
    }

    reset({
      fullName: "",
      crmNumber: "",
      crmState: "",
      email: "",
      phone: "",
      specialtyId: "",
    });
  }, [doctor, mode, reset]);

  const inputClassName = `
    h-11
    w-full
    rounded-xl
    border
    border-slate-200
    bg-white
    px-3
    text-sm
    text-[#18234A]
    outline-none
    transition

    placeholder:text-slate-400

    focus:border-[#2448A5]
    focus:ring-2
    focus:ring-[#2448A5]/10

    disabled:cursor-not-allowed
    disabled:bg-slate-100
    disabled:text-slate-500
  `;

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      <FormField label="Nome completo" error={errors.fullName?.message}>
        <input
          {...register("fullName")}
          placeholder="Ex.: Dr. João Silva"
          className={inputClassName}
        />
      </FormField>

      <div
        className="
          grid
          grid-cols-1
          gap-4

          sm:grid-cols-[1fr_120px]
        "
      >
        <FormField label="CRM" error={errors.crmNumber?.message}>
          <input
            {...register("crmNumber")}
            disabled={mode === "edit"}
            placeholder="123456"
            className={inputClassName}
          />
        </FormField>

        <FormField label="UF" error={errors.crmState?.message}>
          <input
            {...register("crmState")}
            disabled={mode === "edit"}
            maxLength={2}
            placeholder="ES"
            className={`
              ${inputClassName}
              uppercase
            `}
          />
        </FormField>
      </div>

      {mode === "edit" && (
        <p
          className="
            -mt-2
            text-xs
            text-slate-400
          "
        >
          O CRM não pode ser alterado após o cadastro.
        </p>
      )}

      <FormField label="E-mail" error={errors.email?.message}>
        <input
          {...register("email")}
          type="email"
          placeholder="medico@clinica.com"
          className={inputClassName}
        />
      </FormField>

      <FormField label="Telefone" error={errors.phone?.message}>
        <input
          {...register("phone")}
          placeholder="(27) 99999-9999"
          className={inputClassName}
        />
      </FormField>

      <FormField label="Especialidade" error={errors.specialtyId?.message}>
        <select {...register("specialtyId")} className={inputClassName}>
          <option value="">Selecione uma especialidade</option>

          {specialties.map((specialty) => (
            <option key={specialty.id} value={specialty.id}>
              {specialty.name}
            </option>
          ))}
        </select>
      </FormField>

      <div
        className="
          flex
          flex-col-reverse
          gap-3
          border-t
          border-slate-100
          pt-5

          sm:flex-row
          sm:justify-end
        "
      >
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmitting}
          className="
            h-11
            rounded-xl
            border
            border-slate-200
            bg-white
            px-5
            text-sm
            font-semibold
            text-slate-600
            transition

            hover:bg-slate-50

            disabled:opacity-50
          "
        >
          Cancelar
        </button>

        <button
          type="submit"
          disabled={isSubmitting}
          className="
            h-11
            rounded-xl
            bg-[#2448A5]
            px-5
            text-sm
            font-semibold
            text-white
            transition

            hover:bg-[#172D6B]

            disabled:cursor-not-allowed
            disabled:opacity-60
          "
        >
          {isSubmitting
            ? "Salvando..."
            : mode === "create"
              ? "Cadastrar médico"
              : "Salvar alterações"}
        </button>
      </div>
    </form>
  );
}

type FormFieldProps = {
  label: string;
  error?: string;
  children: ReactNode;
};

function FormField({ label, error, children }: FormFieldProps) {
  return (
    <div>
      <label
        className="
          mb-2
          block
          text-sm
          font-semibold
          text-[#18234A]
        "
      >
        {label}
      </label>

      {children}

      {error && (
        <p
          className="
            mt-1.5
            text-xs
            font-medium
            text-red-600
          "
        >
          {error}
        </p>
      )}
    </div>
  );
}
