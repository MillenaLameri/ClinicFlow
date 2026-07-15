using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Contracts.Dashboard;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using ClinicFlow.Api.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController
    : ControllerBase
{
    private readonly ClinicFlowDbContext
        _dbContext;

    private readonly CurrentUserService
        _currentUser;

    public DashboardController(
        ClinicFlowDbContext dbContext,
        CurrentUserService currentUser
    )
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpGet("admin")]
    [Authorize(
        Policy =
            AuthorizationPolicies.AdminOnly
    )]
    [ProducesResponseType(
        typeof(AdminDashboardResponse),
        StatusCodes.Status200OK
    )]
    public async Task<
        ActionResult<AdminDashboardResponse>
    > GetAdminDashboard(
        CancellationToken cancellationToken
    )
    {
        var today =
            DateOnly.FromDateTime(
                DateTime.Today
            );

        var totalDoctors =
            await _dbContext.Doctors
                .AsNoTracking()
                .CountAsync(
                    doctor =>
                        doctor.IsActive,
                    cancellationToken
                );

        var totalPatients =
            await _dbContext.Patients
                .AsNoTracking()
                .CountAsync(
                    patient =>
                        patient.IsActive,
                    cancellationToken
                );

        var appointmentsToday =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment
                            .AppointmentDate
                            == today,
                    cancellationToken
                );

        var scheduledAppointments =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment.Status
                        == AppointmentStatus
                            .Scheduled
                        && appointment
                            .AppointmentDate
                            >= today,
                    cancellationToken
                );

        var completedToday =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment
                            .AppointmentDate
                            == today
                        && appointment.Status
                            == AppointmentStatus
                                .Completed,
                    cancellationToken
                );

        var noShowToday =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment
                            .AppointmentDate
                            == today
                        && appointment.Status
                            == AppointmentStatus
                                .NoShow,
                    cancellationToken
                );

        return Ok(
            new AdminDashboardResponse(
                totalDoctors,
                totalPatients,
                appointmentsToday,
                scheduledAppointments,
                completedToday,
                noShowToday
            )
        );
    }

    [HttpGet("doctor")]
    [Authorize(
        Policy =
            AuthorizationPolicies.DoctorOnly
    )]
    [ProducesResponseType(
        typeof(DoctorDashboardResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    public async Task<
        ActionResult<DoctorDashboardResponse>
    > GetDoctorDashboard(
        CancellationToken cancellationToken
    )
    {
        if (
            _currentUser.DoctorId
                is not Guid doctorId
        )
        {
            return Forbid();
        }

        var today =
            DateOnly.FromDateTime(
                DateTime.Today
            );

        var appointmentsToday =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment.DoctorId
                            == doctorId
                        && appointment
                            .AppointmentDate
                            == today,
                    cancellationToken
                );

        var scheduledToday =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment.DoctorId
                            == doctorId
                        && appointment
                            .AppointmentDate
                            == today
                        && appointment.Status
                            == AppointmentStatus
                                .Scheduled,
                    cancellationToken
                );

        var completedToday =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment.DoctorId
                            == doctorId
                        && appointment
                            .AppointmentDate
                            == today
                        && appointment.Status
                            == AppointmentStatus
                                .Completed,
                    cancellationToken
                );

        var noShowToday =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment.DoctorId
                            == doctorId
                        && appointment
                            .AppointmentDate
                            == today
                        && appointment.Status
                            == AppointmentStatus
                                .NoShow,
                    cancellationToken
                );

        var upcomingAppointments =
            await _dbContext.Appointments
                .AsNoTracking()
                .CountAsync(
                    appointment =>
                        appointment.DoctorId
                            == doctorId
                        && appointment
                            .AppointmentDate
                            >= today
                        && appointment.Status
                            == AppointmentStatus
                                .Scheduled,
                    cancellationToken
                );

        var totalPatients =
            await _dbContext.Appointments
                .AsNoTracking()
                .Where(
                    appointment =>
                        appointment.DoctorId
                        == doctorId
                )
                .Select(
                    appointment =>
                        appointment.PatientId
                )
                .Distinct()
                .CountAsync(
                    cancellationToken
                );

        return Ok(
            new DoctorDashboardResponse(
                doctorId,
                appointmentsToday,
                scheduledToday,
                completedToday,
                noShowToday,
                upcomingAppointments,
                totalPatients
            )
        );
    }
}