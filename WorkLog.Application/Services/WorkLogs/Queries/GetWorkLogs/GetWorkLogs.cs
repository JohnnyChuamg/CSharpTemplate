using MediatR;

namespace WorkLog.Application.Services.WorkLogs;

public class GetWorkLogs : IRequest<IEnumerable<GetWorkLogsResponse>>
{
    public class GetWorkLogsHandle : IRequestHandler<GetWorkLogs, IEnumerable<GetWorkLogsResponse>>
    {
        public async Task<IEnumerable<GetWorkLogsResponse>> Handle(GetWorkLogs request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}