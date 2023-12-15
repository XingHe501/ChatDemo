using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace WorldServer.Reactor
{
    public class Dispatcher
    {
        class SocketIO
        {
            public IReactorEventHandler Handler;
            public Socket Socket;
        }
        private Dictionary<EventType, Dictionary<Socket, IReactorEventHandler>> _sockets;

        public Dispatcher()
        {
            _sockets = new Dictionary<EventType, Dictionary<Socket, IReactorEventHandler>>();
            foreach (EventType type in Enum.GetValues(typeof(EventType)))
                _sockets[type] = new Dictionary<Socket, IReactorEventHandler>();
        }

        /// <summary>
        /// Thread-safe
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Socket"></param>
        /// <param name="Handler"></param>
        public void Register(EventType Type, Socket Socket, IReactorEventHandler Handler)
        {
            lock (this._sockets)
            {
                if (_sockets[Type].ContainsKey(Socket))
                    _sockets[Type][Socket] = Handler;
                else
                    _sockets[Type].Add(Socket, Handler);
            }
        }

        public void Unregister(EventType Type, Socket Socket)
        {
            lock (this._sockets)
                _sockets[Type].Remove(Socket); 
        }

        public List<Socket> Select(EventType Type)
        {
            lock (this._sockets)
            {
                var sockets = this._sockets[Type];
                return sockets.Keys.ToList<Socket>();
            }
        }

        public void Dispatch(EventType Type, List<Socket> Socks)
        {
            foreach(var sock in Socks)
            {
                IReactorEventHandler handler;
                lock (this._sockets)
                    handler = this._sockets[Type][sock];

                if (!handler.HandleEvent(Type, sock))
                    Unregister(Type, sock);
            }
        }

    }
}
