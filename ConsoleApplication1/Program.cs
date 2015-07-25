using Delphin;
using System;
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class Program
    {/// test comita
        static void Main(string[] args)
        {


            ECR ecr = new ECR();

            

            Console.WriteLine(ecr.Connect(3, 115200));

            //ecr.Beep(1000, 1000);

            //PLU p1 = ecr.ReadPlu(1);
            //PLU p2 = ecr.ReadPlu(2);
            //PLU p3 = ecr.ReadPlu(3);
            

            //ecr.WritePlu(4, 1, 15.55, "Ура работает");

            //PLU p4 = ecr.ReadPlu(4);

            //Console.WriteLine(p1.Name + "  " + p2.Name + "  " + p3.Name + "  " + p4.Name + "  ");

            Console.WriteLine(ecr.GetCheckByNum(3));

             


            Console.ReadKey();
            Console.WriteLine(ecr.Disconnect());
        } // Main
    } 
}


