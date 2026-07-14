using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Contracts.Specialties;

public sealed record SpecialtyResponse(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc
)
{
    public static SpecialtyResponse FromEntity(Specialty specialty)
    {
        return new SpecialtyResponse(
            specialty.Id,
            specialty.Name,
            specialty.IsActive,
            specialty.CreatedAtUtc
        );
    }
}