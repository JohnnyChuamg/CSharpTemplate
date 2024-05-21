using System.Linq.Expressions;
using WorkLog.Application.Services.Users;
using WorkLog.Infrastructure.Enums;

namespace WorkLog.Api.Controllers.Users.Models;

public class QueryUserRequest
{
    public string? Username { get; set; }
    public string? Name { get; set; }
    public long? DepartmentId { get; set; }

    public QueryUsers ParseToQueryUsersQuery()
    {
        Expression<Func<Domain.Entities.User, bool>> predicate = f => f.Status == Status.Enable;

        if (!string.IsNullOrWhiteSpace(Username))
        {
            predicate = predicate.And(p => p.Username == Username);
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            predicate = predicate.And(p => p.Name == Name);
        }

        if (DepartmentId.HasValue)
        {
            predicate = predicate.And(p => p.DepartmentId == DepartmentId);
        }

        return new QueryUsers
        {
            Predicate = predicate
        };
    }
}