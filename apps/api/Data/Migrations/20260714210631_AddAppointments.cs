using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicFlow.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_availability_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.id);
                    table.CheckConstraint("ck_appointments_status", "status BETWEEN 1 AND 4");
                    table.CheckConstraint("ck_appointments_time_range", "start_time < end_time");
                    table.ForeignKey(
                        name: "FK_appointments_doctor_availabilities_doctor_availability_id",
                        column: x => x.doctor_availability_id,
                        principalTable: "doctor_availabilities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_doctors_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_doctor_availability_id",
                table: "appointments",
                column: "doctor_availability_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_doctor_date_status",
                table: "appointments",
                columns: new[] { "doctor_id", "appointment_date", "status" });

            migrationBuilder.CreateIndex(
                name: "ux_appointments_doctor_date_start_scheduled",
                table: "appointments",
                columns: new[] { "doctor_id", "appointment_date", "start_time" },
                unique: true,
                filter: "status = 1");

            migrationBuilder.CreateIndex(
                name: "ux_appointments_patient_date_start_scheduled",
                table: "appointments",
                columns: new[] { "patient_id", "appointment_date", "start_time" },
                unique: true,
                filter: "status = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");
        }
    }
}
