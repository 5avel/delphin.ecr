using Delphin;
using System;
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class Program
    {/// test comita
        static void Main(string[] args)
        {


            ECRfor1C ecr1c = new ECRfor1C();



            Console.WriteLine(ecr1c.Connect(3, 115200));

            //ecr.Beep(1000, 1000);

            //PLU p1 = ecr.ReadPlu(1);
            //PLU p2 = ecr.ReadPlu(2);
            //PLU p3 = ecr.ReadPlu(3);
            

            //ecr.WritePlu(4, 1, 15.55, "Ура работает");

            //PLU p4 = ecr.ReadPlu(4);

            //Console.WriteLine(p1.Name + "  " + p2.Name + "  " + p3.Name + "  " + p4.Name + "  ");

            ecr1c.DataSalesFrom = "28-07-15";
            ecr1c.DataSalesTo = "28-07-15";
            while (ecr1c.GetCheck())
            {
                Console.WriteLine("\n Чек - " + ecr1c.JCheckNum + "  Дата - " + ecr1c.JCheckDate + "\n  Скидка на чек " + ecr1c.JCheckDis + "%" + " Возврат - " + ecr1c.JCheckIsReturn + " Отменен -" + ecr1c.JCheckIsVoid + " Z -" + ecr1c.JCheckNumZRep);
                while (ecr1c.ReadSales())
                {
                    Console.WriteLine("\t" + ecr1c.JArtCode + " " + ecr1c.JArtName + " " + ecr1c.JArtPrice + " " + ecr1c.JArtQnt + " " + ecr1c.JArtSum + " " + ecr1c.JArtDis + " " + ecr1c.JArtVoid);
                }
                Console.WriteLine("Количество Оплат = " + ecr1c.JCheckPayCount);
                Console.WriteLine("ВИД Оплаты1 - " + ecr1c.JCheckPay1Type);
                Console.WriteLine("Сумма Оплаты1 - " + ecr1c.JCheckPay1Sum);
                Console.WriteLine("ВИД Оплаты2 - " + ecr1c.JCheckPay2Type);
                Console.WriteLine("Сумма Оплаты2 - " + ecr1c.JCheckPay2Sum);
                Console.WriteLine("ВИД Оплаты3 - " + ecr1c.JCheckPay3Type);
                Console.WriteLine("Сумма Оплаты3 - " + ecr1c.JCheckPay3Sum);
                Console.WriteLine("Сумма Чека - " + ecr1c.JCheckSum);
                Console.WriteLine("Tax1 - " + ecr1c.JCheckTax1Sum);
                Console.WriteLine("Tax1Zbir - " + ecr1c.JCheckTax1Zbir);

            }

             


            Console.ReadKey();
            Console.WriteLine(ecr1c.Disconnect());
        } // Main
    } 
}


