using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server_Echo
{
    class Program
    {
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
                    // Open a Multiplying, one flow for one user
                    ThreadPool.QueueUserWorkItem(HandleClient, handler);
                }
            }
            finally
            {
                listener.Close();
            }
        }

        static void HandleClient( object state) // object `cause ThreadPool takes only object 
        {
            Socket handler = (Socket)state; ;


            try
            {
                Console.WriteLine($"Клиент подключен: {handler.RemoteEndPoint}");
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int received = handler.Receive(buffer);
                    if (received == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Получено: {request}");

                    if (request.Contains("<TheEnd>"))
                    {
                        Console.WriteLine("Клиент отключился");
                        break;
                    }

                    // Добавляем задержку перед ответом 
                    Thread.Sleep(500);

                    string response = $"Ответ сервера: {request.Length} символов";
                    handler.Send(Encoding.UTF8.GetBytes(response));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}