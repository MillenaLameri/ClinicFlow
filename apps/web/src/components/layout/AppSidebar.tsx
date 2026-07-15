import {
  CalendarDays,
  LayoutDashboard,
  LogOut,
  Stethoscope,
  Users,
  X,
} from "lucide-react";
import { NavLink, useNavigate } from "react-router";
import { ClinicFlowLogo } from "@/features/auth/components/ClinicFlowLogo";
import { useAuth } from "@/features/auth/hooks/useAuth";

type AppSidebarProps = {
  isOpen: boolean;
  onClose: () => void;
};

const navigationItems = [
  {
    label: "Dashboard",
    path: "/dashboard/admin",
    icon: LayoutDashboard,
  },
  {
    label: "Médicos",
    path: "/admin/doctors",
    icon: Stethoscope,
  },
  {
    label: "Pacientes",
    path: "/admin/patients",
    icon: Users,
  },
  {
    label: "Consultas",
    path: "/admin/appointments",
    icon: CalendarDays,
  },
];

export function AppSidebar({ isOpen, onClose }: AppSidebarProps) {
  const navigate = useNavigate();

  const { signOut } = useAuth();

  function handleLogout() {
    signOut();

    navigate("/login", {
      replace: true,
    });
  }

  return (
    <>
      {isOpen && (
        <button
          type="button"
          aria-label="Fechar menu"
          onClick={onClose}
          className="
            fixed
            inset-0
            z-40
            bg-slate-950/40
            backdrop-blur-[2px]

            lg:hidden
          "
        />
      )}

      <aside
        className={[
          `
            fixed
            inset-y-0
            left-0
            z-50
            flex
            w-[280px]
            flex-col
            bg-[#172D6B]
            px-5
            py-6
            text-white
            transition-transform
            duration-300

            lg:static
            lg:z-auto
            lg:h-full
            lg:translate-x-0
            lg:rounded-[28px]
          `,
          isOpen ? "translate-x-0" : "-translate-x-full",
        ].join(" ")}
      >
        <div
          className="
            flex
            items-center
            justify-between
          "
        >
          <ClinicFlowLogo light />

          <button
            type="button"
            aria-label="Fechar menu"
            onClick={onClose}
            className="
              flex
              size-10
              items-center
              justify-center
              rounded-xl
              text-blue-100
              transition

              hover:bg-white/10

              lg:hidden
            "
          >
            <X size={20} />
          </button>
        </div>

        <div
          className="
            mt-10
            flex
            flex-1
            flex-col
          "
        >
          <p
            className="
              px-3
              text-[11px]
              font-semibold
              uppercase
              tracking-[0.15em]
              text-blue-200/60
            "
          >
            Administração
          </p>

          <nav
            className="
              mt-3
              space-y-1
            "
          >
            {navigationItems.map((item) => {
              const Icon = item.icon;

              return (
                <NavLink
                  key={item.path}
                  to={item.path}
                  onClick={onClose}
                  className={({ isActive }) =>
                    [
                      `
                          flex
                          items-center
                          gap-3
                          rounded-xl
                          px-3
                          py-3
                          text-sm
                          font-medium
                          transition
                        `,
                      isActive
                        ? "bg-white text-[#172D6B]"
                        : "text-blue-100 hover:bg-white/10 hover:text-white",
                    ].join(" ")
                  }
                >
                  <Icon size={19} />

                  {item.label}
                </NavLink>
              );
            })}
          </nav>
        </div>

        <button
          type="button"
          onClick={handleLogout}
          className="
            flex
            w-full
            items-center
            gap-3
            rounded-xl
            px-3
            py-3
            text-sm
            font-medium
            text-blue-100
            transition

            hover:bg-white/10
            hover:text-white
          "
        >
          <LogOut size={19} />
          Sair
        </button>
      </aside>
    </>
  );
}
