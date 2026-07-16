import { Plus, Search, Tags } from "lucide-react";
import { useMemo, useState } from "react";
import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DeleteSpecialtyDialog } from "@/features/specialties/components/DeleteSpecialtyDialog";
import { SpecialtiesTable } from "@/features/specialties/components/SpecialtiesTable";
import { SpecialtyFormDialog } from "@/features/specialties/components/SpecialtyFormDialog";
import { useSpecialties } from "@/features/specialties/hooks/useSpecialties";
import type { Specialty } from "@/features/specialties/types/specialty.types";

export function SpecialtiesPage() {
  const [search, setSearch] = useState("");

  const [includeInactive, setIncludeInactive] = useState(false);

  const [isFormOpen, setIsFormOpen] = useState(false);

  const [editingSpecialty, setEditingSpecialty] = useState<Specialty | null>(
    null,
  );

  const [deletingSpecialty, setDeletingSpecialty] = useState<Specialty | null>(
    null,
  );

  const { data, isLoading, isError, refetch } = useSpecialties(includeInactive);

  const filteredSpecialties = useMemo(() => {
    if (!data) {
      return [];
    }

    const normalized = search.trim().toLowerCase();

    if (!normalized) {
      return data;
    }

    return data.filter((specialty) =>
      specialty.name.toLowerCase().includes(normalized),
    );
  }, [data, search]);

  function handleCreate() {
    setEditingSpecialty(null);

    setIsFormOpen(true);
  }

  function handleEdit(specialty: Specialty) {
    setEditingSpecialty(specialty);

    setIsFormOpen(true);
  }

  function handleCloseForm() {
    setIsFormOpen(false);

    setEditingSpecialty(null);
  }

  return (
    <DashboardLayout>
      <div
        className="
          py-6

          lg:py-8
        "
      >
        <div
          className="
            flex
            flex-col
            gap-5

            sm:flex-row
            sm:items-end
            sm:justify-between
          "
        >
          <div>
            <span
              className="
                inline-flex
                items-center
                gap-2
                rounded-full
                bg-[#EDF3FF]
                px-3
                py-1.5
                text-xs
                font-semibold
                text-[#2448A5]
              "
            >
              <Tags size={14} />
              Gestão de especialidades
            </span>

            <h1
              className="
                mt-4
                text-2xl
                font-bold
                text-[#18234A]

                sm:text-3xl
              "
            >
              Especialidades
            </h1>

            <p
              className="
                mt-2
                text-sm
                text-slate-500
              "
            >
              Gerencie as especialidades disponíveis para os médicos.
            </p>
          </div>

          <button
            type="button"
            onClick={handleCreate}
            className="
              flex
              h-11
              items-center
              justify-center
              gap-2
              rounded-xl
              bg-[#2448A5]
              px-4
              text-sm
              font-semibold
              text-white

              hover:bg-[#172D6B]
            "
          >
            <Plus size={18} />
            Nova especialidade
          </button>
        </div>

        <div
          className="
            mt-8
            flex
            flex-col
            gap-4
            rounded-2xl
            border
            border-slate-100
            bg-white
            p-4

            sm:flex-row
            sm:items-center
            sm:justify-between
          "
        >
          <div
            className="
              relative
              w-full

              sm:max-w-md
            "
          >
            <Search
              size={18}
              className="
                absolute
                left-4
                top-1/2
                -translate-y-1/2
                text-slate-400
              "
            />

            <input
              type="search"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Buscar especialidade"
              className="
                h-11
                w-full
                rounded-xl
                border
                border-slate-200
                bg-[#F9FAFC]
                pl-11
                pr-4
                text-sm
                outline-none

                focus:border-[#2448A5]
              "
            />
          </div>

          <label
            className="
              flex
              cursor-pointer
              items-center
              gap-3
              text-sm
              font-medium
              text-slate-600
            "
          >
            <input
              type="checkbox"
              checked={includeInactive}
              onChange={() => setIncludeInactive((current) => !current)}
              className="
                size-4
                accent-[#2448A5]
              "
            />
            Mostrar inativas
          </label>
        </div>

        <div
          className="
            mt-5
          "
        >
          {isLoading && (
            <div
              className="
                rounded-2xl
                bg-white
                p-8
                text-center
                text-sm
                text-slate-500
              "
            >
              Carregando especialidades...
            </div>
          )}

          {isError && (
            <div
              className="
                rounded-2xl
                border
                border-red-100
                bg-red-50
                p-6
              "
            >
              <p
                className="
                  font-semibold
                  text-red-700
                "
              >
                Não foi possível carregar as especialidades.
              </p>

              <button
                type="button"
                onClick={() => {
                  void refetch();
                }}
                className="
                  mt-4
                  rounded-xl
                  bg-red-600
                  px-4
                  py-2
                  text-sm
                  font-semibold
                  text-white
                "
              >
                Tentar novamente
              </button>
            </div>
          )}

          {data && (
            <SpecialtiesTable
              specialties={filteredSpecialties}
              onEdit={handleEdit}
              onDelete={setDeletingSpecialty}
            />
          )}
        </div>
      </div>

      <SpecialtyFormDialog
        isOpen={isFormOpen}
        specialty={editingSpecialty}
        onClose={handleCloseForm}
      />

      <DeleteSpecialtyDialog
        specialty={deletingSpecialty}
        onClose={() => setDeletingSpecialty(null)}
      />
    </DashboardLayout>
  );
}
