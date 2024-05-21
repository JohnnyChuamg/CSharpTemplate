namespace WorkLog.Application.Services.Users;

public class QueryUsersResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Username { get; set; }
    public int DepartmentId { get; set; }
    public int RoleId { get; set; }

    public static QueryUsersResponse FromUser(Domain.Entities.User source)
    {
        return new QueryUsersResponse
        {
            Id = source.Id,
            Name = source.Name,
            Username = source.Username
            //DepartmentId = source.DepartmentId,
            //RoleId = source.RoleId
        };
    }
}