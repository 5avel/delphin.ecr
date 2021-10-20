using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

// 49 9 48 9 49 9 49 9 49 9 53 48 46 50 50 9 48 46 48 48 9 48 46 48 48 48 9 48 46 48 48 48 9
// 50 9 48 9 49 9 49 9 49 9 49 48 46 48 48 9 48 46 48 48 9 48 46 48 48 48 9 48 46 48 48 48 9

// 52 56 50 48 55 55 56 50 9 48                                                       9 48                9 48 9 210 197 209 210 49 9 48 9 52 53 55 53 53 50 50 55 53 55 9 48 9 
// 48                      9 49 49 53 51 48 48 57 55 48 54 48 53 53 50 51 55 55 50 56 9 49 54 52 57 54 56 9 48 9 210 197 209 210 50 9 48 9 9 48 9
//                           1  1  5  3  0  0  9  7  0  6  0  5  5  2  3  7  7  2  8   

// 1       0       1       1       1       50.22   0.00    0.000   0.000   48207782        0       0       0       ТЕСТ1   0       4575522757      0
// 2       0       1       1       1       10.00   0.00    0.000   0.000   0       1153009706055237728     164968  0       ТЕСТ2   0               0



//{PLU}<SEP>{TaxGr}<SEP>{Dep}<SEP>{Group}<SEP>{PriceType}<SEP>{Price}<SEP>{AddQty}<SEP>{Quantity}<SEP>{Bar1}<SEP>{Bar2}<SEP>{Bar3}<SEP>{Bar4}<SEP>{Name}<SEP>{FractionalQty}<SEP>{CustomCode}<SEP>{Unit}<SEP>
//{PLU}<SEP>{TaxGr}<SEP>{Dep}<SEP>{Group}<SEP>{PriceType}<SEP>{Price}<SEP>{Tu-ver}<SEP>{SoldQty }<SEP>{StockQty}<SEP>{Bar1}<SEP>{Bar2}<SEP>{Bar3}<SEP>{Bar4}<SEP>{Name}<SEP>{FractionalQty}<SEP>{CustomCode}<SEP>{Unit}<SEP>


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

        private bool Send(byte[] sendBytes)
        {
            if (!client.Connected) return false;
            byte[] leng = { Convert.ToByte(sendBytes.Length) };
            sendBytes = leng.Concat(sendBytes).ToArray();
            tcpStream.Write(sendBytes, 0, sendBytes.Length);
            byte[] bytes = new byte[client.ReceiveBufferSize];
            int bytesRead = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);
            if (bytesRead == 0) return false;
            if (bytes[6] != 48) return false;
            answer = bytes;
            answerlenght = bytesRead;
            return true;

        }

        public bool WritePlu(int plu, string taxGr, byte dep, byte group, byte priceType, double price, double addQty,
                               double quantity, string barX, string name, int fractionalQty, string customCode)
        {
            if (client.Connected == false) return false; // состояние соединенияя

            // P
            //31h 30h 37h 09h 50h 09h  
            byte[] sendBytes = { 5, 17, 0, 1, 0, 49, 48, 55, 9, 80, 9 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(plu.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(taxGr.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(dep.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(group.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(priceType.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(price.ToString().Replace(',', '.'))).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            //if (addQty < 0)
            //{
            //sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(addQty.ToString().Replace(',', '.'))).ToArray();
            //}
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(quantity.ToString().Replace(',', '.'))).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            //sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(barX.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            //sendBytes = sendBytes.Concat(Encoding.Default.GetBytes("".ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            //sendBytes = sendBytes.Concat(Encoding.Default.GetBytes("".ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            //sendBytes = sendBytes.Concat(Encoding.Default.GetBytes("".ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(name.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(fractionalQty.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(customCode.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes("0".ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();

            if (Send(sendBytes))
            {
                return true;
            }
            return false;
        }
    }
}
