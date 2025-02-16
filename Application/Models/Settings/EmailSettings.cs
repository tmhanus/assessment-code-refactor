namespace Refactoring.Application.Models.Settings;

public record EmailSettings
{
    public required string OutgoingUrl { get; init; }
}