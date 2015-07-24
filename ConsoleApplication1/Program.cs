using Delphin;
using System;
namespace ConsoleApplication1
{
    class Program
    {/// test comita
        static void Main(string[] args)
        {


            ECR ecr = new ECR();

            

            Console.WriteLine(ecr.Connect(3, 115200));

            ecr.Beep(1000, 1000);

            PLU p1 = ecr.ReadPlu(1);
            PLU p2 = ecr.ReadPlu(2);
            PLU p3 = ecr.ReadPlu(3);

            Console.WriteLine(p1.Name+"  "+p2.Name+"  "+p3.Name+"  ");



            Console.ReadKey();
            Console.WriteLine(ecr.Disconnect());
        } // Main
    } 
}


