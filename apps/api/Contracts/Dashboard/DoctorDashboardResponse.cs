namespace ClinicFlow.Api.Contracts.Dashboard;

public sealed record DoctorDashboardResponse(
    Guid DoctorId,
    int AppointmentsToday,
    int ScheduledToday,
    int CompletedToday,
    int NoShowToday,
    int UpcomingAppointments,
    int TotalPatients
);