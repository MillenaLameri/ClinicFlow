import { HeartPulse } from "lucide-react";

type ClinicFlowLogoProps = {
  light?: boolean;
};

export function ClinicFlowLogo({ light = false }: ClinicFlowLogoProps) {
  return (
    <div className="flex items-center gap-3">
      <div
        className={[
          "flex size-10 items-center justify-center rounded-xl",
          light ? "bg-white/15 text-white" : "bg-[#2448A5] text-white",
        ].join(" ")}
      >
        <HeartPulse size={22} />
      </div>

      <span
        className={[
          "text-xl font-bold tracking-tight",
          light ? "text-white" : "text-[#18234A]",
        ].join(" ")}
      >
        ClinicFlow
      </span>
    </div>
  );
}
