type DoctorStatusBadgeProps = {
  isActive: boolean;
};

export function DoctorStatusBadge({ isActive }: DoctorStatusBadgeProps) {
  return (
    <span
      className={[
        `
          inline-flex
          items-center
          gap-2
          rounded-full
          px-2.5
          py-1
          text-xs
          font-semibold
        `,
        isActive
          ? "bg-emerald-50 text-emerald-700"
          : "bg-slate-100 text-slate-500",
      ].join(" ")}
    >
      <span
        className={[
          "size-1.5 rounded-full",
          isActive ? "bg-emerald-500" : "bg-slate-400",
        ].join(" ")}
      />

      {isActive ? "Ativo" : "Inativo"}
    </span>
  );
}
