using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Linq;

namespace Delphin
{
    [Guid("a0cc9128-46fc-4bf7-a2b8-76d1e81ae686"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IEvents))]
    public class ECR : IECR
    {

#region Private Field
        private TcpClient client = null;
        private NetworkStream tcpStream = null;
        private byte logNum = 0;
        private byte[] SEP = {9};
        private byte[] answer = new byte[256];
        private int answerlenght = 0;
#endregion Private Field

#region Public Field
        public PLU plu = new PLU();
#endregion Public Field

#region Public methods

        public bool Connect(string ip, int port, byte logNum)
        {
            if(logNum>0|| logNum<100)
            {
                this.logNum = logNum;
            }
            else
            {
                return false;
            }
            try
            {
                client = new TcpClient();
                client.Connect(ip, port); // Соединяемся с сервером
                if (client.Connected)
                {
                    tcpStream = client.GetStream();
                    byte[] sendBytes = { 5, 5, 1, 0, logNum, 0 }; // Open SET
                    tcpStream.Write(sendBytes, 0, sendBytes.Length);

                    byte[] bytes = new byte[client.ReceiveBufferSize];
                    int bytesRead = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);
                    if (bytesRead > 0)
                    {
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
            catch(SocketException ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
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
                    byte[] sendBytes = { 5, 5, 2, 0, this.logNum, 0 }; // Close SET
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

        public bool Beep(int tone, int len)
        {
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 56, 48, 09 };
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(tone.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(len.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            return Send(sendBytes);
        }

        public bool ReadPlu(int pluCode)
        {                                                             // R
                                                       //31h 30h 37h 09h 52h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 48, 55, 09, 82, 09 };
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(pluCode.ToString())).ToArray(); // 31h
            sendBytes = sendBytes.Concat(SEP).ToArray(); // 09h
            if(Send(sendBytes) && answerlenght > 8)
            {
               return GetPlu();
            }
            return false;
        }

        public bool DeletingPlu(int firstPlu, int lastPlu)
        {                                                             // D
                                                       //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 48, 55, 09, 68, 09 };
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(firstPlu.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray(); // 09h
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(lastPlu.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray(); // 09h
            if (Send(sendBytes))
            {
                return true;
            }
            return false;
        }

        public bool DeletingPlu(int pluCode)
        {                                                             
            return DeletingPlu(pluCode, pluCode);
        }

        public bool WritePlu(   int plu, byte taxGr, byte dep, byte group, byte priceType, double price, double addQty,
                                double quantity, string bar1, string bar2, string bar3, string bar4, string name, int connectedPLU)
        {                                                             // P
                                                       //31h 30h 37h 09h 50h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 48, 55, 09, 80, 09 };
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
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(price.ToString().Replace(',','.'))).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (addQty < 0)
            {
                sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(addQty.ToString().Replace(',', '.'))).ToArray();
            }
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(quantity.ToString().Replace(',', '.'))).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(bar1.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(bar2.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(bar3.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(bar4.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray(); 
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(name.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (addQty < 0)
            {
                sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(connectedPLU.ToString())).ToArray();
            }
            sendBytes = sendBytes.Concat(SEP).ToArray(); 
            if (Send(sendBytes))
            {
                return true;
            }
            return false;
        }


        public bool WritePlu(   int plu, double price, string name, byte taxGr = 1,  byte dep = 1,
                                byte group = 1,  byte priceType = 0, double addQty = 0, double quantity = 0,
                                string bar1 = "", string bar2 = "", string bar3 = "", string bar4 = "", int connectedPLU = 0)
        {
            return WritePlu(plu, taxGr, dep, group, priceType, price, addQty, quantity, bar1, bar2, bar3, bar4, name, connectedPLU);
        }

        public bool WritePlu(int plu, byte taxGr, double price, double quantity, string bar1, string name, int connectedPLU)
        {
            return WritePlu(plu, taxGr, 1, 1, 0, price, 0, quantity, bar1, "", "", "", name, connectedPLU);
        }

        public bool WritePlu(int plu, byte taxGr, double price, string bar1, string name)
        {
            return WritePlu(plu, taxGr, price, 0, bar1, name , 0);
        }

        public bool WritePlu(int plu, byte taxGr, double price, string name)
        {
            return WritePlu(plu, taxGr, price, "", name);
        }

        /// <summary>
        /// Check for mode connection with PC
        /// </summary>
        /// <returns></returns>
        public bool CheckMode()
        {                                                             // D
                                                       //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 45, 09 };

            if (Send(sendBytes))
            {
                return true;
            }
            return false;
        }

        public bool GetDataTime()
        {                                                             // D
            //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 54, 50, 09};

            if (Send(sendBytes))
            {
                Console.WriteLine(Encoding.Default.GetString(answer, 7, 30));
                return true;
            }
            return false;
        }

        public bool GetEJ()
        {
            
            // 124|17-10-14 00:00:01 DST|20-10-14 00:00:01 DST|
            /* HHHHHHHHHHH    1 7  -  1  0  -  1  4   " " 0  0  :  0  0  :  0  1   " " D  S  T  |
             31-32-34-7C-    31-37-2D-31-30-2D-31-34- 20 -30-30-3A-30-30-3A-30-31- 20 -44-53-54-7C-
             * 
             *                2 0  -  1  0  -  1  4   " " 0  0  :  0  0  :  0  1   " " D  S  T  |
             *               32-30-2D-31-30-2D-31-34- 20 -30-30-3A-30-30-3A-30-31- 20 -44-53-54-7C
             */

            // 124
                                                       //31h 32h 34h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 52, 09, 09, 09 };

            if (Send(sendBytes))
            {
                Console.WriteLine(Encoding.Default.GetString(answer, 7, 50));
                return true;
            }
            return false;
        }




#endregion Public methods

#region Privat methods

        private bool Send(byte[] sendBytes)
        {
            if (client.Connected)
            {
                byte[] leng = { Convert.ToByte(sendBytes.Length) };
                sendBytes = leng.Concat(sendBytes).ToArray();
                tcpStream.Write(sendBytes, 0, sendBytes.Length);
                byte[] bytes = new byte[client.ReceiveBufferSize];
                int bytesRead = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);
                if (bytesRead > 0)
                {
                    if (bytes[6] == 80)
                    {
                        // answer = BitConverter.ToString(bytes, 0, bytesRead);
                        answer = bytes;
                        answerlenght = bytesRead;
                        return true;
                    }
                    else if (bytes[6] == 70)
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

        private bool GetPlu()
        {
            string[] sPlu = new string[15];
            String temp = String.Empty;
            int p = 0;
            for (int i = 8; i < answerlenght - 1; i++) // перебор массива ответа
            {
                if (answer[i] != 9) // не встретили сепаратор
                {
                    temp += ASCIIEncoding.Default.GetString(answer, i,1);
                }
                else // втретили сепаратор - значит конец строки. 
                {
                    sPlu[p++] = temp;
                    temp = String.Empty;
                }
            }
                plu.Code = Convert.ToInt32(sPlu[0]);
                plu.TaxGr = Convert.ToByte(sPlu[1]);
                plu.Dep = Convert.ToByte(sPlu[2]);
                plu.Group = Convert.ToByte(sPlu[3]);
                plu.PriceType = Convert.ToByte(sPlu[4]);
                plu.Price = Convert.ToDouble(sPlu[5].Replace(".",","));
                plu.Turnover = Convert.ToDouble(sPlu[6].Replace(".", ","));
                plu.SoldQty = Convert.ToDouble(sPlu[7].Replace(".", ","));
                plu.StockQty = Convert.ToDouble(sPlu[8].Replace(".", ","));
                plu.Bar1 = sPlu[9];
                plu.Bar2 = sPlu[10];
                plu.Bar3 = sPlu[11];
                plu.Bar4 = sPlu[12];
                plu.Name = sPlu[13];
                plu.ConnectedPLU = Convert.ToInt32(sPlu[14]);
            return true;
        }


        /*
         Command: 90 (5Ah)

            Diagnostic information

            This is example command syntax: Sintax 1:

            {Param}<SEP>
            Optional parameters:

            none - Diagnostic information without firmware checksum;
            Answer(1)
            1 - Diagnostic information with firmware checksum;
            Answer(1)
            # - Device identification;
            Answer(2)
         */

#endregion Privat methods

    } // class ECR
} // namespace DP_25_ole
