using MediatR;


namespace CommonPractices.CQRS
{
    public interface IQuery : IRequest { }

    public interface IQueryCP<TResponse>:IRequest<TResponse>
    {
    }
    
}
