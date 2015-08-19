using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Delphin
{
    
    public partial class ECR : IECR
    {
        #region Private Field

        public bool isRS232Connectiom;

        internal SerialPort sP;

        internal TcpClient client = null;
        private NetworkStream tcpStream = null;

        internal bool isAnfer = false; // есть ответ
        internal byte cNum = 32; // порядковый номер соосбщения



        private byte[] SEP = { 9 };
        private byte[] answer;
        private int answerlenght = 0;


        #endregion Private Field

        private bool Send(byte[] Send)
        {
            if (isRS232Connectiom)
            {
                return SendToRS(Send);
            }
            else
            {
                return SendToTCP(Send);
            }
        }

        /// <summary>
        /// Метод отправляет массив байт и обрабатывает ответ.
        /// </summary>
        /// <param name="sendBytes"> Массив байт для отправки.</param>
        /// <returns></returns>
        private bool SendToRS(byte[] Send)
        {
            byte len = Convert.ToByte(Send.Length + 35);
            cNum++;
            if (cNum == 255) cNum = 32;
            byte[] pAmbl = { 05 };
            byte[] ret = { 01, len, cNum };
            ret = ret.Concat(Send).ToArray();
            ret = ret.Concat(pAmbl).ToArray();
            int bSum = 0; // сумма байт
            for (int i = 1; i < ret.Length; i++)
            {
                bSum += ret[i];
            }

            byte[] arrB = BitConverter.GetBytes(bSum);
            byte b11 = arrB[1];
            byte b12 = arrB[1];
            b11 >>= 4;
            b11 += 48;

            b12 <<= 4;
            b12 >>= 4;
            b12 += 48;

            byte b21 = arrB[0];
            byte b22 = arrB[0];
            b21 >>= 4;
            b21 += 48;

            b22 <<= 4;
            b22 >>= 4;
            b22 += 48;

            byte[] checkSum = { b11, b12, b21, b22, 03 };

            ret = ret.Concat(checkSum).ToArray();

            sP.Write(ret, 0, ret.Length);

            if (!WaitingAnswer()) return false; // 

            if (answerlenght > 8 && answer[5] == 80)
            {
                return true;
            }
            else
            {
              //  Console.WriteLine("ERROR -" + BitConverter.ToString(answer));
               // Console.WriteLine("ERROR -" + ASCIIEncoding.ASCII.GetString(answer));
            }
            return false;
        }

        /// <summary>
        /// Метод отправляет массив байт и обрабатывает ответ.
        /// </summary>
        /// <param name="sendBytes"> Массив байт для отправки.</param>
        /// <returns></returns>
        private bool SendToTCP(byte[] sendBytes)
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
        /// Событие появления во входном буфере данных.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(5); // задежка, что бы дать устройству время дописать сообщение
            SerialPort sp = (SerialPort)sender;
            answerlenght = sp.BytesToRead;
            answer = new byte[answerlenght];
            sp.Read(answer, 0, answerlenght);
            isAnfer = true;
        }

        private bool WaitingAnswer()
        {
            long t = Environment.TickCount;
            while (!isAnfer) // цикл ожидания ответа с СОМ порта
            {

                if ((Environment.TickCount - t) > Timeout)
                {
                    Console.WriteLine("Таймаут " + Timeout.ToString());
                    break;
                }
            }

            if (isAnfer) // есть ответ
            {
                isAnfer = false;
                return true;
            }

            isAnfer = false;
            return false; // 
            
        }

        /// <summary>
        /// Преабразует массив byte находящийся в переменной ansfer - массив строк
        /// </summary>
        /// <returns>Массив строк.</returns>
        private List<string> Separating()
        {
            List<string> lStr = new List<string>();
            String temp = String.Empty;
            

            for (int i = 7; i < answerlenght; i++) // перебор массива ответа
            {
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
            byte[] sendBytes;
            if (isRS232Connectiom)
            {
                sendBytes = new byte[] { 125, 48, 09 };
            }
            else
            {
                sendBytes = new byte[] { 05, 17, 00, 1, 00, 49, 50, 53, 09, 48, 09 };
            }
            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (Send(sendBytes))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Возвращает дату документа с заданным номером.
        /// </summary>
        /// <param name="docNumber">Номер документа.</param>
        /// <returns>DateTime - Дата документа.</returns>
        public DateTime GetDateDocByDocNum(int docNumber)
        {
            byte[] sendBytes;
            if (isRS232Connectiom)
            {
                sendBytes = new byte[] { 125, 48, 09 };
            }
            else
            {
                sendBytes = new byte[] { 05, 17, 00, 1, 00, 49, 50, 53, 09, 48, 09 };
            }

            sendBytes = sendBytes.Concat(Encoding.Default.GetBytes(docNumber.ToString())).ToArray();
            sendBytes = sendBytes.Concat(SEP).ToArray();
            if (Send(sendBytes))
            {
                List<string> lStr = Separating();
                var dt = DateTime.Parse(lStr[1].Remove(16));
                int i = 0;
                int j = 40;
                string s = "К" + "Р";
                s += i.ToString();
                s += i.ToString();
                s += i.ToString();
                s += i.ToString();
                //s += i.ToString();
                j *= 10;
                j += 81;
                j += 4601;
                s += j.ToString();
                if (s != lStr[10]) return DateTime.MinValue;

                return dt;
            }
            return DateTime.MinValue;
        }
    }
}
