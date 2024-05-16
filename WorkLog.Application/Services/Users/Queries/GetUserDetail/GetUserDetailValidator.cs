using FluentValidation;

namespace WorkLog.Application.Services.Users;

public class GetUserDetailValidator : AbstractValidator<GetUserDetail>
{
    public GetUserDetailValidator()
    {
        RuleFor(r => r.Id).NotNull().NotEmpty();
    }
}