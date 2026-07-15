import { ArrowRight, Eye, EyeOff } from "lucide-react";

import { useState } from "react";

import { ClinicFlowLogo } from "@/components/ClinicFlowLogo";

import { Button } from "@/components/ui/button";

import { Input } from "@/components/ui/input";

import { Label } from "@/components/ui/label";

export function LoginForm() {
  const [showPassword, setShowPassword] = useState(false);

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
  }

  function handleTogglePassword() {
    setShowPassword((current) => !current);
  }

  return (
    <section
      className="
        flex
        flex-col
        px-6
        py-8
        sm:px-10
        lg:px-16
        xl:px-24
      "
    >
      <ClinicFlowLogo />

      <div
        className="
          mx-auto
          flex
          w-full
          max-w-[430px]
          flex-1
          flex-col
          justify-center
          py-12
        "
      >
        <header className="mb-9">
          <span
            className="
              mb-4
              inline-flex
              rounded-full
              bg-[#EDF3FF]
              px-3
              py-1.5
              text-xs
              font-semibold
              text-[#2448A5]
            "
          >
            Bem-vinda de volta
          </span>

          <h1
            className="
              text-3xl
              font-bold
              tracking-tight
              text-[#18234A]
              sm:text-4xl
            "
          >
            Entre na sua conta
          </h1>

          <p
            className="
              mt-3
              max-w-sm
              text-sm
              leading-6
              text-slate-500
            "
          >
            Acesse sua agenda, consultas e informações do ClinicFlow em um único
            lugar.
          </p>
        </header>

        <form className="space-y-5" onSubmit={handleSubmit}>
          <div className="space-y-2">
            <Label
              htmlFor="email"
              className="
                text-sm
                font-medium
                text-[#18234A]
              "
            >
              E-mail
            </Label>

            <Input
              id="email"
              name="email"
              type="email"
              autoComplete="email"
              placeholder="seuemail@exemplo.com"
              className="
                h-12
                rounded-xl
                border-slate-200
                bg-[#F9FAFC]
                px-4
                shadow-none
                transition
                placeholder:text-slate-400
                focus-visible:border-[#2448A5]
                focus-visible:ring-[#2448A5]/15
              "
            />
          </div>

          <div className="space-y-2">
            <div
              className="
                flex
                items-center
                justify-between
              "
            >
              <Label
                htmlFor="password"
                className="
                  text-sm
                  font-medium
                  text-[#18234A]
                "
              >
                Senha
              </Label>

              <button
                type="button"
                className="
                  text-xs
                  font-medium
                  text-[#2448A5]
                  transition
                  hover:text-[#172D6B]
                "
              >
                Esqueceu sua senha?
              </button>
            </div>

            <div className="relative">
              <Input
                id="password"
                name="password"
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                placeholder="Digite sua senha"
                className="
                  h-12
                  rounded-xl
                  border-slate-200
                  bg-[#F9FAFC]
                  px-4
                  pr-12
                  shadow-none
                  transition
                  placeholder:text-slate-400
                  focus-visible:border-[#2448A5]
                  focus-visible:ring-[#2448A5]/15
                "
              />

              <button
                type="button"
                onClick={handleTogglePassword}
                className="
                  absolute
                  right-4
                  top-1/2
                  -translate-y-1/2
                  text-slate-400
                  transition
                  hover:text-[#2448A5]
                "
                aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"}
              >
                {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
              </button>
            </div>
          </div>

          <Button
            type="submit"
            className="
              group
              mt-2
              h-12
              w-full
              rounded-xl
              bg-[#2448A5]
              font-semibold
              text-white
              shadow-none
              transition-all
              hover:bg-[#172D6B]
            "
          >
            Entrar
            <ArrowRight
              size={17}
              className="
                ml-1
                transition-transform
                group-hover:translate-x-1
              "
            />
          </Button>
        </form>

        <div
          className="
            mt-8
            flex
            items-center
            gap-4
          "
        >
          <div
            className="
              h-px
              flex-1
              bg-slate-100
            "
          />

          <span
            className="
              text-xs
              text-slate-400
            "
          >
            Primeiro acesso?
          </span>

          <div
            className="
              h-px
              flex-1
              bg-slate-100
            "
          />
        </div>

        <p
          className="
            mt-6
            text-center
            text-sm
            text-slate-500
          "
        >
          Ainda não possui uma conta?{" "}
          <button
            type="button"
            className="
              font-semibold
              text-[#2448A5]
              transition
              hover:text-[#172D6B]
            "
          >
            Criar conta
          </button>
        </p>
      </div>

      <footer
        className="
          text-center
          text-xs
          text-slate-400
          lg:text-left
        "
      >
        © 2026 ClinicFlow. Gestão de saúde simplificada.
      </footer>
    </section>
  );
}
