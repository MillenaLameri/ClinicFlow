using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patients_cpf",
                table: "patients",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patients_email",
                table: "patients",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patients");
        }
    }
}
