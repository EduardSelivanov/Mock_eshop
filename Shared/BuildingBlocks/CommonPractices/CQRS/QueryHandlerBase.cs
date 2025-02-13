using MediatR;


namespace CommonPractices.CQRS
{
    public interface IQueryHanlerdBase<TQuery> : IRequestHandler<TQuery>
        where TQuery : IRequest
    {
       
    }

    public interface IQueryHandlerBase<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IRequest<TResponse>
    {
       
    }
}
