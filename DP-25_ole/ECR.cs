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

        private int eJfirstDoc = 0, eJlastDoc = 0; // поля заполняются методом GetDocNumber(ДатаС, ДатаПО), если вернул true.
 
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
                List<string> lPlu = Separating();
                plu.Code = Convert.ToInt32(lPlu[0]);
                plu.TaxGr = Convert.ToByte(lPlu[1]);
                plu.Dep = Convert.ToByte(lPlu[2]);
                plu.Group = Convert.ToByte(lPlu[3]);
                plu.PriceType = Convert.ToByte(lPlu[4]);
                plu.Price = Convert.ToDouble(lPlu[5].Replace(".", ","));
                plu.Turnover = Convert.ToDouble(lPlu[6].Replace(".", ","));
                plu.SoldQty = Convert.ToDouble(lPlu[7].Replace(".", ","));
                plu.StockQty = Convert.ToDouble(lPlu[8].Replace(".", ","));
                plu.Bar1 = lPlu[9];
                plu.Bar2 = lPlu[10];
                plu.Bar3 = lPlu[11];
                plu.Bar4 = lPlu[12];
                plu.Name = lPlu[13];
                plu.ConnectedPLU = Convert.ToInt32(lPlu[14]);
               return true;
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
        /// Check for mode connection with PC Проверить на COM порту.
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

        public string GetDataTime()
        {                                                             // D
            //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 54, 50, 09};

            if (Send(sendBytes))
            {

                return Encoding.Default.GetString(answer, 7, 30);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dstsTime">DateTime - Date and time in format: DD-MM-YY<SPACE>hh:mm:ss<SPACE>DST</param>
        /// <returns></returns>
        public bool SetDataTime(string dataTime)
        {                                                             // D
            //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 54, 49, 09 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(dataTime)).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();

            if (Send(sendBytes))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Получает диапозон документов пробитых в заданном промежутке времени.
        /// </summary>
        /// <param name="startData">>DD-MM-YY HH:MM:ss< Если пусто, то значение устанавливается как дата фискализации</param>
        /// <param name="endDate">>DD-MM-YY HH:MM:ss< Если пусто, то значение устанавливается как дата последнего фискального документа</param>
        /// <returns></returns>
        public bool GetDocNumberByDataTime(string startData, string endDate)
        {
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 52, 09};
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(startData + " DST")).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(endDate + " DST")).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();

            if (Send(sendBytes))
            {
                //Console.WriteLine(BitConverter.ToString(answer, 0, answerlenght));
              //  Console.WriteLine(Encoding.Default.GetString(answer, 7, answerlenght));
                List<string> lAnswer = Separating();
                if(lAnswer.Count == 4)
                {
                    eJfirstDoc = Convert.ToInt32(lAnswer[2]);
                    eJfirstDoc = Convert.ToInt32(lAnswer[3]);
                    return true;
                }
                
                return false;
            }
            return false;
        }

         /// <summary>
        /// Получает диапозон документов пробитых в заданном промежутке времени.
        /// </summary>
        /// <param name="startData">>DD-MM-YY< Дата с включительно. Если пусто, то значение устанавливается как дата фискализации</param>
        /// <param name="endDate">>DD-MM-YY< Дата по включительно. Если пусто, то значение устанавливается как дата последнего фискального документа</param>
        /// <returns></returns>
        public bool GetDocNumberByData(string startData, string endDate)
        {
            return GetDocNumberByDataTime(startData + " 00:00:00", endDate+" 23:59:59");
        }

        public bool ReadDoc()
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

        /// <summary>
        /// Преабразует массив byte находящийся в переменной ansfer - массив строк
        /// </summary>
        /// <returns>Массив строк.</returns>
        private List<string> Separating()
        {
            List<string> lPlu = new List<string>();
            String temp = String.Empty;
            for (int i = 8; i < answerlenght; i++) // перебор массива ответа
            {
                if (answer[i] != 09) // не встретили сепаратор
                {
                    temp += ASCIIEncoding.Default.GetString(answer, i, 1);
                }
                else // втретили сепаратор - значит конец строки. 
                {
                    lPlu.Add(temp);
                    temp = String.Empty;      
                }
            }
            return lPlu;
        }
        /// <summary>
        /// задает документ для чтения
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public bool SetDocForRead(int docNumber)
        {// 125 | 1 | docNum
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 53, 09, 48, 09};
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (Send(sendBytes))
            {
                Console.WriteLine(Encoding.Default.GetString(answer, 7, answerlenght));
                return true;
            }

            Console.WriteLine(Encoding.Default.GetString(answer, 7, answerlenght));
            return false;
        }

        


#endregion Privat methods

    } // class ECR
} // namespace DP_25_ole
