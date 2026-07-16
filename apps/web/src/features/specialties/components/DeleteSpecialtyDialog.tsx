import { TriangleAlert } from "lucide-react";
import { useDeleteSpecialty } from "../hooks/useDeleteSpecialty";
import type { Specialty } from "../types/specialty.types";

type DeleteSpecialtyDialogProps = {
  specialty: Specialty | null;

  onClose: () => void;
};

export function DeleteSpecialtyDialog({
  specialty,
  onClose,
}: DeleteSpecialtyDialogProps) {
  const deleteSpecialty = useDeleteSpecialty();

  if (!specialty) {
    return null;
  }

  async function handleConfirm() {
    await deleteSpecialty.mutateAsync(specialty!.id);

    onClose();
  }

  return (
    <div
      className="
        fixed
        inset-0
        z-[110]
        flex
        items-center
        justify-center
        bg-slate-950/40
        px-4
        backdrop-blur-[2px]
      "
    >
      <div
        className="
          w-full
          max-w-[440px]
          rounded-[24px]
          bg-white
          p-6
          shadow-[0_24px_80px_rgba(15,23,42,0.18)]
        "
      >
        <div
          className="
            flex
            size-12
            items-center
            justify-center
            rounded-2xl
            bg-red-50
            text-red-600
          "
        >
          <TriangleAlert size={23} />
        </div>

        <h2
          className="
            mt-5
            text-xl
            font-bold
            text-[#18234A]
          "
        >
          Desativar especialidade?
        </h2>

        <p
          className="
            mt-2
            text-sm
            leading-6
            text-slate-500
          "
        >
          A especialidade <strong>{specialty.name}</strong> deixará de aparecer
          nos novos cadastros de médicos.
        </p>

        <div
          className="
            mt-6
            flex
            justify-end
            gap-3
          "
        >
          <button
            type="button"
            onClick={onClose}
            disabled={deleteSpecialty.isPending}
            className="
              h-11
              rounded-xl
              border
              border-slate-200
              px-4
              text-sm
              font-semibold
              text-slate-600
            "
          >
            Cancelar
          </button>

          <button
            type="button"
            onClick={() => {
              void handleConfirm();
            }}
            disabled={deleteSpecialty.isPending}
            className="
              h-11
              rounded-xl
              bg-red-600
              px-4
              text-sm
              font-semibold
              text-white

              disabled:opacity-60
            "
          >
            {deleteSpecialty.isPending ? "Desativando..." : "Desativar"}
          </button>
        </div>
      </div>
    </div>
  );
}
