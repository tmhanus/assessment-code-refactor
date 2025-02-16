namespace Refactoring.Domain.Interfaces;

public interface IEmailClient
{
    Task SendEmailAsync(string url, string email, string message, CancellationToken cancellationToken);
}