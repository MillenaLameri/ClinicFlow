import { ArrowRight, Eye, EyeOff, LoaderCircle } from "lucide-react";

import { useState } from "react";
import { useNavigate } from "react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";

import { ClinicFlowLogo } from "@/features/auth/components/ClinicFlowLogo";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { useLogin } from "@/features/auth/hooks/useLogin";

import {
  loginSchema,
  type LoginFormData,
} from "@/features/auth/schemas/loginSchema";

export function LoginForm() {
  const [showPassword, setShowPassword] = useState(false);

  const navigate = useNavigate();

  const loginMutation = useLogin();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),

    defaultValues: {
      email: "",
      password: "",
    },
  });

  function handleTogglePassword() {
    setShowPassword((current) => !current);
  }

  async function onSubmit(data: LoginFormData) {
    try {
      await loginMutation.mutateAsync(data);

      navigate("/dashboard", {
        replace: true,
      });
    } catch {
      // O erro é exibido através
      // do estado da mutation.
    }
  }

  function getLoginError(): string | null {
    if (!loginMutation.error) {
      return null;
    }

    if (axios.isAxiosError(loginMutation.error)) {
      if (loginMutation.error.response?.status === 401) {
        return "E-mail ou senha inválidos.";
      }

      if (loginMutation.error.response?.status === 403) {
        return "Sua conta não possui permissão para acessar o sistema.";
      }

      const detail = loginMutation.error.response?.data?.detail;

      if (detail) {
        return String(detail);
      }
    }

    return "Não foi possível entrar. Tente novamente.";
  }

  const loginError = getLoginError();

  return (
    <section
      className="
        flex
        h-full
        min-h-0
        w-full
        flex-col
        overflow-y-auto

        px-5
        pb-5
        pt-8

        sm:px-8
        sm:pb-6
        sm:pt-8

        md:px-10

        lg:px-12
        lg:pb-8
        lg:pt-8

        xl:px-20
      "
    >
      <header
        className="
          relative
          z-10
          mx-auto
          flex
          w-full
          max-w-[430px]
          shrink-0
          items-center
          pb-5
        "
      >
        <ClinicFlowLogo />
      </header>

      <div
        className="
          mx-auto
          flex
          w-full
          max-w-[430px]
          flex-1
          flex-col
          justify-center

          py-3

          sm:py-4

          lg:py-5
        "
      >
        <header
          className="
            mb-7

            sm:mb-9
          "
        >
          <span
            className="
              mb-3
              inline-flex
              rounded-full
              bg-[#EDF3FF]
              px-3
              py-1.5
              text-xs
              font-semibold
              text-[#2448A5]

              sm:mb-4
            "
          >
            Bem-vinda de volta
          </span>

          <h1
            className="
              text-2xl
              font-bold
              tracking-tight
              text-[#18234A]

              sm:text-3xl

              lg:text-4xl
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

        <form
          className="space-y-5"
          onSubmit={handleSubmit(onSubmit)}
          noValidate
        >
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
              type="email"
              inputMode="email"
              autoComplete="email"
              autoCapitalize="none"
              placeholder="seuemail@exemplo.com"
              aria-invalid={Boolean(errors.email)}
              {...register("email")}
              className="
                h-12
                rounded-xl
                border-slate-200
                bg-[#F9FAFC]
                px-4
                text-base
                shadow-none
                transition
                placeholder:text-slate-400

                focus-visible:border-[#2448A5]
                focus-visible:ring-[#2448A5]/15

                sm:text-sm
              "
            />

            {errors.email && (
              <p
                role="alert"
                className="
                  text-xs
                  font-medium
                  text-red-500
                "
              >
                {errors.email.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <div
              className="
                flex
                items-center
                justify-between
                gap-4
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
                  shrink-0
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
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                placeholder="Digite sua senha"
                aria-invalid={Boolean(errors.password)}
                {...register("password")}
                className="
                  h-12
                  rounded-xl
                  border-slate-200
                  bg-[#F9FAFC]
                  px-4
                  pr-12
                  text-base
                  shadow-none
                  transition
                  placeholder:text-slate-400

                  focus-visible:border-[#2448A5]
                  focus-visible:ring-[#2448A5]/15

                  sm:text-sm
                "
              />

              <button
                type="button"
                onClick={handleTogglePassword}
                className="
                  absolute
                  right-3
                  top-1/2
                  flex
                  size-9
                  -translate-y-1/2
                  items-center
                  justify-center
                  rounded-md
                  text-slate-400
                  transition

                  hover:bg-slate-100
                  hover:text-[#2448A5]
                "
                aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"}
              >
                {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
              </button>
            </div>

            {errors.password && (
              <p
                role="alert"
                className="
                  text-xs
                  font-medium
                  text-red-500
                "
              >
                {errors.password.message}
              </p>
            )}
          </div>

          {loginError && (
            <div
              role="alert"
              aria-live="polite"
              className="
                rounded-xl
                border
                border-red-100
                bg-red-50
                px-4
                py-3
                text-sm
                leading-5
                text-red-600
              "
            >
              {loginError}
            </div>
          )}

          <Button
            type="submit"
            disabled={loginMutation.isPending}
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

              disabled:cursor-not-allowed
              disabled:opacity-70
            "
          >
            {loginMutation.isPending ? (
              <>
                <LoaderCircle
                  size={17}
                  className="
                    animate-spin
                  "
                />
                Entrando...
              </>
            ) : (
              <>
                Entrar
                <ArrowRight
                  size={17}
                  className="
                    ml-1
                    transition-transform

                    group-hover:translate-x-1
                  "
                />
              </>
            )}
          </Button>
        </form>

        <div
          className="
            mt-7
            flex
            items-center
            gap-3

            sm:mt-8
            sm:gap-4
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
              shrink-0
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
            mt-5
            text-center
            text-sm
            leading-6
            text-slate-500

            sm:mt-6
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
          mx-auto
          w-full
          max-w-[430px]
          shrink-0
          pb-1
          pt-3
          text-center
          text-[11px]
          leading-5
          text-slate-400

          sm:text-xs

          lg:text-left
        "
      >
        © 2026 ClinicFlow. Gestão de saúde simplificada.
      </footer>
    </section>
  );
}
