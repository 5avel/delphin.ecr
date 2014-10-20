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
            if(Send(sendBytes))
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
                return GetPlu();
            }
            return false;
        }

        public bool DeletingPlu(int pluCode)
        {                                                             
            return DeletingPlu(pluCode, pluCode);
        }

        public bool WritePlu(int Row, int Code, string Name, double Price)
        {

            return true;
        }

        public bool UpdatePlu(int Row, int Code, string Name, double Price)
        {
            
            return true;
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
            String temp = "";
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

#endregion Privat methods

    } // class ECR
} // namespace DP_25_ole
