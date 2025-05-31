using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Interfaces
{
    public interface IMessageHandler<Guid, TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(Guid? playerId, TRequest msg, CancellationToken ct);
    }
}
