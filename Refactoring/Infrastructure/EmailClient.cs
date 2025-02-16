using Refactoring.Domain.Interfaces;

namespace Refactoring.Infrastructure;

internal class HttpEmailClient(
    IHttpClientFactory httpClientFactory, 
    ILogger<HttpEmailClient> logger, 
    IOutputWriter outputWriter) : IEmailClient
{
    public async Task SendEmailAsync(string url, string email, string message, CancellationToken cancellationToken)
    {
        using var client = httpClientFactory.CreateClient();
        try
        {
            var response = await client
                .GetStringAsync($"{url}?to={email}&message={message}", cancellationToken);
            
            outputWriter.Write(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email} at {EmailEndpointUrl}", email, url); // TO be redacted
        }
    }
}