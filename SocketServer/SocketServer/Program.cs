// See https://aka.ms/new-console-template for more information
using SocketServer;

Console.WriteLine("Starting Server!");
SocketListener socketListener = new();
socketListener.DoListen(11000);
