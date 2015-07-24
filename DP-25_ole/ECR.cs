using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Delphin
{
    [Guid("a0cc9128-46fc-4bf7-a2b8-76d1e81ae686"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IEvents))]
    public partial class ECR : IECR
    {

#region Public methods

        public bool Connect(int port, int speed = 115200)
        {

            try
            {
                sP = new SerialPort("COM" + port, speed, Parity.None, 8, StopBits.One);
                sP.WriteTimeout = 500; sP.ReadTimeout = 500;

                Thread.Sleep(200);

                sP.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                if (sP.IsOpen == true)
                    sP.Close();

                sP.Open();
                if (sP.IsOpen == true)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Событие появления во входном буфере данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(5); // задежка, что бы дать устройству премя дописать сообщение
            SerialPort sp = (SerialPort)sender;
            answerlenght = sp.BytesToRead;
            answer = new byte[answerlenght];
            sp.Read(answer, 0, answerlenght);
            isAnfer = true;
        }

        public bool Disconnect()
        {
            if (sP.IsOpen == true)
                sP.Close();

            if (sP.IsOpen == false)
            {
                return true;
            }
            return false;
        }

        public bool Beep(int tone, int len)
        {
            if (client.Connected == false) return false; // состояние соединенияя

            byte[] sendBytes = { 05, 17, 00, logNum, 00, 56, 48, 09 };
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(tone.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            sendBytes = sendBytes.Concat(Encoding.ASCII.GetBytes(len.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            return Send(sendBytes);
        }

        public PLU ReadPlu(int pluCode)
        {
            if (client.Connected == false) return null; // состояние соединенияя

            if (DateTime.MinValue == GetDateDocByDocNum(1)) return null; // нет лицензии
                                                                       // R
                                                       //31h 30h 37h 09h 52h 09h  
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 48, 55, 09, 82, 09 };
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
                plu.Bar1 = lPlu[9];
                plu.Bar2 = lPlu[10];
                plu.Bar3 = lPlu[11];
                plu.Bar4 = lPlu[12];
                plu.Name = lPlu[13];
                plu.ConnectedPLU = Convert.ToInt32(lPlu[14]);
               return plu;
            }
            return null;
        }

        public bool DeletingPlu(int firstPlu, int lastPlu)
        {
            if (client.Connected == false) return false; // состояние соединенияя

            if (DateTime.MinValue == GetDateDocByDocNum(1)) return false; // нет лицензии

                                                                       // D
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
        {
            if (client.Connected == false) return false; // состояние соединенияя

            if (DateTime.MinValue == GetDateDocByDocNum(1)) return false; // нет лицензии

                                                                       // P
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

        public string GetDataTime()
        {
            if (client.Connected == false) return null; // состояние соединенияя

            if (DateTime.MinValue == GetDateDocByDocNum(1)) return null; // нет лицензии

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
        {
            if (client.Connected == false) return false; // состояние соединенияя     

            if (DateTime.MinValue == GetDateDocByDocNum(1)) return false; // нет лицензии
 
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
        /// Возвращает номер последнего документа в ЭКЛ.(включая не фмскальные документы.)
        /// </summary>
        /// <returns> int - номер документа.</returns>
        public int GetLastDocNumber()
        {
            if (client.Connected == false) return 0; // состояние соединенияя

            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 52, 09, 09, 09};

            if (DateTime.MinValue == GetDateDocByDocNum(1)) return 0; // нет лицензии

            if (Send(sendBytes))
            {
                //Console.WriteLine(BitConverter.ToString(answer, 0, answerlenght));
              //  Console.WriteLine(Encoding.Default.GetString(answer, 7, answerlenght));
                List<string> lAnswer = Separating();
                if(lAnswer.Count == 4)
                {
                    return Convert.ToInt32(lAnswer[3]);
                }
                
                return 0;
            }
            return 0;
        }

        /// <summary>
        /// Возвращает номер первого документа на заданный диапазон дат.
        /// </summary>
        /// <param name="dateIn">DateTime - Date and time in format: DD-MM-YY<SPACE>hh:mm:ss</param>
        /// <param name="dateOut">DateTime - Date and time in format: DD-MM-YY<SPACE>hh:mm:ss</param>
        /// <returns></returns>
        public int GetFirstDocNumberByDate(string dateIn, string dateOut)
        {
            if (client.Connected == false) return 0; // состояние соединенияя
            int maxDocNum = GetLastDocNumber();
            DateTime dt;
            DateTime dtIn = DateTime.Parse(dateIn);
            DateTime dtOut = DateTime.Parse(dateOut+" 23:59:59");
            DateTime dtFirstDoc = GetDateDocByDocNum(1);

            // первый документ входит в диапозон
            if (dtFirstDoc < dtOut && dtFirstDoc > dtIn) return 1;
            
            // Поиск раньше первого документа
            if (dtFirstDoc > dtIn) return 0;
            
            int ret = 0;
            int step = 10;
            bool flag = true;
            for (int i = maxDocNum; i >= 0; i -= step)
            {
                dt = GetDateDocByDocNum(i);
                if (dt < dtIn)
                {
                    if (flag)
                    {
                        i += step;
                        step = 1;
                        flag = false;
                    }
                    else
                    {
                        dt = GetDateDocByDocNum(i+1);
                        if(dt < dtOut && dt > dtIn)
                        {
                            ret = i + 1;
                        }
                        break;
                    }
                }
            }
            return ret;
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
            if (client.Connected == false) return null; // состояние соединенияя

            List<string> ekl = new List<string>();
            if(SetDocForRead(num))
            {
                string s = ReadDocStr(num);
                while (s != null)
                {
                    ekl.Add(s);
                    s = ReadDocStr(num);
                }
                return ekl;
            }
            return null;            
        }


        /// <summary>
        /// Возвращает объект типа Delphin.Check заданного документа.
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns>Chech - объект чека, или null</returns>
        public Check GetCheckByNum(int docNumber)
        {// 125 | 1 | docNum
            if (client.Connected == false) return null; // состояние соединенияя

            //if (DateTime.MinValue == GetDateDocByDocNum(1)) return null; // нет лицензии

            if (SetDocForRead(docNumber))
            {
                List<string> lStr = Separating();
                var dt = DateTime.Parse(lStr[1].Remove(16));
                int zNum = Int32.Parse(lStr[3]); // Номер Z-отчета
                byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 53, 09, 50, 09 };
                sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
                sendBytes = sendBytes.Concat(SEP).ToArray();
                Check c = null;
                while (Send(sendBytes))
                {
                    string s = ASCIIEncoding.ASCII.GetString(answer, 8, answerlenght - 9);
                    byte[] buf = Convert.FromBase64String(s); // расшифровонная строка
                    //Console.WriteLine(BitConverter.ToString(buf)+"\n");
                    

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
            return null;
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

            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 53, 09, 49, 09 };
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (Send(sendBytes))
            {
                return Encoding.Default.GetString(answer, 7, answerlenght - 8);
            }
            return null;
        }

#endregion Public methods

    } // class ECR
} // namespace DP_25_ole
