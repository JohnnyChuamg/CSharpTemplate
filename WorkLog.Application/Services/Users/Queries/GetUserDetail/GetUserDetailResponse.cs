namespace WorkLog.Application.Services.Users;

public class GetUserDetailResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Username { get; set; }

    public static GetUserDetailResponse FromUser(Domain.Entities.User user)
        => new GetUserDetailResponse
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.Username
        };
}