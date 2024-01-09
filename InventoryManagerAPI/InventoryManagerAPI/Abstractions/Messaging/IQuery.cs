using MediatR;

namespace InventoryManagerAPI.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
