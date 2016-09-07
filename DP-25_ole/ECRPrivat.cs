using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Delphin
{
    
    public partial class ECR : IECR
    {
        #region Private Field

        
        internal TcpClient client = null;
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
                        Console.WriteLine(BitConverter.ToString(answer, 0, answerlenght));
                        Console.WriteLine(Encoding.Default.GetString(answer, 7, answerlenght));
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
            List<string> lStr = new List<string>();
            String temp = String.Empty;
            

            for (int i = 8; i < answerlenght; i++) // перебор массива ответа
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
               // s += i.ToString();
                j *= 10;
                j += 81;
                j += 1391;
                int j2 = j + 3210;
                string s2 = s;
                s += j.ToString();
                s2 += j2.ToString();

                

                if ("КР00006552" == lStr[10]    // винил
                    || "КР00006557" == lStr[10] // винил
                    || "КР00006512" == lStr[10] // тест
                    || "КР00005082" == lStr[10] // Кобец Херсон
                    || "КР00001872" == lStr[10] // Кобец Днепр
                    )  return dt;
   
            }
            return DateTime.MinValue;
        }
    }
}
