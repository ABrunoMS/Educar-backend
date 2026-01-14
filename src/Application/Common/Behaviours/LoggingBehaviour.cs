using Educar.Backend.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Educar.Backend.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger, IUser user) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger = logger;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = user.Id?.ToString() ?? string.Empty;
        var userName = string.Empty;

        _logger.LogInformation("Educar.Backend Request: {Name} {@UserId} {@Request}",
            requestName, userId, request);

        return Task.CompletedTask;
    }
}