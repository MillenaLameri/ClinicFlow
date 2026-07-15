import { Mail, Phone, Stethoscope } from "lucide-react";
import { DoctorStatusBadge } from "./DoctorStatusBadge";
import type { Doctor } from "../types/doctor.types";

type DoctorsTableProps = {
  doctors: Doctor[];
};

export function DoctorsTable({ doctors }: DoctorsTableProps) {
  if (doctors.length === 0) {
    return (
      <div
        className="
          flex
          min-h-[300px]
          flex-col
          items-center
          justify-center
          rounded-2xl
          border
          border-dashed
          border-slate-200
          bg-white
          px-6
          text-center
        "
      >
        <div
          className="
            flex
            size-12
            items-center
            justify-center
            rounded-2xl
            bg-[#EDF3FF]
            text-[#2448A5]
          "
        >
          <Stethoscope size={22} />
        </div>

        <h2
          className="
            mt-4
            font-semibold
            text-[#18234A]
          "
        >
          Nenhum médico encontrado
        </h2>

        <p
          className="
            mt-2
            max-w-sm
            text-sm
            leading-6
            text-slate-500
          "
        >
          Não encontramos médicos para os filtros informados.
        </p>
      </div>
    );
  }

  return (
    <>
      {/* Desktop */}
      <div
        className="
          hidden
          overflow-hidden
          rounded-2xl
          border
          border-slate-100
          bg-white

          md:block
        "
      >
        <div className="overflow-x-auto">
          <table
            className="
              w-full
              min-w-[850px]
              border-collapse
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
                    tracking-wide
                    text-slate-400
                  "
                >
                  Médico
                </th>

                <th
                  className="
                    px-5
                    py-4
                    text-xs
                    font-semibold
                    uppercase
                    tracking-wide
                    text-slate-400
                  "
                >
                  CRM
                </th>

                <th
                  className="
                    px-5
                    py-4
                    text-xs
                    font-semibold
                    uppercase
                    tracking-wide
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
                    tracking-wide
                    text-slate-400
                  "
                >
                  Contato
                </th>

                <th
                  className="
                    px-5
                    py-4
                    text-xs
                    font-semibold
                    uppercase
                    tracking-wide
                    text-slate-400
                  "
                >
                  Status
                </th>
              </tr>
            </thead>

            <tbody>
              {doctors.map((doctor) => (
                <tr
                  key={doctor.id}
                  className="
                      border-b
                      border-slate-100
                      transition

                      last:border-none

                      hover:bg-slate-50/60
                    "
                >
                  <td
                    className="
                        px-5
                        py-4
                      "
                  >
                    <div
                      className="
                          flex
                          items-center
                          gap-3
                        "
                    >
                      <DoctorAvatar name={doctor.fullName} />

                      <div
                        className="
                            min-w-0
                          "
                      >
                        <p
                          className="
                              truncate
                              text-sm
                              font-semibold
                              text-[#18234A]
                            "
                        >
                          {doctor.fullName}
                        </p>

                        <p
                          className="
                              mt-1
                              truncate
                              text-xs
                              text-slate-400
                            "
                        >
                          {doctor.email}
                        </p>
                      </div>
                    </div>
                  </td>

                  <td
                    className="
                        px-5
                        py-4
                        text-sm
                        font-medium
                        text-slate-600
                      "
                  >
                    {doctor.crmNumber}/{doctor.crmState}
                  </td>

                  <td
                    className="
                        px-5
                        py-4
                      "
                  >
                    <span
                      className="
                          inline-flex
                          rounded-lg
                          bg-[#EDF3FF]
                          px-2.5
                          py-1.5
                          text-xs
                          font-medium
                          text-[#2448A5]
                        "
                    >
                      {doctor.specialty.name}
                    </span>
                  </td>

                  <td
                    className="
                        px-5
                        py-4
                      "
                  >
                    <div
                      className="
                          space-y-1
                          text-xs
                          text-slate-500
                        "
                    >
                      <div
                        className="
                            flex
                            items-center
                            gap-2
                          "
                      >
                        <Mail size={14} />

                        <span>{doctor.email}</span>
                      </div>

                      {doctor.phone && (
                        <div
                          className="
                              flex
                              items-center
                              gap-2
                            "
                        >
                          <Phone size={14} />

                          <span>{doctor.phone}</span>
                        </div>
                      )}
                    </div>
                  </td>

                  <td
                    className="
                        px-5
                        py-4
                      "
                  >
                    <DoctorStatusBadge isActive={doctor.isActive} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Mobile */}
      <div
        className="
          grid
          gap-3

          md:hidden
        "
      >
        {doctors.map((doctor) => (
          <article
            key={doctor.id}
            className="
                rounded-2xl
                border
                border-slate-100
                bg-white
                p-4
              "
          >
            <div
              className="
                  flex
                  items-start
                  justify-between
                  gap-3
                "
            >
              <div
                className="
                    flex
                    min-w-0
                    items-center
                    gap-3
                  "
              >
                <DoctorAvatar name={doctor.fullName} />

                <div
                  className="
                      min-w-0
                    "
                >
                  <h2
                    className="
                        truncate
                        text-sm
                        font-semibold
                        text-[#18234A]
                      "
                  >
                    {doctor.fullName}
                  </h2>

                  <p
                    className="
                        mt-1
                        text-xs
                        text-slate-400
                      "
                  >
                    CRM {doctor.crmNumber}/{doctor.crmState}
                  </p>
                </div>
              </div>

              <DoctorStatusBadge isActive={doctor.isActive} />
            </div>

            <div
              className="
                  mt-4
                  border-t
                  border-slate-100
                  pt-4
                "
            >
              <p
                className="
                    text-xs
                    font-medium
                    text-[#2448A5]
                  "
              >
                {doctor.specialty.name}
              </p>

              <div
                className="
                    mt-3
                    space-y-2
                    text-xs
                    text-slate-500
                  "
              >
                <div
                  className="
                      flex
                      items-center
                      gap-2
                    "
                >
                  <Mail size={14} />

                  <span
                    className="
                        truncate
                      "
                  >
                    {doctor.email}
                  </span>
                </div>

                {doctor.phone && (
                  <div
                    className="
                        flex
                        items-center
                        gap-2
                      "
                  >
                    <Phone size={14} />

                    {doctor.phone}
                  </div>
                )}
              </div>
            </div>
          </article>
        ))}
      </div>
    </>
  );
}

type DoctorAvatarProps = {
  name: string;
};

function DoctorAvatar({ name }: DoctorAvatarProps) {
  const initials = name
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0])
    .join("")
    .toUpperCase();

  return (
    <div
      className="
        flex
        size-10
        shrink-0
        items-center
        justify-center
        rounded-xl
        bg-[#EDF3FF]
        text-xs
        font-bold
        text-[#2448A5]
      "
    >
      {initials}
    </div>
  );
}
