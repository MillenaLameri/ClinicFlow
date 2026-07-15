import {
  CalendarCheck,
  CalendarDays,
  CircleCheckBig,
  Stethoscope,
  UserRoundX,
  Users,
} from "lucide-react";
import { DashboardLayout } from "@/components/layout/DashboardLayout";
import { AdminStatsCard } from "@/features/dashboard/admin/components/AdminStatsCard";
import { useAdminDashboard } from "@/features/dashboard/admin/hooks/useAdminDashboard";
import { useAuth } from "@/features/auth/hooks/useAuth";

export function AdminDashboardPage() {
  const { user } = useAuth();

  const { data, isLoading, isError, refetch } = useAdminDashboard();

  const firstName = user?.fullName?.split(" ")[0] ?? "Administrador";

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
            gap-4

            sm:flex-row
            sm:items-end
            sm:justify-between
          "
        >
          <div>
            <span
              className="
                inline-flex
                rounded-full
                bg-[#EDF3FF]
                px-3
                py-1.5
                text-xs
                font-semibold
                text-[#2448A5]
              "
            >
              Visão geral
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
              Olá, {firstName}
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
              Acompanhe os principais indicadores e gerencie sua clínica em um
              único lugar.
            </p>
          </div>
        </div>

        {isLoading && <DashboardLoading />}

        {isError && (
          <DashboardError
            onRetry={() => {
              void refetch();
            }}
          />
        )}

        {data && (
          <>
            <div
              className="
                mt-8
                grid
                grid-cols-1
                gap-4

                sm:grid-cols-2

                xl:grid-cols-3

                2xl:grid-cols-6
              "
            >
              <AdminStatsCard
                title="Médicos"
                value={data.totalDoctors}
                description="Profissionais cadastrados"
                icon={Stethoscope}
                variant="blue"
              />

              <AdminStatsCard
                title="Pacientes"
                value={data.totalPatients}
                description="Pacientes cadastrados"
                icon={Users}
                variant="green"
              />

              <AdminStatsCard
                title="Consultas hoje"
                value={data.appointmentsToday}
                description="Agendamentos para hoje"
                icon={CalendarDays}
                variant="yellow"
              />

              <AdminStatsCard
                title="Agendadas"
                value={data.scheduledAppointments}
                description="Consultas aguardando atendimento"
                icon={CalendarCheck}
                variant="blue"
              />

              <AdminStatsCard
                title="Concluídas hoje"
                value={data.completedToday}
                description="Atendimentos concluídos hoje"
                icon={CircleCheckBig}
                variant="green"
              />

              <AdminStatsCard
                title="Faltas hoje"
                value={data.noShowToday}
                description="Pacientes que não compareceram"
                icon={UserRoundX}
                variant="pink"
              />
            </div>
          </>
        )}
      </div>
    </DashboardLayout>
  );
}

function DashboardLoading() {
  return (
    <div
      className="
        mt-8
        grid
        grid-cols-1
        gap-4

        sm:grid-cols-2

        xl:grid-cols-3

        2xl:grid-cols-6
      "
    >
      {Array.from({
        length: 6,
      }).map((_, index) => (
        <div
          key={index}
          className="
            h-[185px]
            animate-pulse
            rounded-2xl
            border
            border-slate-100
            bg-white
            p-5
          "
        >
          <div
            className="
              size-11
              rounded-xl
              bg-slate-100
            "
          />

          <div
            className="
              mt-5
              h-4
              w-24
              rounded
              bg-slate-100
            "
          />

          <div
            className="
              mt-3
              h-9
              w-16
              rounded
              bg-slate-100
            "
          />

          <div
            className="
              mt-3
              h-3
              w-32
              rounded
              bg-slate-100
            "
          />
        </div>
      ))}
    </div>
  );
}

type DashboardErrorProps = {
  onRetry: () => void;
};

function DashboardError({ onRetry }: DashboardErrorProps) {
  return (
    <div
      className="
        mt-8
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
        Não foi possível carregar o dashboard.
      </p>

      <p
        className="
          mt-1
          text-sm
          text-red-600
        "
      >
        Verifique sua conexão e tente novamente.
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
        "
      >
        Tentar novamente
      </button>
    </div>
  );
}
