using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "doctors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    crm_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    crm_state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    specialty_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctors", x => x.id);
                    table.ForeignKey(
                        name: "FK_doctors_specialties_specialty_id",
                        column: x => x.specialty_id,
                        principalTable: "specialties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctors_crm_number_crm_state",
                table: "doctors",
                columns: new[] { "crm_number", "crm_state" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctors_email",
                table: "doctors",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctors_specialty_id",
                table: "doctors",
                column: "specialty_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doctors");
        }
    }
}
