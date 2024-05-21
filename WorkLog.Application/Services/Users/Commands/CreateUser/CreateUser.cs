using Infrastructure.Commons;
using Infrastructure.Contracts.ResultContracts;
using Infrastructure.Data.Repositories;
using Infrastructure.Data.Repositories.EntityFramework.Extensions;
using MediatR;
using WorkLog.Domain.Entities;
using WorkLog.Infrastructure.Enums;

namespace WorkLog.Application.Services.Users;

public class CreateUser : IRequest<Result>
{
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public long RoleId { get; set; }
    public long DepartmentId { get; set; }
    public Status Status { get; set; } = Status.Enable;

    public class CreateUserHandle(IRepositoryFactory repositoryFactory, Snowflake snowflake)
        : IRequestHandler<CreateUser, Result>
    {
        public async Task<Result> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            using var repoUser = repositoryFactory.Create<User>();

            if (await repoUser.ExistsAsync(user => user.Username == request.Username, cancellationToken))
                return await Result.ConflictAsync(nameof(request.Username));

            var created = new User
            {
                Id = snowflake.Generate(),
                Username = request.Username,
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Status = request.Status,
                RoleId = request.RoleId,
                DepartmentId = request.DepartmentId,
            };

            await repoUser.CreateAsync(created, cancellationToken);

            repositoryFactory.SaveChanges();

            return await Result.SuccessCreatedAsync();
        }
    }
}