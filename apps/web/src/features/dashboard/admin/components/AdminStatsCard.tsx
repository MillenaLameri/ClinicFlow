import type { LucideIcon } from "lucide-react";

type AdminStatsCardVariant = "blue" | "green" | "yellow" | "pink";

type AdminStatsCardProps = {
  title: string;
  value: number;
  description: string;
  icon: LucideIcon;
  variant?: AdminStatsCardVariant;
};

const variantClasses: Record<
  AdminStatsCardVariant,
  {
    iconContainer: string;
    icon: string;
  }
> = {
  blue: {
    iconContainer: "bg-[#EDF3FF]",
    icon: "text-[#2448A5]",
  },

  green: {
    iconContainer: "bg-[#D8F1E7]",
    icon: "text-[#207A5B]",
  },

  yellow: {
    iconContainer: "bg-[#FFF5D6]",
    icon: "text-[#9A7515]",
  },

  pink: {
    iconContainer: "bg-[#F5DCE7]",
    icon: "text-[#9D5274]",
  },
};

export function AdminStatsCard({
  title,
  value,
  description,
  icon: Icon,
  variant = "blue",
}: AdminStatsCardProps) {
  const styles = variantClasses[variant];

  return (
    <article
      className="
        rounded-2xl
        border
        border-slate-100
        bg-white
        p-5
        shadow-[0_10px_35px_rgba(31,53,100,0.04)]
        transition

        hover:-translate-y-0.5
        hover:shadow-[0_14px_40px_rgba(31,53,100,0.08)]
      "
    >
      <div
        className="
          flex
          items-start
          justify-between
          gap-4
        "
      >
        <div
          className={[
            `
              flex
              size-11
              shrink-0
              items-center
              justify-center
              rounded-xl
            `,
            styles.iconContainer,
          ].join(" ")}
        >
          <Icon size={21} className={styles.icon} />
        </div>
      </div>

      <p
        className="
          mt-5
          text-sm
          font-medium
          text-slate-500
        "
      >
        {title}
      </p>

      <p
        className="
          mt-2
          text-3xl
          font-bold
          tracking-tight
          text-[#18234A]
        "
      >
        {value}
      </p>

      <p
        className="
          mt-2
          text-xs
          leading-5
          text-slate-400
        "
      >
        {description}
      </p>
    </article>
  );
}
