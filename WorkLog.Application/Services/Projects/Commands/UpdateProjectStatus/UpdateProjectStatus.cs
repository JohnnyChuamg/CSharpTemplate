using MediatR;

namespace WorkLog.Application.Services.Projects;

public class UpdateProjectStatus : IRequest<UpdateProjectStatusResponse>
{
    public class UpdateProjectStatusHandle : IRequestHandler<UpdateProjectStatus, UpdateProjectStatusResponse>
    {
        public Task<UpdateProjectStatusResponse> Handle(UpdateProjectStatus command,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}