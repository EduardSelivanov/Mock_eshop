using MediatR;

namespace CommonPractices.CQRS
{
    public interface ICommandCP : IRequest { }
    public interface ICommandCP<TResponse> : IRequest<TResponse> { }
}
