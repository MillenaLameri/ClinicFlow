import { LoginForm } from "./components/LoginForm";
import { LoginHero } from "./components/LoginHero";

export function LoginPage() {
  return (
    <main
      className="
        min-h-screen
        bg-[#F5F8FC]
        p-4
        md:p-6
        lg:p-8
      "
    >
      <div
        className="
          mx-auto
          grid
          min-h-[calc(100vh-2rem)]
          max-w-[1440px]
          overflow-hidden
          rounded-[32px]
          bg-white
          shadow-[0_24px_80px_rgba(31,53,100,0.08)]
          md:min-h-[calc(100vh-3rem)]
          lg:grid-cols-[0.9fr_1.1fr]
        "
      >
        <LoginForm />
        <LoginHero />
      </div>
    </main>
  );
}
