using MediatR;


namespace CommonPractices.CQRS
{
    public interface ICommHandBase<TCommand> : IRequestHandler<TCommand>
        where TCommand : IRequest
    {
        
    }

    public interface ICommHandBase<TCommand,TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
    {
        
    }
}
