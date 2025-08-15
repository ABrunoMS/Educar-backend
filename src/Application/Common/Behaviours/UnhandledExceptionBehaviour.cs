using Educar.Backend.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;

namespace Educar.Backend.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ValidationException)
        {
            // Do not log ValidationException
            throw;
        }
        catch (Educar.Backend.Application.Common.Exceptions.NotFoundException)
        {
            // Do not log NotFoundException
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            // Do not log UnauthorizedAccessException
            throw;
        }
        catch (ForbiddenAccessException)
        {
            // Do not log ForbiddenAccessException
            throw;
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "Educar.Backend Request: Unhandled Exception for Request {Name} {@Request}",
                requestName, request);

            throw;
        }
    }
}