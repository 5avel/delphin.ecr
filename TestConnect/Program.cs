using System;
using System.Net.Sockets;

namespace TestConnect
{
    class Program
    {
        static void Main(string[] args)
        {
            ECR ecr = new ECR();

            ecr.Connect("192.168.0.157", 5000);
            ecr.Disconnect();

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }


    class ECR
    {

        internal TcpClient client = null;
        private NetworkStream tcpStream = null;
        private byte[] SEP = { 9 };
        private byte[] answer = new byte[256];
        private int answerlenght = 0;


        public bool Connect(string ip, int port )
        {
            Console.WriteLine("ip - {0}, port - {1}", ip, port);
           
            try
            {
                client = new TcpClient();
                Console.WriteLine("tcp client Connecting");
                client.Connect(ip, port); // Соединяемся с сервером
                if (client.Connected)
                {
                    Console.WriteLine("tcp client Connected");
                    tcpStream = client.GetStream();
                    byte[] sendBytes = { 5, 5, 1, 0, 1, 0 }; // Open SET
                    tcpStream.Write(sendBytes, 0, sendBytes.Length);

                    byte[] bytes = new byte[client.ReceiveBufferSize];
                    int bytesRead = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);
                    if (bytesRead > 0)
                    {
                        Console.WriteLine($"b[5] = {bytes[5]}");
                        if (bytes[5] == 00)
                        {

                            return true;
                        }
                        else if (bytes[5] == 02)
                        {
                            return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Exception1: " + ex.ToString());
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                if (client.Connected)
                {
                    tcpStream = client.GetStream();
                    byte[] sendBytes = { 5, 5, 2, 0, 1, 0 }; // Close SET
                    tcpStream.Write(sendBytes, 0, sendBytes.Length);

                    byte[] bytes = new byte[client.ReceiveBufferSize];
                    int bytesRead = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);
                    if (bytesRead > 0)
                    {
                        if (bytes[5] == 00)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                return false;
            }
            finally
            {
                client.Close();
            }
        }
    }
}
