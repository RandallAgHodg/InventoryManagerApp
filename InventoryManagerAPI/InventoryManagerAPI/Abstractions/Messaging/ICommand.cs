using MediatR;

namespace InventoryManagerAPI.Abstractions.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
