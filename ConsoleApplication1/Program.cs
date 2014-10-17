using System;
using System.Net.Sockets;
using System.Text;

using Delphin;
namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ECR ecr = new ECR();
            Console.WriteLine("Connect - " + ecr.Connect("192.168.1.2", 5999, 1).ToString());


            ecr.Beep("1000", "100").ToString();
            ecr.Beep("500", "100").ToString();
            ecr.Beep("1000", "100").ToString();
            ecr.Beep("500", "100").ToString();

            ecr.ReadPlu("1");
            //ecr._plu.Name = "ewe";
            Console.WriteLine(ecr._plu.Name);


            Console.WriteLine("DisConnect - " + ecr.Disconnect().ToString());
           // byte[] b = {107};
           // Console.WriteLine(Encoding.ASCII.GetString(b));

            //Console.WriteLine(BitConverter.ToString(Encoding.ASCII.GetBytes("R")));



            Console.ReadKey();
        } // Main
    } 
}




//String server = "192.168.1.2"; int port = 5999;
//Console.WriteLine("IP:"+server+" port:"+port);
//try
//{
//    TcpClient client = new TcpClient();
//    client.Connect(server, port); // Соединяемся с сервером
//    Console.WriteLine("Connected - " + client.Connected);
//    if(client.Connected)
//    {
//        NetworkStream tcpStream = client.GetStream();
//        byte[] sendBytes = {5, 5, 1, 0, 1, 0 };
//        Console.WriteLine("->> byte array: " + BitConverter.ToString(sendBytes));
//        tcpStream.Write(sendBytes, 0, sendBytes.Length);

//        byte[] bytes = new byte[client.ReceiveBufferSize];
//        int bytesRead = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);
//        // Строка, содержащая ответ от сервера
//        string returnData = BitConverter.ToString(bytes, 0, bytesRead);
//        Console.WriteLine("<<- byte array: " + returnData);
//        Console.WriteLine("<<- String array: " + Encoding.ASCII.GetString(bytes, 7, bytesRead));
//        //client.Close();
//    }

//    Console.WriteLine("Connected - " + client.Connected);
//}
//catch (SocketException ex)
//{
//    Console.WriteLine("Exception: " + ex.ToString());
//}
//Console.ReadKey();