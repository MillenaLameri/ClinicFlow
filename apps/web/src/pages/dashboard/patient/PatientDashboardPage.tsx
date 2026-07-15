import { useAuth } from "@/features/auth/hooks/useAuth";

export function PatientDashboardPage() {
  const { user } = useAuth();

  return (
    <main className="min-h-dvh bg-[#F6F9FC] p-6">
      <div className="mx-auto max-w-7xl">
        <span className="text-sm font-medium text-[#2448A5]">
          Área do paciente
        </span>

        <h1 className="mt-2 text-3xl font-bold text-[#18234A]">
          Olá, {user?.fullName}
        </h1>

        <p className="mt-2 text-slate-500">
          Encontre médicos e acompanhe suas consultas.
        </p>
      </div>
    </main>
  );
}
