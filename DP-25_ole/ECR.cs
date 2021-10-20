using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Delphin
{
    [Guid("a0cc9128-46fc-4bf7-a2b8-76d1e81ae686"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IEvents))]
    public class ECR : IECR
    {

#region Public methods

        public bool Connect(string ip, int port)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port); // Соединяемся с сервером
                if (!client.Connected) return false;
                tcpStream = client.GetStream();
                byte[] sendBytes = { 5, 5, 1, 0, 1, 0 }; // Open SET
                tcpStream.Write(sendBytes, 0, sendBytes.Length);
                byte[] bytes = new byte[client.ReceiveBufferSize];
                int bytesCount = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);

                if (bytesCount == 0 || bytes[5] != 0)
                {
                    Disconnect();
                    return false;
                }

                return true;
            }
            catch(SocketException ex)
            {
                Disconnect();
                Console.WriteLine("Exception from Connect: " + ex.ToString());
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                if (!client.Connected) return false;
                tcpStream = client.GetStream();
                byte[] sendBytes = { 5, 5, 2, 0, 1, 0 }; // Close SET
                tcpStream.Write(sendBytes, 0, sendBytes.Length);
                byte[] bytes = new byte[client.ReceiveBufferSize];
                int bytesCount = tcpStream.Read(bytes, 0, client.ReceiveBufferSize);
                if (bytesCount == 0 || bytes[5] != 0) return false;
                return true;
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
            if (!client.Connected) return false; // состояние соединенияя

            byte[] sendBytes = { 5, 17, 0, 1, 0, 56, 48, 9 };
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(tone.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(len.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            return Send(sendBytes);
        }



        public PLU ReadPlu(int pluCode)
        {
            if (client.Connected == false) return null; // состояние соединенияя

                                                                       // R
                                                       //31h 30h 37h 09h 52h 09h  
            byte[] sendBytes = { 5, 17, 0, 1, 00, 49, 48, 55, 9, 82, 9 };
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(pluCode.ToString())).ToArray(); // 31h
            sendBytes = sendBytes.Concat(SEP).ToArray(); // 09h
            if(Send(sendBytes) && answerlenght > 8)
            {
                PLU plu = new PLU();
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
                plu.BarX = lPlu[9];
                plu.Name = lPlu[13];
                plu.FractionalQty = lPlu[14];
                plu.CustomCode = lPlu[15];
               return plu;
            }
            return null;
        }

        public bool DeletingPlu(int firstPlu, int lastPlu)
        {
            if (!client.Connected) return false; // состояние соединенияя
                                                                       // D
                                                       //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 5, 17, 0, 1, 0, 49, 48, 55, 9, 68, 9 };
            
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

        public bool WritePlu(   int plu, byte taxGr, byte dep, byte group, byte priceType, string price, double addQty,
                                double quantity, string barX, string name, int fractionalQty, string customCode)
        {
            if (client.Connected == false) return false; // состояние соединенияя

                                                                       //107 = 49 48 55
                                                                       // 80 = P
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
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(price.ToString().Replace(',','.'))).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (addQty < 0)
            {
                sendBytes = sendBytes.Concat(Encoding.Default.GetBytes($"{addQty}.00".ToString().Replace(',', '.'))).ToArray();
            }
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(quantity.ToString().Replace(',', '.'))).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();

            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(barX.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes("0".ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes("0".ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes("0".ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(name.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(fractionalQty.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(customCode.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(0.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();






            Console.WriteLine();

            List<string> lStr = new List<string>();
            String temp = String.Empty;
            for (int i = 11; i < sendBytes.Length; i++) // перебор массива ответа
            {
                Console.Write($"{ASCIIEncoding.Default.GetString(sendBytes, i, 1)}");
                if (sendBytes[i] != 09) // не встретили сепаратор
                {
                    temp += ASCIIEncoding.Default.GetString(sendBytes, i, 1);
                }
                else // втретили сепаратор - значит конец строки. 
                {
                    lStr.Add(temp);
                    temp = String.Empty;
                }
            }




            if (Send(sendBytes))
            {
                return true;
            }
            return false;
        }


        public string GetDataTime()
        {
            if (!client.Connected) return null; // состояние соединенияя
            //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 5, 17, 0, 1, 0, 54, 50, 9};
            if (!Send(sendBytes)) return null;
            return Encoding.Default.GetString(answer, 7, 30);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dstsTime">DateTime - Date and time in format: DD-MM-YY<SPACE>hh:mm:ss<SPACE>DST</param>
        /// <returns></returns>
        public bool SetDataTime(string dataTime)
        {
            if (client.Connected == false) return false; // состояние соединенияя   
            //31h 30h 37h 09h 44h 09h  
            byte[] sendBytes = { 5, 17, 0, 1, 0, 54, 49, 9 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(dataTime)).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (!Send(sendBytes)) return false;
            return true;
        }

        /// <summary>
        /// Возвращает номер последнего документа в ЭКЛ.(включая не фмскальные документы.)
        /// </summary>
        /// <returns> int - номер документа.</returns>
        public int GetLastDocNumber()
        {
            if (client.Connected == false) return 0; // состояние соединенияя
            byte[] sendBytes = { 5, 17, 0, 1, 0, 49, 50, 52, 9, 9, 9};
            if (!Send(sendBytes)) return 0;
            List<string> lAnswer = Separating();
            if (lAnswer.Count != 4) return 0;
            return Convert.ToInt32(lAnswer[3]); 
        }

        public List<string> SearchReceipt(string dateIn, string dateOut)
        {
            byte[] sendBytes = { 5, 17, 0, 1, 0, 49, 50, 52, 9, };  
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(dateIn)).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(dateOut)).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (!Send(sendBytes)) return null;
            List<string> lAnswer = Separating();
            Console.WriteLine($"lAnswer Count {lAnswer.Count}");
            if (lAnswer.Count != 4) return null;
            return lAnswer;
        }

        /// <summary>
        /// Возвращает номер первого документа на заданный диапазон дат.
        /// </summary>
        /// <param name="dateIn">DateTime - Date and time in format: DD-MM-YY<SPACE>hh:mm:ss</param>
        /// <param name="dateOut">DateTime - Date and time in format: DD-MM-YY<SPACE>hh:mm:ss</param>
        /// <returns></returns>
        public int GetFirstDocNumberByDate(string dateIn, string dateOut)
        {
            List<string> lS = SearchReceipt(dateIn + " 00:00:00", dateOut+" 23:59:59");
            if (lS == null) return 0;
            return Convert.ToInt32(lS[2]);
        }

        /// <summary>
        /// Возвращает номер первого документа на заданную дату.
        /// </summary>
        /// <param name="date">DateTime - Date and time in format: DD-MM-YY<SPACE>hh:mm:ss</param>
        /// <returns></returns>
        public int GetFirstDocNumberByDate(string date)
        {
            return GetFirstDocNumberByDate(date+" 00:00:00", date+" 23:59:59");
        }

        /// <summary>
        /// Возвращает заданный документ в текстовом виде.
        /// </summary>
        /// <param name="num">Номер документа.</param>
        /// <returns>Список строк документа. В случаее ошибки вернет NULL.</returns>
        public List<string> GetDocTxtByNum(int num)
        {
            if (!client.Connected) return null; // состояние соединенияя
            List<string> ekl = new List<string>();
            if (!SetDocForRead(num)) return null;
            
                string s = ReadDocStr(num);
                while (s != null)
                {
                    ekl.Add(s);
                    s = ReadDocStr(num);
                }
                return ekl;           
        }


        /// <summary>
        /// Возвращает объект типа Delphin.Check заданного документа.
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns>Chech - объект чека, или null</returns>
        public Check GetCheckByNum(int docNumber)
        {// 125 | 1 | docNum
            if (!client.Connected) return null; // состояние соединенияя
            if (!SetDocForRead(docNumber)) return null;

            List<string> lStr = Separating();
            var dt = DateTime.Parse(lStr[1].Remove(16));
            int zNum = Int32.Parse(lStr[3]); // Номер Z-отчета
            byte[] sendBytes = { 5, 17, 0, 1, 0, 49, 50, 53, 9, 50, 9 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            Check c = null;
            while (Send(sendBytes))
            {
                string s = ASCIIEncoding.ASCII.GetString(answer, 8, answerlenght - 9);
                byte[] buf = Convert.FromBase64String(s); // расшифровонная строка
                if (buf[0] == 99) // Начало чека
                {
                    if (buf[4] == 0) // Чек
                    {
                        c = new Check(dt, BitConverter.ToUInt32(buf, 8), zNum);
                    }
                    else if (buf[4] == 1) // Возвратный Чек
                    {
                        c = new Check(dt, BitConverter.ToUInt32(buf, 8), zNum, true);
                    }
                }
                else if (buf[0] == 01 && c != null) // Продажа
                {
                    uint code = BitConverter.ToUInt32(buf, 24); // код товара

                    string name = Encoding.Default.GetString(buf, 44, 32); // название товара
                    name = name.Substring(0, name.IndexOf('\0') > 0 ? name.IndexOf('\0') : 32);
                    double price = Convert.ToDouble(BitConverter.ToUInt32(buf, 8)) / 100; // цена
                    double quantity = Convert.ToDouble(BitConverter.ToUInt32(buf, 40)) / 1000; // количество
                    double sum = Convert.ToDouble(BitConverter.ToUInt64(buf, 16)) / 100; // сумма
                    c.AddGood(code, price, quantity, sum, name); // добавляем товар в чек
                }
                else if (buf[0] == 04) // скидка надбавка
                {
                    if (buf[8] == 01) // на Весь Чек
                    {
                        if (buf[6] == 01) // надбавка
                        {
                            double proc = Convert.ToDouble(BitConverter.ToUInt64(buf, 104)) / 100;
                            // надбавка на Весь Чек  
                            c.discSurc = proc; 
                        }
                        else if (buf[6] == 02) // скидка
                        {
                            double proc = Convert.ToDouble(BitConverter.ToUInt64(buf, 104)) / 100;
                            // скидка на Весь Чек
                            c.discSurc = -proc;
                        }
                    }
                    else // на товар
                    {
                        if (buf[6] == 01) // надбавка
                        {
                            double proc = Convert.ToDouble(BitConverter.ToUInt64(buf, 104)) / 100;
                            // надбавка на последний товар, на текущий момент.
                            c.goods.Last<Good>().discSurc = proc;
                        }
                        else if (buf[6] == 02) // скидка
                        {
                            double proc = Convert.ToDouble(BitConverter.ToUInt64(buf, 104)) / 100;
                            double test = Convert.ToDouble(BitConverter.ToUInt64(buf, 24)) / 100;
                            // скидка на последний товар, на текущий момент.
                            c.goods.Last<Good>().discSurc = -proc;
                        }
                    }
                }
                else if (buf[0] == 08) // Корекция, Отмена внутри чека
                {
                    uint code = BitConverter.ToUInt32(buf, 24); // код товара
                    string name = Encoding.Default.GetString(buf, 44, 32); // название товара
                    double price = Convert.ToDouble(BitConverter.ToUInt32(buf, 8)) / 100; // цена
                    double quantity = Convert.ToDouble(BitConverter.ToUInt32(buf, 40)) / 1000; // количество
                    double sum = Convert.ToDouble(BitConverter.ToUInt64(buf, 16)) / 100; // сумма

                    if(c.goods.Last().code == code)
                    {
                        c.goods.Last().isVoid = true;     // отменен                   
                    }
                    else
                    {
                        foreach(var g in c.goods)
                        {
                            if(g.code == code)
                            {
                                g.isVoid = true;
                                break;
                            }
                        }
                    }

                    //c.AddGood(code, price, -quantity, -sum, name, true);
                }
                else if (buf[0] == 03) // Оплата
                {
                    byte type = buf[4];
                    double pay = Convert.ToDouble(BitConverter.ToUInt64(buf, 16)) / 100;
                    double change = Convert.ToDouble(BitConverter.ToUInt64(buf, 24)) / 100;
                    c.AddPayment(type, pay, change);
                }
                else if(buf[0] == 10) // Отмена скидки надбавки на весь чек
                {
                    if (buf[8] == 01) // на Весь Чек
                    {
                        if (buf[6] == 01) // надбавка
                        {
                            double proc = Convert.ToDouble(BitConverter.ToUInt64(buf, 104)) / 100;
                            // надбавка на на все товары чека до текущего момента
                            c.discSurc = 0; // отнимаем надбавку от каждого товара
                        }
                        else if (buf[6] == 02) // скидка
                        {
                            double proc = Convert.ToDouble(BitConverter.ToUInt64(buf, 104)) / 100;
                            // скидка на на все товары чека до текущего момента
                            c.discSurc += proc; // отнимаем скидку от каждого товара
                        }
                    }
                }
                else if(buf[0] == 18)
                {
                    c.isVoidCheck = true;
                }
                else if(buf[0] == 100)
                {  // налоги
                    double testSum = Convert.ToDouble(BitConverter.ToUInt64(buf, 8)) / 100;
                    double tax1sum = Convert.ToDouble(BitConverter.ToUInt64(buf, 16)) / 100;
                    double zbir1sum = Convert.ToDouble(BitConverter.ToUInt64(buf, 24)) / 100;
                    double checkSum = Convert.ToDouble(BitConverter.ToUInt64(buf, 40)) / 100;
                    if (c.CheckTax1ZbirSam == 0 && checkSum != 0)
                    {
                        c.CheckSum = checkSum;
                        c.CheckTax1Sam = tax1sum;
                        c.CheckTax1ZbirSam = zbir1sum;
                    }
                }
            }
            return c; // Возврат Чека
        }

        /// <summary>
        /// Возвращает оодну стоку заданного документа документа за вызов
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns>string - одна строка из документа</returns>
        public string ReadDocStr(int docNumber)
        {// 125 | 1 | docNum
            if (client.Connected == false) return null; // состояние соединенияя

            if (DateTime.MinValue == GetDateDocByDocNum(1)) return null; // нет лицензии

            byte[] sendBytes = { 5, 17, 0, 1, 00, 49, 50, 53, 9, 49, 9 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (Send(sendBytes))
            {
                return Encoding.Default.GetString(answer, 7, answerlenght - 8);
            }
            return null;
        }

        #endregion Public methods

        #region Private Field


        internal TcpClient client = null;
        private NetworkStream tcpStream = null;
        private byte[] SEP = { 9 };
        private byte[] answer = new byte[256];
        private int answerlenght = 0;


        #endregion Private Field

        /// <summary>
        /// Метод отправляет массив байт и обрабатывает ответ.
        /// </summary>
        /// <param name="sendBytes"> Массив байт для отправки.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Преабразует массив byte находящийся в переменной ansfer - массив строк
        /// </summary>
        /// <returns>Массив строк.</returns>
        private List<string> Separating()
        {
            Console.WriteLine();

            List<string> lStr = new List<string>();
            String temp = String.Empty;
            for (int i = 8; i < answerlenght; i++) // перебор массива ответа
            {
                Console.Write($"{ASCIIEncoding.Default.GetString(answer, i, 1)}");
                if (answer[i] != 09) // не встретили сепаратор
                {
                    temp += ASCIIEncoding.Default.GetString(answer, i, 1);
                }
                else // втретили сепаратор - значит конец строки. 
                {
                    lStr.Add(temp);
                    temp = String.Empty;
                }
            }
            return lStr;
        }

        /// <summary>
        /// задает документ для чтения
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns>bool</returns>
        private bool SetDocForRead(int docNumber)
        {// 125 | 1 | docNum
            byte[] sendBytes = { 5, 17, 0, 1, 0, 49, 50, 53, 9, 48, 9 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (!Send(sendBytes)) return false;
            return true;
        }

        /// <summary>
        /// Возвращает дату документа с заданным номером.
        /// </summary>
        /// <param name="docNumber">Номер документа.</param>
        /// <returns>DateTime - Дата документа.</returns>
        public DateTime GetDateDocByDocNum(int docNumber)
        {
            byte[] sendBytes = { 5, 17, 0, 1, 0, 49, 50, 53, 9, 48, 9 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (!Send(sendBytes)) return DateTime.MinValue;
            List<string> lStr = Separating();
            var dt = DateTime.Parse(lStr[1].Remove(16));
            return dt;
        }

    } // class ECR
} // namespace DP_25_ole
