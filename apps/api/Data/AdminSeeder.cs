using ClinicFlow.Api.Configuration;
using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Api.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services
    )
    {
        await using var scope =
            services.CreateAsyncScope();

        var options =
            scope.ServiceProvider
                .GetRequiredService<
                    IOptions<AdminSeedOptions>
                >()
                .Value;

        if (!options.Enabled)
        {
            return;
        }

        if (
            string.IsNullOrWhiteSpace(
                options.FullName
            )
        )
        {
            throw new InvalidOperationException(
                "O nome do administrador inicial não foi configurado."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                options.Email
            )
        )
        {
            throw new InvalidOperationException(
                "O e-mail do administrador inicial não foi configurado."
            );
        }

        if (
            string.IsNullOrWhiteSpace(
                options.Password
            )
        )
        {
            throw new InvalidOperationException(
                "A senha do administrador inicial não foi configurada."
            );
        }

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<
                    UserManager<ApplicationUser>
                >();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<
                    RoleManager<
                        IdentityRole<Guid>
                    >
                >();

        var adminRoleExists =
            await roleManager.RoleExistsAsync(
                UserRoleNames.Admin
            );

        if (!adminRoleExists)
        {
            throw new InvalidOperationException(
                "A role Admin não existe no banco de dados. Execute as migrations antes de iniciar a aplicação."
            );
        }

        var normalizedEmail =
            options.Email
                .Trim()
                .ToLowerInvariant();

        var existingUser =
            await userManager.FindByEmailAsync(
                normalizedEmail
            );

        if (existingUser is not null)
        {
            var alreadyAdmin =
                await userManager.IsInRoleAsync(
                    existingUser,
                    UserRoleNames.Admin
                );

            if (!alreadyAdmin)
            {
                var addRoleResult =
                    await userManager.AddToRoleAsync(
                        existingUser,
                        UserRoleNames.Admin
                    );

                if (!addRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        BuildErrorMessage(
                            "Não foi possível adicionar a role Admin ao usuário existente.",
                            addRoleResult
                        )
                    );
                }
            }

            return;
        }

        var admin = new ApplicationUser(
            options.FullName,
            normalizedEmail
        )
        {
            EmailConfirmed = true
        };

        var createResult =
            await userManager.CreateAsync(
                admin,
                options.Password
            );

        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                BuildErrorMessage(
                    "Não foi possível criar o administrador inicial.",
                    createResult
                )
            );
        }

        var roleResult =
            await userManager.AddToRoleAsync(
                admin,
                UserRoleNames.Admin
            );

        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(admin);

            throw new InvalidOperationException(
                BuildErrorMessage(
                    "O administrador foi criado, mas não foi possível adicionar a role Admin.",
                    roleResult
                )
            );
        }
    }

    private static string BuildErrorMessage(
        string message,
        IdentityResult result
    )
    {
        var errors = string.Join(
            "; ",
            result.Errors.Select(
                error => error.Description
            )
        );

        return $"{message} {errors}";
    }
}