import { Plus, Search, Stethoscope } from "lucide-react";
import { useState } from "react";
import type { FormEvent } from "react";
import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { DoctorFormDialog } from "@/features/doctors/components/DoctorFormDialog";
import { DoctorsPagination } from "@/features/doctors/components/DoctorsPagination";
import { DoctorsTable } from "@/features/doctors/components/DoctorsTable";
import { useDoctors } from "@/features/doctors/hooks/useDoctors";
import type { Doctor } from "@/features/doctors/types/doctor.types";

const PAGE_SIZE = 10;

export function DoctorsPage() {
  const [page, setPage] = useState(1);

  const [searchInput, setSearchInput] = useState("");

  const [search, setSearch] = useState("");

  const [includeInactive, setIncludeInactive] = useState(false);

  const [isDoctorDialogOpen, setIsDoctorDialogOpen] = useState(false);

  const [editingDoctor, setEditingDoctor] = useState<Doctor | null>(null);

  const { data, isLoading, isError, refetch } = useDoctors({
    page,
    pageSize: PAGE_SIZE,
    search,
    includeInactive,
  });

  function handleCreateDoctor() {
    setEditingDoctor(null);

    setIsDoctorDialogOpen(true);
  }

  function handleEditDoctor(doctor: Doctor) {
    setEditingDoctor(doctor);

    setIsDoctorDialogOpen(true);
  }

  function handleCloseDoctorDialog() {
    setIsDoctorDialogOpen(false);

    setEditingDoctor(null);
  }

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setPage(1);

    setSearch(searchInput.trim());
  }

  function handleInactiveChange() {
    setPage(1);

    setIncludeInactive((current) => !current);
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
              <Stethoscope size={14} />
              Gestão de médicos
            </span>

            <h1
              className="
                mt-4
                text-2xl
                font-bold
                tracking-tight
                text-[#18234A]

                sm:text-3xl
              "
            >
              Médicos
            </h1>

            <p
              className="
                mt-2
                max-w-xl
                text-sm
                leading-6
                text-slate-500
              "
            >
              Consulte e gerencie os profissionais cadastrados no ClinicFlow.
            </p>
          </div>

          <button
            type="button"
            onClick={handleCreateDoctor}
            className="
              flex
              h-11
              w-full
              items-center
              justify-center
              gap-2
              rounded-xl
              bg-[#2448A5]
              px-4
              text-sm
              font-semibold
              text-white
              transition

              hover:bg-[#172D6B]

              focus:outline-none
              focus:ring-2
              focus:ring-[#2448A5]/20

              sm:w-auto
            "
          >
            <Plus size={18} />
            Novo médico
          </button>
        </div>

        <div
          className="
            mt-8
            rounded-2xl
            border
            border-slate-100
            bg-white
            p-4

            sm:p-5
          "
        >
          <div
            className="
              flex
              flex-col
              gap-4

              lg:flex-row
              lg:items-center
              lg:justify-between
            "
          >
            <form
              onSubmit={handleSearch}
              className="
                flex
                w-full
                gap-2

                lg:max-w-xl
              "
            >
              <div
                className="
                  relative
                  flex-1
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
                  value={searchInput}
                  onChange={(event) => setSearchInput(event.target.value)}
                  placeholder="Buscar por nome, CRM ou e-mail"
                  className="
                    h-11
                    w-full
                    rounded-xl
                    border
                    border-slate-200
                    bg-[#F9FAFC]
                    pl-11
                    pr-4
                    text-base
                    text-[#18234A]
                    outline-none
                    transition

                    placeholder:text-slate-400

                    focus:border-[#2448A5]
                    focus:ring-2
                    focus:ring-[#2448A5]/10

                    sm:text-sm
                  "
                />
              </div>

              <button
                type="submit"
                className="
                  h-11
                  shrink-0
                  rounded-xl
                  border
                  border-slate-200
                  bg-white
                  px-4
                  text-sm
                  font-semibold
                  text-[#18234A]
                  transition

                  hover:bg-slate-50

                  focus:outline-none
                  focus:ring-2
                  focus:ring-[#2448A5]/20
                "
              >
                Buscar
              </button>
            </form>

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
                onChange={handleInactiveChange}
                className="
                  size-4
                  accent-[#2448A5]
                "
              />
              Mostrar médicos inativos
            </label>
          </div>
        </div>

        <div className="mt-5">
          {isLoading && <DoctorsLoading />}

          {isError && (
            <DoctorsError
              onRetry={() => {
                void refetch();
              }}
            />
          )}

          {data && (
            <>
              <DoctorsTable doctors={data.items} onEdit={handleEditDoctor} />

              <DoctorsPagination
                page={data.page}
                totalPages={data.totalPages}
                totalItems={data.totalItems}
                hasPreviousPage={data.hasPreviousPage}
                hasNextPage={data.hasNextPage}
                onPageChange={setPage}
              />
            </>
          )}
        </div>
      </div>

      <DoctorFormDialog
        isOpen={isDoctorDialogOpen}
        mode={editingDoctor ? "edit" : "create"}
        doctor={editingDoctor}
        onClose={handleCloseDoctorDialog}
      />
    </DashboardLayout>
  );
}

function DoctorsLoading() {
  return (
    <div
      className="
        overflow-hidden
        rounded-2xl
        border
        border-slate-100
        bg-white
      "
    >
      {Array.from({
        length: 5,
      }).map((_, index) => (
        <div
          key={index}
          className="
              flex
              animate-pulse
              items-center
              gap-4
              border-b
              border-slate-100
              p-5

              last:border-none
            "
        >
          <div
            className="
                size-10
                shrink-0
                rounded-xl
                bg-slate-100
              "
          />

          <div
            className="
                flex-1
              "
          >
            <div
              className="
                  h-4
                  w-40
                  rounded
                  bg-slate-100
                "
            />

            <div
              className="
                  mt-2
                  h-3
                  w-56
                  max-w-full
                  rounded
                  bg-slate-100
                "
            />
          </div>
        </div>
      ))}
    </div>
  );
}

type DoctorsErrorProps = {
  onRetry: () => void;
};

function DoctorsError({ onRetry }: DoctorsErrorProps) {
  return (
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
        Não foi possível carregar os médicos.
      </p>

      <p
        className="
          mt-1
          text-sm
          text-red-600
        "
      >
        Tente carregar os dados novamente.
      </p>

      <button
        type="button"
        onClick={onRetry}
        className="
          mt-4
          rounded-xl
          bg-red-600
          px-4
          py-2
          text-sm
          font-semibold
          text-white
          transition

          hover:bg-red-700

          focus:outline-none
          focus:ring-2
          focus:ring-red-600/20
        "
      >
        Tentar novamente
      </button>
    </div>
  );
}
