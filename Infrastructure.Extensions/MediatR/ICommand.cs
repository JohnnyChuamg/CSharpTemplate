using MediatR;

namespace Infrastructure.Extensions.MeidatR;

public interface ICommand<out TResponse> : IRequest<TResponse>;