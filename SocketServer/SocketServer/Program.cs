// See https://aka.ms/new-console-template for more information
using SocketServer;

Console.WriteLine("Starting Server!");
SocketListener socketListener = new SocketListener();
socketListener.DoListen(11000);
