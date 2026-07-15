import {
  CalendarDays,
  HeartPulse,
  ShieldCheck,
  Stethoscope,
} from "lucide-react";

import { LoginFeatureCard } from "../../../features/auth/components/LoginFeatureCard";

const features = [
  {
    icon: CalendarDays,
    title: "Agenda",
    description: "Sempre organizada",
    variant: "glass" as const,
  },
  {
    icon: Stethoscope,
    title: "Médicos",
    description: "Mais conectados",
    variant: "green" as const,
    elevated: true,
  },
  {
    icon: HeartPulse,
    title: "Pacientes",
    description: "Melhor experiência",
    variant: "pink" as const,
  },
];

const profileInitials = ["A", "M", "J"];

export function LoginHero() {
  return (
    <section
      className="
    relative
    hidden
    h-full
    min-h-0
    overflow-hidden
    bg-[#172D6B]

    lg:flex
    lg:flex-col
    lg:justify-between
    lg:p-10

    xl:p-14
  "
    >
      <div
        className="
          absolute
          -right-24
          -top-24
          size-[430px]
          rounded-full
          bg-[#4B78DE]/35
          blur-3xl
        "
      />

      <div
        className="
          absolute
          -bottom-40
          -left-20
          size-[520px]
          rounded-full
          bg-[#72B7E9]/20
          blur-3xl
        "
      />

      <div
        className="
          absolute
          right-[12%]
          top-[24%]
          size-52
          rounded-full
          bg-white/5
        "
      />

      <div className="relative z-10">
        <span
          className="
            inline-flex
            items-center
            gap-2
            rounded-full
            border
            border-white/15
            bg-white/10
            px-4
            py-2
            text-xs
            font-medium
            text-blue-50
            backdrop-blur-sm
          "
        >
          <ShieldCheck size={15} />
          Saúde conectada e organizada
        </span>
      </div>

      <div
        className="
          relative
          z-10
          max-w-xl
        "
      >
        <h2
          className="
            text-5xl
            font-bold
            leading-[1.08]
            tracking-[-0.04em]
            text-white
            xl:text-6xl
          "
        >
          Cuidar da saúde pode ser mais simples.
        </h2>

        <p
          className="
            mt-6
            max-w-lg
            text-base
            leading-7
            text-blue-100/80
          "
        >
          Consultas, médicos e agendas conectados em uma experiência moderna
          para pacientes e profissionais de saúde.
        </p>

        <div
          className="
            mt-10
            grid
            max-w-lg
            grid-cols-3
            gap-3
          "
        >
          {features.map((feature) => (
            <LoginFeatureCard
              key={feature.title}
              icon={feature.icon}
              title={feature.title}
              description={feature.description}
              variant={feature.variant}
              elevated={feature.elevated}
            />
          ))}
        </div>
      </div>

      <div
        className="
          relative
          z-10
          flex
          items-center
          justify-between
          border-t
          border-white/10
          pt-6
        "
      >
        <p
          className="
            text-xs
            text-blue-100/60
          "
        >
          Uma plataforma para toda a jornada de cuidado.
        </p>

        <div className="flex -space-x-2">
          {profileInitials.map((initial) => (
            <div
              key={initial}
              className="
                  flex
                  size-8
                  items-center
                  justify-center
                  rounded-full
                  border-2
                  border-[#172D6B]
                  bg-[#DCEBFF]
                  text-[10px]
                  font-bold
                  text-[#2448A5]
                "
            >
              {initial}
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
