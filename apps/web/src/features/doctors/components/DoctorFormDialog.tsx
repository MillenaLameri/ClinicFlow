import axios from "axios";
import { X } from "lucide-react";
import { useEffect, useState } from "react";
import { useSpecialties } from "@/features/specialties/hooks/useSpecialties";
import { useCreateDoctor } from "../hooks/useCreateDoctor";
import { useUpdateDoctor } from "../hooks/useUpdateDoctor";
import { DoctorForm } from "./DoctorForm";
import type { DoctorFormValues } from "../schemas/doctorSchema";
import type { Doctor } from "../types/doctor.types";

type DoctorFormDialogProps = {
  isOpen: boolean;

  mode: "create" | "edit";

  doctor: Doctor | null;

  onClose: () => void;
};

type ApiProblem = {
  title?: string;
  detail?: string;

  errors?: Record<string, string[]>;
};

export function DoctorFormDialog({
  isOpen,
  mode,
  doctor,
  onClose,
}: DoctorFormDialogProps) {
  const [serverError, setServerError] = useState<string | null>(null);

  const specialtiesQuery = useSpecialties();

  const createDoctor = useCreateDoctor();

  const updateDoctor = useUpdateDoctor();

  const isSubmitting = createDoctor.isPending || updateDoctor.isPending;

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    setServerError(null);

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape" && !isSubmitting) {
        onClose();
      }
    }

    window.addEventListener("keydown", handleKeyDown);

    return () => {
      window.removeEventListener("keydown", handleKeyDown);
    };
  }, [isOpen, isSubmitting, onClose]);

  if (!isOpen) {
    return null;
  }

  async function handleSubmit(values: DoctorFormValues) {
    setServerError(null);

    const commonData = {
      fullName: values.fullName.trim(),

      email: values.email.trim().toLowerCase(),

      phone: values.phone.trim() || null,

      specialtyId: values.specialtyId,
    };

    try {
      if (mode === "create") {
        await createDoctor.mutateAsync({
          ...commonData,

          crmNumber: values.crmNumber.trim().toUpperCase(),

          crmState: values.crmState.trim().toUpperCase(),
        });
      } else {
        if (!doctor) {
          return;
        }

        await updateDoctor.mutateAsync({
          id: doctor.id,
          data: commonData,
        });
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
        overflow-y-auto
        bg-slate-950/40
        px-4
        py-6
        backdrop-blur-[2px]
      "
    >
      <div
        className="
          flex
          min-h-full
          items-center
          justify-center
        "
      >
        <div
          role="dialog"
          aria-modal="true"
          aria-labelledby="doctor-dialog-title"
          className="
            w-full
            max-w-[600px]
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
              gap-4
              border-b
              border-slate-100
              px-5
              py-5

              sm:px-6
            "
          >
            <div>
              <h2
                id="doctor-dialog-title"
                className="
                  text-xl
                  font-bold
                  text-[#18234A]
                "
              >
                {mode === "create" ? "Novo médico" : "Editar médico"}
              </h2>

              <p
                className="
                  mt-1
                  text-sm
                  text-slate-500
                "
              >
                {mode === "create"
                  ? "Cadastre um novo profissional no ClinicFlow."
                  : "Atualize os dados do profissional."}
              </p>
            </div>

            <button
              type="button"
              aria-label="Fechar"
              disabled={isSubmitting}
              onClick={onClose}
              className="
                flex
                size-10
                shrink-0
                items-center
                justify-center
                rounded-xl
                text-slate-400
                transition

                hover:bg-slate-100
                hover:text-slate-600
              "
            >
              <X size={20} />
            </button>
          </div>

          <div
            className="
              px-5
              py-6

              sm:px-6
            "
          >
            {specialtiesQuery.isLoading && (
              <p
                className="
                  py-8
                  text-center
                  text-sm
                  text-slate-500
                "
              >
                Carregando especialidades...
              </p>
            )}

            {specialtiesQuery.isError && (
              <div
                className="
                  rounded-xl
                  border
                  border-red-100
                  bg-red-50
                  p-4
                  text-sm
                  text-red-700
                "
              >
                Não foi possível carregar as especialidades.
              </div>
            )}

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
                  font-medium
                  text-red-700
                "
              >
                {serverError}
              </div>
            )}

            {specialtiesQuery.data && (
              <DoctorForm
                mode={mode}
                doctor={doctor}
                specialties={specialtiesQuery.data}
                isSubmitting={isSubmitting}
                onCancel={onClose}
                onSubmit={handleSubmit}
              />
            )}
          </div>
        </div>
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

  if (data?.errors) {
    const firstError = Object.values(data.errors).flat().at(0);

    if (firstError) {
      return firstError;
    }
  }

  if (data?.title) {
    return data.title;
  }

  return "Não foi possível salvar o médico.";
}
