using ClinicFlow.Api.Contracts.AvailableSlots;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Services.Scheduling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route(
    "api/doctors/{doctorId:guid}/available-slots"
)]
public sealed class AvailableSlotsController : ControllerBase
{
    private readonly ClinicFlowDbContext _dbContext;

    private readonly AvailableSlotsService
        _availableSlotsService;

    public AvailableSlotsController(
        ClinicFlowDbContext dbContext,
        AvailableSlotsService availableSlotsService
    )
    {
        _dbContext = dbContext;
        _availableSlotsService = availableSlotsService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<AvailableSlotResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<
        ActionResult<IReadOnlyCollection<AvailableSlotResponse>>
    > GetAvailableSlots(
        Guid doctorId,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken
    )
    {
        if (date is null)
        {
            ModelState.AddModelError(
                nameof(date),
                "A data da consulta é obrigatória."
            );

            return ValidationProblem(ModelState);
        }

        var doctorExists = await _dbContext.Doctors
            .AsNoTracking()
            .AnyAsync(
                doctor =>
                    doctor.Id == doctorId
                    && doctor.IsActive,
                cancellationToken
            );

        if (!doctorExists)
        {
            return Problem(
                statusCode:
                    StatusCodes.Status404NotFound,
                title: "Médico ativo não encontrado.",
                detail:
                    $"Não foi encontrado um médico ativo com o ID '{doctorId}'."
            );
        }

        var generatedSlots =
            await _availableSlotsService.GenerateAsync(
                doctorId,
                date.Value,
                cancellationToken
            );

        var response = generatedSlots
            .Select(slot =>
                new AvailableSlotResponse(
                    slot.AvailabilityId,
                    slot.Date,
                    slot.DayOfWeek,
                    slot.StartTime,
                    slot.EndTime,
                    slot.DurationMinutes
                )
            )
            .ToList();

        return Ok(response);
    }
}