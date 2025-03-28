using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server_Echo
{
    class Program
    {
        private static readonly List<Socket> Clients = new List<Socket>();
        private static readonly object ClientsLock = new object();

        static void Main()
        {
            const int port = 11000;
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ipEndPoint);
                listener.Listen(10);
                Console.WriteLine($"Сервер запущен на порту {port}...");

                while (true)
                {
                    Socket handler = listener.Accept();
                    lock (ClientsLock)
                    {
                        Clients.Add(handler);
                    }

                    ThreadPool.QueueUserWorkItem(HandleClient, handler);
                }
            }
            finally
            {
                listener.Close();
            }
        }

        static void HandleClient(object state)
        {
            Socket handler = (Socket)state;
            string clientEndpoint = handler.RemoteEndPoint.ToString();
            bool debugMode = false;
            bool echoMode = false;

            try
            {
                Console.WriteLine($"Клиент подключен: {clientEndpoint}");
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int received = handler.Receive(buffer);
                    if (received == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Получено от {clientEndpoint}: {request}");

                    if (request.Contains("<TheEnd>"))
                    {
                        Console.WriteLine($"Клиент {clientEndpoint} отключился");
                        break;
                    }

                    switch (request)
                    {
                        case "<DebugOn>":
                            debugMode = true;
                            continue;
                        case "<DebugOff>":
                            debugMode = false;
                            continue;
                        case "<EchoOn>":
                            echoMode = true;
                            continue;
                        case "<EchoOff>":
                            echoMode = false;
                            continue;
                    }

                    if (debugMode)
                    {
                        Thread.Sleep(500);
                        string response = $"Ответ сервера: {request.Length} символов";
                        handler.Send(Encoding.UTF8.GetBytes(response));
                        continue;
                    }
                    if (echoMode)
                    {
                        Thread.Sleep(500);
                        string response = $"Ответ сервера: {request}";
                        handler.Send(Encoding.UTF8.GetBytes(response));
                        continue;
                    }
                    // Рассылка сообщения всем клиентам, кроме отправителя
                    lock (ClientsLock)
                    {
                        foreach (Socket client in Clients)
                        {
                            if (client != handler && client.Connected)
                            {
                                try
                                {
                                    client.Send(buffer, 0, received, SocketFlags.None);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Ошибка отправки клиенту {client.RemoteEndPoint}: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка с клиентом {clientEndpoint}: {ex.Message}");
            }
            finally
            {
                lock (ClientsLock)
                {
                    Clients.Remove(handler);
                }

                try
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch { }

                Console.WriteLine($"Клиент {clientEndpoint} отключен и удален из списка");
            }
        }
    }
}