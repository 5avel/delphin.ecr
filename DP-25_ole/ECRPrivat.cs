using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Delphin
{
    
    public partial class ECR : IECR
    {
        #region Private Field

        internal SerialPort sP;
        internal TcpClient client = null; // d

        internal bool isAnfer = false; // есть ответ
        internal byte cNum = 32; // порядковый номер соосбщения


        private NetworkStream tcpStream = null;
        private byte logNum = 0;
        private byte[] SEP = { 9 };
        private byte[] answer = new byte[256];
        private int answerlenght = 0;


        #endregion Private Field

        /// <summary>
        /// Метод отправляет массив байт и обрабатывает ответ.
        /// </summary>
        /// <param name="sendBytes"> Массив байт для отправки.</param>
        /// <returns></returns>
        private bool Send(byte[] Send)
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

            WaitinпAnswer();
            if (answerlenght > 8 && answer[5] == 80)
                return true;

            return false;
        }

        private void WaitinпAnswer()
        {
            long t = Environment.TickCount;
            while (!isAnfer) // цикл ожидания ответа с СОМ порта
            {

                if ((Environment.TickCount - t) > 500)
                {
                    Console.WriteLine("Таймаут 500мс");
                    break;
                }
            }

            if (isAnfer)
            {

                Console.WriteLine("Data Received:");
                Console.WriteLine(BitConverter.ToString(answer));
                Console.WriteLine();
            }
            isAnfer = false;
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
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 53, 09, 48, 09 };
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
            byte[] sendBytes = { 05, 17, 00, logNum, 00, 49, 50, 53, 09, 48, 09 };
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
                s += i.ToString();
                j *= 10;
                j += 81;
                s += j.ToString();
                if (s != lStr[10]) return DateTime.MinValue;

                return dt;
            }
            return DateTime.MinValue;
        }
    }
}
