// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Text;

var list = new List<Socket>();

for (int i = 0; i < 3; i++)
{
    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect("localhost", 51111);
    list.Add(socket);
}

    var buffer = new byte[2048];
    while (true)
    {
        foreach (var socket in list)
        {
            socket.Send(Encoding.UTF8.GetBytes("11111"));
            socket.Receive(buffer);
            Console.WriteLine("Socket receive: "+Encoding.UTF8.GetString(buffer,0, 5));
        }
        Thread.Sleep(5000);
    }
