using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delphin
{
    public class ECRfor1C
    {
        private ECR ecr;
        public ECRfor1C()  // Реализовать сингтон
        {
            ecr = new ECR();
        }

        public bool Connect(string ip, int port, byte logNum)
        {
            return ecr.Connect(ip, port, logNum);
        }

        public bool Disconnect()
        {
            return ecr.Disconnect();
        }


        public int ArtCode { private set; get; }
        public string ArtName { private set; get; }
        public double ArtPrice { private set; get; }
        public double ArtQnty { private set; get; }
        public byte ArtDep { private set; get; }
        public byte ArtGrp { private set; get; }
        public byte ArtTax { private set; get; }
        public uint ArtNC { private set; get; }
        public uint ArtNK { private set; get; }
        public string ArtBarCode { private set; get; }
        public double ArtSaleQnty { private set; get; }
        public double ArtSaleSum { private set; get; }
        /// <summary>
        /// Назначение:  Функция предназначена для чтения артикула и данных о продажах по нему из РРО. 
        /// После успешного выполнения данной функции становятся доступны свойства, в которых записана 
        /// информация о прочитанном артикуле.
        /// Свойства доступные (только для чтения) после успешного выполнения функции чтения артикула ReadArticul
        /// ArtCode – код товара (целое число)
        /// ArtName – название товара (строка)
        /// ArtPrice – цена товара (вещественное число)
        /// ArtQnty – количество товара (вещественное число)
        /// ArtDep – номер отдела товара (целое число)
        /// ArtGrp – номер группы товара (целое число)
        /// ArtTax – номер налоговой группы товара (целое число)
        /// ArtNC – признак товара (целое число)
        /// ArtNK – признак ведения остатков товара (целое число)
        /// ArtBarCode – штрих-код товара (строка)
        /// ArtSaleQnty – количество проданного товара (вещественное число)
        /// ArtSaleSum– сумма проданного товара (вещественное число)
        /// </summary>
        /// <param name="Row">Row – целое число, номер ряда таблицы артикулов (Таблица 1 РРО).</param>
        /// <returns>Возвращаемое значение: логического типа, ИСТИНА, если функция выполнена успешно,
        /// ЛОЖЬ – если возникла ошибка. Возвращаемое значение можно также прочитать функцией GetLastError,
        /// которая возвращает результат выполнения последней функции (0-успешно, 1-ошибка).</returns>
        public bool ReadArticul(int Row)
        {
            PLU art = ecr.ReadPlu(Row);
            if(art != null)
            {
                ArtCode = art.Code;
                ArtName = art.Name;
                ArtPrice = art.Price;
                ArtQnty = art.StockQty;
                ArtDep = art.Dep;
                ArtGrp = art.Group;
                ArtTax = art.TaxGr;
                ArtNC = 1;
                ArtNK = 1;
                ArtBarCode = art.Bar1;
                ArtSaleQnty = art.SoldQty;
                ArtSaleSum = art.Turnover;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Функция предназначена для программирования артикула в кассовый аппарат.
        /// Максимальное количество артикулов зависит от модели РРО. См. подробнее 
        /// описание Таблица 1 инструкции кассового аппарата.
        /// </summary>
        /// <param name="Row"> Не используется! целое число, номер ряда таблицы артикулов, в которую будет запрограммирован товар.</param>
        /// <param name="Code">целое число, код артикула по которому будет осуществляться продажа товара. 
        /// Рекомендуется, чтобы номер ряда и код совпадали, во избежание путаницы.</param>
        /// <param name="Name">строка, наименование товара. Максимальная длина наименования зависит от модели РРО.</param>
        /// <param name="Price">вещественное число, цена товара.</param>
        /// <param name="Qnty">вещественное число, количество товара.</param>
        /// <param name="Dep">целое число, номер отдела.</param>
        /// <param name="Grp">целое число, номер группы товара.</param>
        /// <param name="Tax">целое число, номер налоговой группы товара.</param>
        /// <param name="NC">целое число, признак товара</param>
        /// <param name="NK">целое число, ведение остатков товара.</param>
        /// <param name="BarCode">строка, штрих-код товара (до 13 цифр)</param>
        /// <returns>Возвращаемое значение: логического типа, ИСТИНА, если функция выполнена успешно, ЛОЖЬ – если возникла ошибка. 
        /// Возвращаемое значение можно также прочитать функцией GetLastError, которая возвращает результат выполнения последней 
        /// функции (0-успешно, 1-ошибка).</returns>
        public bool WriteArticul(int Row, int Code, string Name, double Price, double Qnty, byte Dep, byte Grp, byte Tax, byte NC, byte NK, string BarCode)
        {
            return ecr.WritePlu(Code, Tax, Dep, Grp, 0, Price, 0, Qnty, BarCode, "", "", "", Name, 0);
        }

        /// <summary>
        ///  Назначение:  Функция предназначена для удаления артикула запрограммированного в кассовый аппарат. 
        ///  Кассовый аппарат удаляет артикул только в том случае, если по данному артикулу не было оборота (продаж).
        ///  Т.е. это или после снятия Z-1 отчета, или после снятия Z-3 отчета, если запрограммировано не обнулять 
        ///  оборот при снятии Z – отчета.
        /// </summary>
        /// <param name="Row"> целое число, номер ряда таблицы артикулов,  из которого будет удален товар.</param>
        /// <returns>Возвращаемое значение: логического типа, ИСТИНА, если функция выполнена успешно, ЛОЖЬ – если возникла ошибка. 
        /// Возвращаемое значение можно также прочитать функцией GetLastError, которая возвращает результат
        /// выполнения последней функции (0-успешно, 1-ошибка).</returns>
        public bool ClearArticul(int Row)
        {
            return ecr.DeletingPlu(Row);
        }

        public double OperName { private set; get; }
        public string OperPas { private set; get; }
        /// <summary>
        /// Назначение:  Функция предназначена для чтения информации об операторе (кассире) из кассового аппарата.
        /// Свойства доступные (только для чтения) после успешного выполнения функции чтения оператора ReadOperator
        /// OperName – BSTR строка, имя оператора (до 20 знаков).
        /// OperPas – целое число, пароль оператора (до 8 цифр).
        /// </summary>
        /// <param name="OperNum">целое число, номер оператора. Может принимать значения 1 - 30.</param>
        /// <returns>Возвращаемое значение: логического типа, ИСТИНА, если функция выполнена успешно, ЛОЖЬ – если возникла ошибка.
        /// Возвращаемое значение можно также прочитать функцией GetLastError, которая возвращает результат выполнения последней 
        /// функции (0-успешно, 1-ошибка). </returns>
        public bool ReadOperator(int OperNum)
        {
            return true;
        }

        /// <summary>
        /// Функция предназначена для записи информации об операторе (кассире) в  кассовый аппарат. 
        /// Перед выполнением необходимо выполнить Z-отчет!
        /// </summary>
        /// <param name="OperNum">целое число, номер оператора. Может принимать значения 1 - 30.</param>
        /// <param name="OperName">строка, имя оператора (до 20 знаков).</param>
        /// <param name="OperPas">целое число, пароль оператора (до 8 цифр).</param>
        /// <returns>Возвращаемое значение: логического типа, ИСТИНА, если функция выполнена успешно, ЛОЖЬ – если возникла ошибка. 
        /// Возвращаемое значение можно также прочитать функцией GetLastError, которая возвращает результат выполнения последней 
        /// функции (0-успешно, 1-ошибка).</returns>
        public bool WriteOperator(int OperNum, string OperName, int OperPas)
        {
            return true;
        }


        public bool Beep(int Tone, int Leng)
        {
            return ecr.Beep(Tone, Leng);
        }

    }
}
