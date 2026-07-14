using System.Text.Json.Serialization;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using ClinicFlow.Api.Services.Scheduling;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder =
    WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options
            .JsonSerializerOptions
            .Converters
            .Add(
                new JsonStringEnumConverter()
            );
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection"
    )
    ?? throw new InvalidOperationException(
        "A connection string 'DefaultConnection' não foi configurada."
    );

builder.Services.AddDbContext<ClinicFlowDbContext>(
    options =>
    {
        options.UseNpgsql(connectionString);
    }
);

builder.Services
    .AddDataProtection()
    .SetApplicationName("ClinicFlow");

builder.Services
    .AddIdentityCore<ApplicationUser>(
        options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 4;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(15);
        }
    )
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ClinicFlowDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

builder.Services.AddScoped<AvailableSlotsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();