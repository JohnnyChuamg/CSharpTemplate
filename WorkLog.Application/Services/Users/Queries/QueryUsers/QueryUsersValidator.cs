using FluentValidation;

namespace WorkLog.Application.Services.Users;

public class QueryUsersValidator : AbstractValidator<QueryUsers>
{
    public QueryUsersValidator()
    {
        RuleFor(r => r.Predicate).NotNull().NotEmpty();
    }
}