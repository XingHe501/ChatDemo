// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using WorldServer.Reactor;
using WorldServer.Service;

// 协议定制{ 消息长度、消息类型、消息内容 }

var r = new Reactor(); // 单线程事件分发器
var a = new Acceptor(51111, r); // 监听端口  其他program连接51111，
a.OnNewConnection += delegate(Socket socket)
{
    var wh = new WriteHandler(r);
    r.Register(socket, EventType.Read, wh);
    r.Register(socket,EventType.Error,wh);
    Console.WriteLine("delegate On");
};
a.Start();
r.Start();

class WriteHandler : IReactorEventHandler
{
     private byte[] buffer = new byte[2048];
     private Reactor _reactor;

     public WriteHandler(Reactor r)
     {
         _reactor = r;
     }
    public bool HandleEvent(EventType Type, Socket socket)
    {
        
        // Console.WriteLine("WirteHandle"+Type);
        switch (Type)
        {
            case EventType.Read:
                socket.Receive(buffer);
                Console.WriteLine(Encoding.UTF8.GetString(buffer,0,5));
                this._reactor.Register(socket, EventType.Write, this);
                return true;
            case EventType.Write:
                socket.Send(buffer);
                return false;
            case EventType.Error:
                Console.WriteLine("write handler error");
                return false;
        }

        return false;
    }
}