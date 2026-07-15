import { Menu } from "lucide-react";
import { useAuth } from "@/features/auth/hooks/useAuth";

type AppHeaderProps = {
  onOpenMenu: () => void;
};

export function AppHeader({ onOpenMenu }: AppHeaderProps) {
  const { user } = useAuth();

  const firstName = user?.fullName?.split(" ")[0] ?? "Usuário";

  const initials =
    user?.fullName
      ?.split(" ")
      .filter(Boolean)
      .slice(0, 2)
      .map((name) => name[0])
      .join("")
      .toUpperCase() ?? "CF";

  return (
    <header
      className="
        flex
        h-16
        shrink-0
        items-center
        justify-between
        gap-4
        border-b
        border-slate-100
        bg-white
        px-4

        sm:px-6

        lg:h-20
        lg:border-none
        lg:bg-transparent
        lg:px-0
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
        <button
          type="button"
          aria-label="Abrir menu"
          onClick={onOpenMenu}
          className="
            flex
            size-10
            shrink-0
            items-center
            justify-center
            rounded-xl
            border
            border-slate-200
            bg-white
            text-[#18234A]

            lg:hidden
          "
        >
          <Menu size={20} />
        </button>

        <div
          className="
            hidden

            sm:block
          "
        >
          <p
            className="
              text-xs
              font-medium
              text-slate-400
            "
          >
            Bem-vindo ao ClinicFlow
          </p>

          <p
            className="
              mt-0.5
              text-sm
              font-semibold
              text-[#18234A]
            "
          >
            Olá, {firstName}
          </p>
        </div>
      </div>

      <div
        className="
          flex
          min-w-0
          items-center
          gap-3
        "
      >
        <div
          className="
            hidden
            max-w-[180px]
            text-right

            sm:block
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
            {user?.fullName}
          </p>

          <p
            className="
              truncate
              text-xs
              text-slate-400
            "
          >
            Administrador
          </p>
        </div>

        <div
          className="
            flex
            size-10
            shrink-0
            items-center
            justify-center
            rounded-xl
            bg-[#DCEBFF]
            text-sm
            font-bold
            text-[#2448A5]
          "
        >
          {initials}
        </div>
      </div>
    </header>
  );
}
