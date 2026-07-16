import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";
import { X } from "lucide-react";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useCreateSpecialty } from "../hooks/useCreateSpecialty";
import { useUpdateSpecialty } from "../hooks/useUpdateSpecialty";
import type { Specialty } from "../types/specialty.types";
import { specialtyFormSchema, type SpecialtyFormValues } from "../schema/specialtySchema";

type SpecialtyFormDialogProps = {
  isOpen: boolean;

  specialty: Specialty | null;

  onClose: () => void;
};

type ApiProblem = {
  title?: string;
  detail?: string;

  errors?: Record<string, string[]>;
};

export function SpecialtyFormDialog({
  isOpen,
  specialty,
  onClose,
}: SpecialtyFormDialogProps) {
  const [serverError, setServerError] = useState<string | null>(null);

  const createSpecialty = useCreateSpecialty();

  const updateSpecialty = useUpdateSpecialty();

  const isEditing = specialty !== null;

  const isSubmitting = createSpecialty.isPending || updateSpecialty.isPending;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<SpecialtyFormValues>({
    resolver: zodResolver(specialtyFormSchema),

    defaultValues: {
      name: "",
    },
  });

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    setServerError(null);

    reset({
      name: specialty?.name ?? "",
    });
  }, [isOpen, specialty, reset]);

  if (!isOpen) {
    return null;
  }

  async function onSubmit(values: SpecialtyFormValues) {
    setServerError(null);

    try {
      const data = {
        name: values.name.trim(),
      };

      if (specialty) {
        await updateSpecialty.mutateAsync({
          id: specialty.id,

          data,
        });
      } else {
        await createSpecialty.mutateAsync(data);
      }

      onClose();
    } catch (error) {
      setServerError(getApiErrorMessage(error));
    }
  }

  return (
    <div
      className="
        fixed
        inset-0
        z-[100]
        flex
        items-center
        justify-center
        bg-slate-950/40
        px-4
        backdrop-blur-[2px]
      "
    >
      <div
        role="dialog"
        aria-modal="true"
        className="
          w-full
          max-w-[500px]
          overflow-hidden
          rounded-[24px]
          bg-white
          shadow-[0_24px_80px_rgba(15,23,42,0.18)]
        "
      >
        <div
          className="
            flex
            items-start
            justify-between
            border-b
            border-slate-100
            px-6
            py-5
          "
        >
          <div>
            <h2
              className="
                text-xl
                font-bold
                text-[#18234A]
              "
            >
              {isEditing ? "Editar especialidade" : "Nova especialidade"}
            </h2>

            <p
              className="
                mt-1
                text-sm
                text-slate-500
              "
            >
              {isEditing
                ? "Atualize o nome da especialidade."
                : "Cadastre uma nova especialidade médica."}
            </p>
          </div>

          <button
            type="button"
            onClick={onClose}
            disabled={isSubmitting}
            className="
              flex
              size-10
              items-center
              justify-center
              rounded-xl
              text-slate-400
              transition

              hover:bg-slate-100
            "
          >
            <X size={20} />
          </button>
        </div>

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="
            px-6
            py-6
          "
        >
          {serverError && (
            <div
              className="
                mb-5
                rounded-xl
                border
                border-red-100
                bg-red-50
                p-4
                text-sm
                text-red-700
              "
            >
              {serverError}
            </div>
          )}

          <label
            className="
              mb-2
              block
              text-sm
              font-semibold
              text-[#18234A]
            "
          >
            Nome
          </label>

          <input
            {...register("name")}
            placeholder="Ex.: Cardiologia"
            autoFocus
            className="
              h-11
              w-full
              rounded-xl
              border
              border-slate-200
              px-3
              text-sm
              text-[#18234A]
              outline-none
              transition

              focus:border-[#2448A5]
              focus:ring-2
              focus:ring-[#2448A5]/10
            "
          />

          {errors.name && (
            <p
              className="
                mt-1.5
                text-xs
                font-medium
                text-red-600
              "
            >
              {errors.name.message}
            </p>
          )}

          <div
            className="
              mt-6
              flex
              justify-end
              gap-3
              border-t
              border-slate-100
              pt-5
            "
          >
            <button
              type="button"
              onClick={onClose}
              disabled={isSubmitting}
              className="
                h-11
                rounded-xl
                border
                border-slate-200
                px-5
                text-sm
                font-semibold
                text-slate-600
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

                disabled:opacity-60
              "
            >
              {isSubmitting
                ? "Salvando..."
                : isEditing
                  ? "Salvar alterações"
                  : "Cadastrar"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function getApiErrorMessage(error: unknown) {
  if (!axios.isAxiosError<ApiProblem>(error)) {
    return "Ocorreu um erro inesperado.";
  }

  const data = error.response?.data;

  if (data?.detail) {
    return data.detail;
  }

  if (data?.title) {
    return data.title;
  }

  return "Não foi possível salvar a especialidade.";
}
