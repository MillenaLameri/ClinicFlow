using ClinicFlow.Api.Contracts.Specialties;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/specialties")]
public sealed class SpecialtiesController : ControllerBase
{
    private readonly ClinicFlowDbContext _dbContext;

    public SpecialtiesController(ClinicFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<SpecialtyResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<IReadOnlyCollection<SpecialtyResponse>>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbContext.Specialties
            .AsNoTracking()
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(specialty => specialty.IsActive);
        }

        var specialties = await query
            .OrderBy(specialty => specialty.Name)
            .Select(specialty => new SpecialtyResponse(
                specialty.Id,
                specialty.Name,
                specialty.IsActive,
                specialty.CreatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        return Ok(specialties);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(SpecialtyResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecialtyResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var specialty = await _dbContext.Specialties
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken
            );

        if (specialty is null)
        {
            return SpecialtyNotFound(id);
        }

        return Ok(SpecialtyResponse.FromEntity(specialty));
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(SpecialtyResponse),
        StatusCodes.Status201Created
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SpecialtyResponse>> Create(
        [FromBody] CreateSpecialtyRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError(
                nameof(request.Name),
                "O nome da especialidade é obrigatório."
            );

            return ValidationProblem(ModelState);
        }

        var name = request.Name.Trim();

        var nameAlreadyExists = await SpecialtyNameExistsAsync(
            name,
            ignoredSpecialtyId: null,
            cancellationToken
        );

        if (nameAlreadyExists)
        {
            return SpecialtyNameConflict(name);
        }

        var specialty = new Specialty(name);

        _dbContext.Specialties.Add(specialty);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsUniqueConstraintViolation(exception))
        {
            return SpecialtyNameConflict(name);
        }

        var response = SpecialtyResponse.FromEntity(specialty);

        return CreatedAtAction(
            nameof(GetById),
            new { id = specialty.Id },
            response
        );
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(SpecialtyResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SpecialtyResponse>> Update(
        Guid id,
        [FromBody] UpdateSpecialtyRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError(
                nameof(request.Name),
                "O nome da especialidade é obrigatório."
            );

            return ValidationProblem(ModelState);
        }

        var specialty = await _dbContext.Specialties
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken
            );

        if (specialty is null)
        {
            return SpecialtyNotFound(id);
        }

        var name = request.Name.Trim();

        var nameAlreadyExists = await SpecialtyNameExistsAsync(
            name,
            ignoredSpecialtyId: id,
            cancellationToken
        );

        if (nameAlreadyExists)
        {
            return SpecialtyNameConflict(name);
        }

        specialty.UpdateName(name);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (IsUniqueConstraintViolation(exception))
        {
            return SpecialtyNameConflict(name);
        }

        return Ok(SpecialtyResponse.FromEntity(specialty));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var specialty = await _dbContext.Specialties
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken
            );

        if (specialty is null)
        {
            return SpecialtyNotFound(id);
        }

        if (!specialty.IsActive)
        {
            return NoContent();
        }

        specialty.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private async Task<bool> SpecialtyNameExistsAsync(
        string name,
        Guid? ignoredSpecialtyId,
        CancellationToken cancellationToken
    )
    {
        var normalizedName = name.ToLower();

        return await _dbContext.Specialties.AnyAsync(
            specialty =>
                specialty.Name.ToLower() == normalizedName
                && (
                    ignoredSpecialtyId == null
                    || specialty.Id != ignoredSpecialtyId.Value
                ),
            cancellationToken
        );
    }

    private ObjectResult SpecialtyNotFound(Guid id)
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Especialidade não encontrada.",
            detail: $"Não foi encontrada uma especialidade com o ID '{id}'."
        );
    }

    private ObjectResult SpecialtyNameConflict(string name)
    {
        return Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Especialidade já cadastrada.",
            detail: $"Já existe uma especialidade com o nome '{name}'."
        );
    }

    private static bool IsUniqueConstraintViolation(
        DbUpdateException exception
    )
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }
}