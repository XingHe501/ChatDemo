using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Reactor;

namespace WorldServer.Service
{
    public class Acceptor : IReactorEventHandler
    {
        private int _port;
        private Socket _socket;
        private Reactor.Reactor _reactor;

        public Action<Socket> OnNewConnection;

        public Acceptor(int Port, Reactor.Reactor reactor)
        {
            _port = Port;
            _reactor = reactor;
        }

        /// <summary>
        /// 处理Reactor事件回调
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool HandleEvent(EventType Type, Socket socket)
        { switch (Type)
            {
                case EventType.Read:
                    _accept();
                    break;
                case EventType.Error:
                    Console.WriteLine("Error: Acceptor encounters error");
                    return false;
            }
            return true;
        }

        public void Start()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ep = new IPEndPoint(IPAddress.Any, _port);
            _socket.Bind(ep);
            _socket.Listen();
            Console.WriteLine($"Listening on {IPAddress.Any}:{_port}");
            _reactor.Register(_socket, EventType.Read, this);
            _reactor.Register(_socket, EventType.Error, this);
        }

        void _accept()
        {
            var sock = _socket.Accept();
            Console.WriteLine($"New Connection is created {sock}");
            OnNewConnection?.Invoke(sock);
        }
    }
}
