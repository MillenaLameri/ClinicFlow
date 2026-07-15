import { LoginForm } from "./components/LoginForm";
import { LoginHero } from "./components/LoginHero";

export function LoginPage() {
  return (
    <main
      className="
        h-dvh
        w-full
        overflow-hidden
        bg-[#F5F8FC]

        sm:p-4
        md:p-6
        lg:p-8
      "
    >
      <div
        className="
          mx-auto
          grid
          h-full
          w-full
          max-w-[1440px]
          overflow-hidden
          bg-white

          sm:rounded-[28px]
          sm:shadow-[0_24px_80px_rgba(31,53,100,0.08)]

          lg:grid-cols-[0.9fr_1.1fr]
          lg:rounded-[32px]
        "
      >
        <LoginForm />

        <LoginHero />
      </div>
    </main>
  );
}
