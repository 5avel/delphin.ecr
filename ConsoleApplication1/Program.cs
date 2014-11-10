using System;
using System.Net.Sockets;
using System.Text;

using Delphin;
namespace ConsoleApplication1
{
    class Program
    {/// test comita
        static void Main(string[] args)
        {
            ECR ecr = new ECR();
            Console.WriteLine("Connect - " + ecr.Connect("in4.pp.ua", 5999, 1).ToString());

            //Console.WriteLine(" lst - " + ecr.GetLastDocNumber());

            //ecr.GetDateDocByDocNum(ecr.GetLastDocNumber());
            //var doc = ecr.GetDocTxtByNum(ecr.GetLastDocNumber());
            //foreach (string s in doc)
            //{
            //     Console.WriteLine(s);
            //}
            ecr.SetDocForRead(ecr.GetLastDocNumber());
            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));
            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));
            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));
            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));
            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));
            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));
            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));

            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));

            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));

            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));

            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));

            Console.WriteLine(ecr.ReadDoc(ecr.GetLastDocNumber()));
            //for (int i = 65; i <= ecr.GetLastDocNumber(); ++i)
            //{
            var doc = ecr.GetDocTxtByNum(72);
            if (doc != null)
            {
                foreach (string s in doc)
                {
                    Console.WriteLine(s);
                }
            }
            //}
            


            ////Console.WriteLine("GetDataTime " + ecr.GetDataTime().ToString()); 11 22 26 49
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("15-10-14", "15-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("16-10-14", "16-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("17-10-14", "17-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("18-10-14", "18-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("19-10-14", "19-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("20-10-14", "20-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("21-10-14", "21-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("22-10-14", "22-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("23-10-14", "23-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("24-10-14", "24-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("25-10-14", "25-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("26-10-14", "26-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("27-10-14", "27-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("28-10-14", "28-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("29-10-14", "29-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("30-10-14", "30-10-14").ToString());
            //Console.WriteLine("GetEJ " + ecr.GetDocNumberByData("06-11-14", "").ToString());
            
            //ecr.Beep(1000, 100).ToString();
            //ecr.Beep(500, 100).ToString();
            //ecr.Beep(1000, 100).ToString();
            //ecr.Beep(500, 100).ToString();

            //Console.WriteLine("rename PLU   "+ ecr.WritePlu(15, 1, 1, 1, 0, 6, 0, 3.521, "", "", "", "", "Чай Ахмат", 0).ToString());
            //ecr.ReadPlu(15);
            //Console.WriteLine(ecr.plu.Code + "\t|  " + ecr.plu.TaxGr + "\t|  " + ecr.plu.StockQty + "\t|  " + ecr.plu.Price + "\t|  " + ecr.plu.Name);

            //ecr.WritePlu(8, 3, 12.31, "Батончик Twix MAX");
            //ecr.ReadPlu(8);
            //Console.WriteLine(ecr.plu.Code + "\t|  " + ecr.plu.TaxGr + "\t|  " + ecr.plu.StockQty + "\t|  " + ecr.plu.Price + "\t|  " + ecr.plu.Name);

            // ecr.WritePlu(9, 2, 12.39, "Батончик Twix MAX2");
            //ecr.ReadPlu(9);
            //Console.WriteLine(ecr.plu.Code + "\t|  " + ecr.plu.TaxGr + "\t|  " + ecr.plu.StockQty + "\t|  " + ecr.plu.Price + "\t|  " + ecr.plu.Name);
            

            //ecr.WritePlu(18, 15.29, "Чашка сeпа223");
            //ecr.ReadPlu(18);
            //Console.WriteLine(ecr.plu.Name);
            //ecr.DeletingPlu(1, 50);
            //ecr.DeletingPlu(1);
            //ecr.ReadPlu(1);
            ////ecr._plu.Name = "ewe";
            //Console.WriteLine(ecr.plu.Name);

            //Console.WriteLine("SetDocForRead(49) - " + ecr.SetDocForRead(49).ToString());

            Console.WriteLine("DisConnect - " + ecr.Disconnect().ToString());
            //byte[] b = {89, 119, 65, 65 };
            //Console.WriteLine(Convert.FromBase64String(b.ToString()));

            ////Console.WriteLine(BitConverter.ToString(Encoding.Default.GetBytes("124|17-10-14 00:00:01 DST|20-10-14 00:00:01 DST|")));

           // Console.WriteLine(BitConverter.ToString(Encoding.Default.GetBytes("1240")));

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