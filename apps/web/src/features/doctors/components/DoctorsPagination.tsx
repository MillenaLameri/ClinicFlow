import { ChevronLeft, ChevronRight } from "lucide-react";

type DoctorsPaginationProps = {
  page: number;
  totalPages: number;
  totalItems: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  onPageChange: (page: number) => void;
};

export function DoctorsPagination({
  page,
  totalPages,
  totalItems,
  hasPreviousPage,
  hasNextPage,
  onPageChange,
}: DoctorsPaginationProps) {
  if (totalItems === 0) {
    return null;
  }

  return (
    <div
      className="
        mt-5
        flex
        flex-col
        gap-4
        rounded-2xl
        border
        border-slate-100
        bg-white
        px-4
        py-4

        sm:flex-row
        sm:items-center
        sm:justify-between
      "
    >
      <p
        className="
          text-center
          text-xs
          text-slate-500

          sm:text-left
        "
      >
        Página{" "}
        <strong
          className="
            text-[#18234A]
          "
        >
          {page}
        </strong>{" "}
        de{" "}
        <strong
          className="
            text-[#18234A]
          "
        >
          {totalPages}
        </strong>
        {" · "}
        {totalItems} {totalItems === 1 ? "médico" : "médicos"}
      </p>

      <div
        className="
          flex
          items-center
          justify-center
          gap-2
        "
      >
        <button
          type="button"
          disabled={!hasPreviousPage}
          onClick={() => onPageChange(page - 1)}
          className="
            flex
            h-10
            items-center
            gap-1
            rounded-xl
            border
            border-slate-200
            bg-white
            px-3
            text-xs
            font-semibold
            text-slate-600
            transition

            hover:bg-slate-50

            disabled:cursor-not-allowed
            disabled:opacity-40
          "
        >
          <ChevronLeft size={16} />
          Anterior
        </button>

        <button
          type="button"
          disabled={!hasNextPage}
          onClick={() => onPageChange(page + 1)}
          className="
            flex
            h-10
            items-center
            gap-1
            rounded-xl
            bg-[#2448A5]
            px-3
            text-xs
            font-semibold
            text-white
            transition

            hover:bg-[#172D6B]

            disabled:cursor-not-allowed
            disabled:opacity-40
          "
        >
          Próxima
          <ChevronRight size={16} />
        </button>
      </div>
    </div>
  );
}
