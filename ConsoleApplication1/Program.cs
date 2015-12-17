using Delphin;
using System;
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class Program
    {/// test comita
        static void Main(string[] args)
        {



            //ECRfor1C ecr = new ECRfor1C();
            //Console.WriteLine("Connect - " + ecr.Connect("192.168.88.53", 5999, 1).ToString());





            //Console.WriteLine(" last - " + ecr.GetLastDocNumber());

            //ecr.GetDateDocByDocNum(ecr.GetLastDocNumber());
            //var doc = ecr.GetDocTxtByNum(ecr.GetLastDocNumber());
            //foreach (string s in doc)
            //{
            //     Console.WriteLine(s);
            //}
            //int doc = ecr.GetLastDocNumber();
            //int doc = 88;

            //Check c = ecr.ReadDoc(doc);
            //if (c != null)
            //{
            //    Console.WriteLine("Номер чека -" + c.num);
            //    Console.WriteLine("Чек возврата? -" + c.isReturnCheck);
            //    Console.WriteLine(c.goods[0].name);
            //}
            //////for (int i = 65; i <= ecr.GetLastDocNumber(); ++i)
            //////{
            //var docStr = ecr.GetDocTxtByNum(doc);
            //if (docStr != null)
            //{
            //    foreach (string s in docStr)
            //    {
            //        Console.WriteLine(s);
            //    }
            //}

             //for (int i = ecr.GetLastDocNumber(); i > 0; --i)
             //{
             //    Console.WriteLine(i+"  -  "+ecr.GetDateDocByDocNum(i));
             //}


            //Console.WriteLine(ecr.GetFirstDocNumberByDate("29-12-14"));
           // Console.WriteLine(ecr.GetLastDocNumber());

            //ecr.GetCheckByNum(266);
            //Console.WriteLine("  -//-//-//-  ");
            //ecr.GetCheckByNum(177);
            //ecr.GetCheckByNum(23);

            //ecr.GetDateDocByDocNum(150);

            //test

            //Console.WriteLine("DisConnect - " + ecr.Disconnect().ToString());


            //// TEST ECRfor1C

            //int mils = DateTime.Now.Millisecond;
            ECRfor1C ecr1c = new ECRfor1C();



            Console.WriteLine(ecr1c.Connect("192.168.88.52", 5999, 1));
           // Console.WriteLine(ecr1c.GetDataTime());
            ecr1c.Beep(1555, 500);

            ecr1c.DataSalesFrom = "01-12-15";
            ecr1c.DataSalesTo = "15-12-15";
            while (ecr1c.GetCheck())
            { 
            
            }
            //List<string> list = ecr1c.SearchReceipt("25-07-15 00:00:00 DST", "25-07-15 23:00:00 DST");

            //Console.WriteLine(list[0]);          
            //Console.WriteLine(list[1]);          
            //Console.WriteLine(list[2]);          
            //Console.WriteLine(list[3]);          
                                                   
                                                   

            //mils = mils - DateTime.Now.Millisecond;

            //Console.WriteLine(mils);

            //ecr1c.DataSales = "22-05-15";
            //while (ecr1c.GetCheck())
            //{
            //    Console.WriteLine("\n Чек - " + ecr1c.JCheckNum + "  Дата - " + ecr1c.JCheckDate + "\n  Скидка на чек " + ecr1c.JCheckDis + "%" + " Возврат - " + ecr1c.JCheckIsReturn + " Отменен -" + ecr1c.JCheckIsVoid + " Z -" + ecr1c.JCheckNumZRep);
            //    while (ecr1c.ReadSales())
            //    {
            //        Console.WriteLine("\t" + ecr1c.JArtCode + " " + ecr1c.JArtName + " " + ecr1c.JArtPrice + " " + ecr1c.JArtQnt + " " + ecr1c.JArtSum + " " + ecr1c.JArtDis + " " + ecr1c.JArtVoid);
            //    }
            //}

            //ecr1c.DataSales = "25-05-15";
            //while (ecr1c.GetCheck())
            //{
            //    Console.WriteLine("\n Чек - " + ecr1c.JCheckNum + "  Дата - " + ecr1c.JCheckDate + "\n  Скидка на чек " + ecr1c.JCheckDis + "%" + " Возврат - " + ecr1c.JCheckIsReturn + " Отменен -" + ecr1c.JCheckIsVoid + " Z -" + ecr1c.JCheckNumZRep);
            //    while (ecr1c.ReadSales())
            //    {
            //        Console.WriteLine("\t" + ecr1c.JArtCode + " " + ecr1c.JArtName + " " + ecr1c.JArtPrice + " " + ecr1c.JArtQnt + " " + ecr1c.JArtSum + " " + ecr1c.JArtDis + " " + ecr1c.JArtVoid);
            //    }
            //}


            //ecr1c.DataSalesFrom = "28-07-15";
            //ecr1c.DataSalesTo = "28-07-15";
            //while (ecr1c.GetCheck())
            //{
            //    Console.WriteLine("\n Чек - " + ecr1c.JCheckNum + "  Дата - " + ecr1c.JCheckDate + "\n  Скидка на чек " + ecr1c.JCheckDis + "%" + " Возврат - " + ecr1c.JCheckIsReturn + " Отменен -" + ecr1c.JCheckIsVoid + " Z -" + ecr1c.JCheckNumZRep);
            //    while (ecr1c.ReadSales())
            //    {
            //        Console.WriteLine("\t" + ecr1c.JArtCode + " " + ecr1c.JArtName + " " + ecr1c.JArtPrice + " " + ecr1c.JArtQnt + " " + ecr1c.JArtSum + " " + ecr1c.JArtDis + " " + ecr1c.JArtVoid);
            //    }
            //    Console.WriteLine("Количество Оплат = " + ecr1c.JCheckPayCount);
            //    Console.WriteLine("ВИД Оплаты1 - " + ecr1c.JCheckPay1Type);
            //    Console.WriteLine("Сумма Оплаты1 - " + ecr1c.JCheckPay1Sum);
            //    Console.WriteLine("ВИД Оплаты2 - " + ecr1c.JCheckPay2Type);
            //    Console.WriteLine("Сумма Оплаты2 - " + ecr1c.JCheckPay2Sum);
            //    Console.WriteLine("ВИД Оплаты3 - " + ecr1c.JCheckPay3Type);
            //    Console.WriteLine("Сумма Оплаты3 - " + ecr1c.JCheckPay3Sum);
            //    Console.WriteLine("Сумма Чека - " + ecr1c.JCheckSum);
            //    Console.WriteLine("Tax1 - " + ecr1c.JCheckTax1Sum);
            //    Console.WriteLine("Tax1Zbir - " + ecr1c.JCheckTax1Zbir);

            //}







            Console.WriteLine(ecr1c.Disconnect());

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

           //Console.WriteLine("rename PLU   "+ ecr.WritePlu(1522, 1, 1, 1, 0, 6, 0, 3.521, "", "", "", "", "Чай Ахмат Tea", 0).ToString());
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

            
            //byte[] b = {242, 05, 00, 00, 00, 00, 00, 00 };
            //Console.WriteLine(BitConverter.ToUInt64(b, 0)); // Код товара

            //byte[] b1 = { 136, 19, 00, 00, 00, 00, 00, 00 };
            //Console.WriteLine(BitConverter.ToUInt32(b1, 0)); // Количество в граммах

            //byte[] b2 = { 184, 11, 00, 00, 00, 00, 00, 00 };
            //Console.WriteLine(BitConverter.ToUInt32(b2, 0)); // Количество в граммах

            //byte[] b3 = { 244, 01, 00, 00 };
            //Console.WriteLine(BitConverter.ToUInt32(b3, 0)); // Количество в граммах


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

