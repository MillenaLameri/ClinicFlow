using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllowRecreatingInactiveAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_doctor_availabilities_doctor_id_day_of_week_start_time_end_~",
                table: "doctor_availabilities");

            migrationBuilder.CreateIndex(
                name: "IX_doctor_availabilities_doctor_id_day_of_week_start_time_end_~",
                table: "doctor_availabilities",
                columns: new[] { "doctor_id", "day_of_week", "start_time", "end_time" },
                unique: true,
                filter: "is_active = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_doctor_availabilities_doctor_id_day_of_week_start_time_end_~",
                table: "doctor_availabilities");

            migrationBuilder.CreateIndex(
                name: "IX_doctor_availabilities_doctor_id_day_of_week_start_time_end_~",
                table: "doctor_availabilities",
                columns: new[] { "doctor_id", "day_of_week", "start_time", "end_time" },
                unique: true);
        }
    }
}
