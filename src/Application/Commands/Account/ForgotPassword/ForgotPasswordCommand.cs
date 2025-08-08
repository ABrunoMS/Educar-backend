using Educar.Backend.Application.Common.Interfaces;

namespace Educar.Backend.Application.Commands.Account.ResetPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Unit>;

public class ForgotPasswordCommandHandler(IIdentityService identityService)
    : IRequestHandler<ForgotPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var success = await identityService.TriggerPasswordReset(request.Email, cancellationToken);
        if (!success) throw new Exception("Error during reset password email trigger");

        return Unit.Value;
    }
}