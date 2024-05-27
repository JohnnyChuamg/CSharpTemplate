using Infrastructure.Mvc.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkLog.Api.Controllers.Users.Models;
using WorkLog.Application.Services.Users;
using WorkLog.Infrastructure.Enums;

namespace WorkLog.Api.Controllers.Users;

[ApiController]
[Area("User")]
[Route("api/[area]")]
public class UsersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> QueryAsync(QueryUserRequest request)
    {
        var result = await mediator.Send(request.ParseToQueryUsersQuery());
        return result.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        var result = await mediator.Send(new GetUserDetail { Id = id });
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateUser userRequest)
    {
        var result = await mediator.Send(userRequest);
        return result.ToActionResult();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> PutAsync([FromRoute] long id, UpdateUserRequest request)
    {
        var result = await mediator.Send(new UpdateUser { Id = id });
        return result.ToActionResult();
    }

    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> SetStatusAsync([FromRoute] long id, Status status)
    {
        var result = await mediator.Send(new UpdateUserStatus { Id = id });
        return result.ToActionResult();
    }
}