import { Pencil, Trash2 } from "lucide-react";
import type { Specialty } from "../types/specialty.types";

type SpecialtiesTableProps = {
  specialties: Specialty[];

  onEdit: (specialty: Specialty) => void;

  onDelete: (specialty: Specialty) => void;
};

export function SpecialtiesTable({
  specialties,
  onEdit,
  onDelete,
}: SpecialtiesTableProps) {
  if (specialties.length === 0) {
    return (
      <div
        className="
          rounded-2xl
          border
          border-dashed
          border-slate-200
          bg-white
          px-6
          py-16
          text-center
        "
      >
        <p
          className="
            font-semibold
            text-[#18234A]
          "
        >
          Nenhuma especialidade encontrada
        </p>

        <p
          className="
            mt-2
            text-sm
            text-slate-500
          "
        >
          Cadastre uma nova especialidade para começar.
        </p>
      </div>
    );
  }

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
      <div
        className="
          overflow-x-auto
        "
      >
        <table
          className="
            w-full
            min-w-[600px]
            text-left
          "
        >
          <thead>
            <tr
              className="
                border-b
                border-slate-100
                bg-slate-50/70
              "
            >
              <th
                className="
                  px-5
                  py-4
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-400
                "
              >
                Especialidade
              </th>

              <th
                className="
                  px-5
                  py-4
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-400
                "
              >
                Status
              </th>

              <th
                className="
                  px-5
                  py-4
                  text-right
                  text-xs
                  font-semibold
                  uppercase
                  text-slate-400
                "
              >
                Ações
              </th>
            </tr>
          </thead>

          <tbody>
            {specialties.map((specialty) => (
              <tr
                key={specialty.id}
                className="
                    border-b
                    border-slate-100

                    last:border-none

                    hover:bg-slate-50/60
                  "
              >
                <td
                  className="
                      px-5
                      py-4
                      text-sm
                      font-semibold
                      text-[#18234A]
                    "
                >
                  {specialty.name}
                </td>

                <td
                  className="
                      px-5
                      py-4
                    "
                >
                  <span
                    className={[
                      `
                          inline-flex
                          rounded-full
                          px-2.5
                          py-1
                          text-xs
                          font-semibold
                        `,
                      specialty.isActive
                        ? "bg-emerald-50 text-emerald-700"
                        : "bg-slate-100 text-slate-500",
                    ].join(" ")}
                  >
                    {specialty.isActive ? "Ativa" : "Inativa"}
                  </span>
                </td>

                <td
                  className="
                      px-5
                      py-4
                      text-right
                    "
                >
                  <div
                    className="
                        flex
                        justify-end
                        gap-1
                      "
                  >
                    <button
                      type="button"
                      onClick={() => onEdit(specialty)}
                      className="
                          flex
                          size-9
                          items-center
                          justify-center
                          rounded-xl
                          text-slate-400

                          hover:bg-[#EDF3FF]
                          hover:text-[#2448A5]
                        "
                    >
                      <Pencil size={17} />
                    </button>

                    {specialty.isActive && (
                      <button
                        type="button"
                        onClick={() => onDelete(specialty)}
                        className="
                            flex
                            size-9
                            items-center
                            justify-center
                            rounded-xl
                            text-slate-400

                            hover:bg-red-50
                            hover:text-red-600
                          "
                      >
                        <Trash2 size={17} />
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
