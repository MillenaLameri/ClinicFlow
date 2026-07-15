import type { LucideIcon } from "lucide-react";

type LoginFeatureCardVariant = "glass" | "green" | "pink";

type LoginFeatureCardProps = {
  icon: LucideIcon;
  title: string;
  description: string;
  variant: LoginFeatureCardVariant;
  elevated?: boolean;
};

const variantClasses: Record<
  LoginFeatureCardVariant,
  {
    container: string;
    icon: string;
    description: string;
  }
> = {
  glass: {
    container: "border-white/10 bg-white/10 text-white backdrop-blur-md",
    icon: "bg-[#DCEBFF] text-[#2448A5]",
    description: "text-blue-100/60",
  },

  green: {
    container: "border-white/10 bg-[#D8F1E7] text-[#18234A]",
    icon: "bg-white/70 text-[#207A5B]",
    description: "text-slate-600",
  },

  pink: {
    container: "border-white/10 bg-[#F5DCE7] text-[#18234A]",
    icon: "bg-white/70 text-[#9D5274]",
    description: "text-slate-600",
  },
};

export function LoginFeatureCard({
  icon: Icon,
  title,
  description,
  variant,
  elevated = false,
}: LoginFeatureCardProps) {
  const styles = variantClasses[variant];

  return (
    <article
      className={[
        "rounded-2xl border p-4",
        styles.container,
        elevated ? "translate-y-6" : "",
      ].join(" ")}
    >
      <div
        className={[
          "mb-7 flex size-10 items-center justify-center rounded-xl",
          styles.icon,
        ].join(" ")}
      >
        <Icon size={20} />
      </div>

      <p className="text-sm font-semibold">{title}</p>

      <p className={["mt-1 text-xs", styles.description].join(" ")}>
        {description}
      </p>
    </article>
  );
}
