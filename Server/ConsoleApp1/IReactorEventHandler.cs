using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace WorldServer.Reactor
{
    public enum EventType
    {
        Write,
        Read,
        Error
    }
    public interface IReactorEventHandler
    {
        public bool HandleEvent(EventType Type, Socket socket);
    }
}
