namespace ClinicFlow.Api.Contracts.Dashboard;

public sealed record AdminDashboardResponse(
    int TotalDoctors,
    int TotalPatients,
    int AppointmentsToday,
    int ScheduledAppointments,
    int CompletedToday,
    int NoShowToday
);