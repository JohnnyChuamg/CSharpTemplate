using MediatR;

namespace WorkLog.Application.Services.Projects.Queries.GetProjectsQuery;

public class GetProjects : IRequest<IEnumerable<GetProjectsResponse>>
{
    public class GetProjectsQueryHandle : IRequestHandler<GetProjects, IEnumerable<GetProjectsResponse>>
    {
        public Task<IEnumerable<GetProjectsResponse>> Handle(GetProjects request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}