using Microsoft.AspNetCore.Mvc;

namespace WorkLog.Api.Controllers.Users.Models;

public class GetUserDetailRequest
{
    [FromRoute]
    public long Id { get; set; }
}