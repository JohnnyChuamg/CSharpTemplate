using FluentValidation;

namespace WorkLog.Application.Services.Users;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(r => r.Username).NotNull().NotEmpty();
        RuleFor(r => r.Name).NotNull().NotEmpty();
        RuleFor(r => r.Password).NotNull().NotEmpty();
        RuleFor(r => r.Email).NotNull().NotEmpty();
        RuleFor(r => r.RoleId).GreaterThan(0);
        RuleFor(r => r.DepartmentId).GreaterThan(0);
        RuleFor(r => r.Status).IsInEnum();
    }
}