// See https://aka.ms/new-console-template for more information
using SocketClient;

Console.WriteLine("Starting Sender!");

SocketSender theClient = new();
theClient.DoSend(11000);
