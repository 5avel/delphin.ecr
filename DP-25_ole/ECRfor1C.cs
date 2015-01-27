using System;

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
                ArtBarCode = art.Bar1;
                ArtSaleQnty = art.SoldQty;
                ArtSaleSum = art.Turnover;
                return true;
            }
                ArtCode = 0;
                ArtName = "";
                ArtPrice = 0;
                ArtQnty = 0;
                ArtDep = 0;
                ArtGrp = 0;
                ArtTax = 0;
                ArtBarCode = "";
                ArtSaleQnty = 0;
                ArtSaleSum = 0;
            return false;
        }


        /// <summary>
        /// Функция предназначена для программирования артикула в кассовый аппарат.
        /// Максимальное количество артикулов зависит от модели РРО. См. подробнее 
        /// описание Таблица 1 инструкции кассового аппарата.
        /// </summary>
        /// <param name="Code">целое число, код артикула по которому будет осуществляться продажа товара. 
        /// Рекомендуется, чтобы номер ряда и код совпадали, во избежание путаницы.</param>
        /// <param name="Name">строка, наименование товара. Максимальная длина наименования зависит от модели РРО.</param>
        /// <param name="Price">вещественное число, цена товара.</param>
        /// <param name="Qnty">вещественное число, количество товара.</param>
        /// <param name="Dep">целое число, номер отдела.</param>
        /// <param name="Grp">целое число, номер группы товара.</param>
        /// <param name="Tax">целое число, номер налоговой группы товара.</param>
        /// <param name="BarCode">строка, штрих-код товара (до 13 цифр)</param>
        /// <returns>Возвращаемое значение: логического типа, ИСТИНА, если функция выполнена успешно, ЛОЖЬ – если возникла ошибка. 
        /// Возвращаемое значение можно также прочитать функцией GetLastError, которая возвращает результат выполнения последней 
        /// функции (0-успешно, 1-ошибка).</returns>
        public bool WriteArticul(int Code, string Name, double Price, double Qnty, byte Dep, byte Grp, byte Tax, string BarCode)
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


        

        private Check check = null;
        private int LastCheckNum = 0;
        private int CurentCheckNum = 0;
        private int CurentCheckLine = 0;

        /*
        
         *  JCheckNum – целое число, номер фискального чека
            JCheckIsReturn – true - Чек возврата, false - Продажный чек.
         *  JCheckIsVoid - true - Чек отменен, false - не отменен.
            JCheckDate - строка, дата чека   
            JCheckDis - вещественное число, процент скидки на весь Чек 0.00…99.99 (отрицательный - скидка, положительная – надбавка) 
         * 
            JArtCode – целое число, код артикула
            JArtDep – целое число, номер отдела
            JArtTax – целое число, номер налоговой группы
            JArtGrp – целое число, номер товарной группы
            JArtVoid – целое число, признак аннулирования товара (0-артикул продан, 1-артикул продан, а потом аннулирован)
            JArtPrice – вещественное число, цена проданного артикула (без учета скидки)
            JArtQnt – вещественное число, количество проданного артикула
            JArtDis– – вещественное число, процент скидки на товар 0.00…99.99 (отрицательный - скидка, положительная – надбавка) 
         */
        public string DataSales { set; get; }

        public uint JCheckNum { private set; get; }
        public bool JCheckIsReturn { private set; get; }
        public bool JCheckIsVoid { private set; get; }
        public string JCheckDate { private set; get; }
        public double JCheckDis { private set; get; }

        public uint JArtCode { private set; get; }
        public bool JArtVoid { private set; get; }
        public double JArtPrice { private set; get; }
        public double JArtQnt { private set; get; }
        public double JArtSum { private set; get; }
        public double JArtDis { private set; get; }
        public string JArtName { private set; get; }

        public bool ReadSales()
        {
            if(CurentCheckLine <= check.goods.Count-1)
            {
                //Заполняем переменные и увелисиваем счетчик
                JArtCode = check.goods[CurentCheckLine].code;
                JArtVoid = check.goods[CurentCheckLine].isVoid;
                JArtPrice = check.goods[CurentCheckLine].price;
                JArtQnt = check.goods[CurentCheckLine].quantity;
                JArtSum = check.goods[CurentCheckLine].sum;
                JArtDis = check.goods[CurentCheckLine].discSurc;
                JArtName = check.goods[CurentCheckLine].name;

                ++CurentCheckLine;
                return true;
            }
            CurentCheckLine = 0;
            return false;
        }

        public bool GetCheck()
        {
            if (CurentCheckNum == 0) // начало загрузки
            {
                int num = ecr.GetFirstDocNumberByDate(DataSales);
                if (num > 0)
                {
                    CurentCheckNum = num;
                    LastCheckNum = ecr.GetLastDocNumber();
                }
            }
            check = ecr.GetCheckByNum(CurentCheckNum++); // Получаем чек по номеру
            while (check == null) // если не чек, то пробуем следующий - пока не будет чек.
            {
                check = ecr.GetCheckByNum(CurentCheckNum++);
                if (CurentCheckNum >= LastCheckNum) break; // Чеки кончились
            }
            if(check != null) // если чек
            {
                if (check.dateTime > DateTime.Parse(DataSales + " 23:59:59")) return false; // Чек старше "заданной даты" 23:59:59
                JCheckNum = check.num;
                JCheckIsReturn = check.isReturnCheck;
                JCheckIsVoid = check.isVoidCheck;
                JCheckDate = check.dateTime.ToString();
                JCheckDis = check.discSurc;
                return true;
            }
            return false;
        }
        

        public bool Beep(int Tone, int Leng)
        {
            return ecr.Beep(Tone, Leng);
        }

    }
}
