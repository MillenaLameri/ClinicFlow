using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorAvailabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "doctor_availabilities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    slot_duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_availabilities", x => x.id);
                    table.CheckConstraint("ck_doctor_availabilities_day_of_week", "day_of_week BETWEEN 1 AND 7");
                    table.CheckConstraint("ck_doctor_availabilities_slot_duration", "slot_duration_minutes BETWEEN 10 AND 240");
                    table.CheckConstraint("ck_doctor_availabilities_time_range", "start_time < end_time");
                    table.ForeignKey(
                        name: "FK_doctor_availabilities_doctors_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctor_availabilities_doctor_id_day_of_week_start_time_end_~",
                table: "doctor_availabilities",
                columns: new[] { "doctor_id", "day_of_week", "start_time", "end_time" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doctor_availabilities");
        }
    }
}
