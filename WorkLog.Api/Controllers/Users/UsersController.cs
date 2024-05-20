using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkLog.Api.Controllers.Users.Models;
using WorkLog.Application.Services.Users;
using WorkLog.Infrastructure.Enums;

namespace WorkLog.Api.Controllers.Users;

[ApiController]
[Area("User")]
[Route("api/v1/[area]/[Controller]")]
public class UsersController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> QueryAsync(QueryUserRequest request)
    {
        var users = new QueryUsers
        {
            Predicate = p => p.Status == Status.Enable
        };
        var result = await mediator.Send(users);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync([FromRoute] int id)
    {
        var result = await mediator.Send(new GetUserDetail { Id = id });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateUserRequest userRequest)
    {
        var result = await mediator.Send(new CreateUser());
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, UpdateUserRequest request)
    {
        var result = await mediator.Send(new UpdateUser { Id = id });
        return Ok(result);
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> SetStatusAsync([FromRoute] int id, Status status)
    {
        var result = await mediator.Send(new UpdateUserStatus { Id = id });
        throw new NotImplementedException();
    }

    [HttpGet("/test")]
    public async Task<IActionResult> TestAsync()
    {
        return Ok();
    }
}