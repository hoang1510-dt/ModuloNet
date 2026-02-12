using MediatR;

namespace ModuloNet.Application.Behaviors;

/// <summary>
/// Placeholder for cross-cutting transaction (e.g. wrap handler in DB transaction when Infrastructure supports it).
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => await next();
}
