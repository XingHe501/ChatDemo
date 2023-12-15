using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Net;

namespace WorldServer.Reactor
{

    public class Reactor
    {
        private Dispatcher _dispatcher;

        // use to interrupt select
        private Socket _dummyServer;
        private Socket _dummyClient;

        public Reactor()
        {
            this._dummyServer = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _dummyServer.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            this._dummyClient = new Socket(SocketType.Dgram, ProtocolType.Udp);
            this._dummyClient.Connect(_dummyServer.LocalEndPoint);
            this._dispatcher = new Dispatcher();
            this._dispatcher.Register(EventType.Read, this._dummyServer, new DummySocketHandler());
        }

        public void Register(Socket Sock, EventType Type, IReactorEventHandler Handler)
        {
            this._dispatcher.Register(Type, Sock, Handler);
            _wakeup();
        }

        public void Unregister(Socket Sock, EventType Type)
        {
            this._dispatcher.Unregister(Type, Sock);
            _wakeup();
        }

        public void Start()
        {
            Console.WriteLine("reactor start");
            while (true)
                _loop();
        }

        private void _wakeup()
        {
            this._dummyClient.Send(new byte[] { 0 });
        }

        private void _loop()
        {
            var r = _dispatcher.Select(EventType.Read);
            var w = _dispatcher.Select(EventType.Write);
            var e = _dispatcher.Select(EventType.Error);

            Socket.Select(r, w, e, -1);
            _dispatcher.Dispatch(EventType.Read, r);
            _dispatcher.Dispatch(EventType.Write, w);
            _dispatcher.Dispatch(EventType.Error, e);
        }

        class DummySocketHandler : IReactorEventHandler
        {
            public bool HandleEvent(EventType Type, Socket socket)
            {
                socket.Receive(new byte[1]);
                // do nothing
                return true;
            }
        }
    }
}
