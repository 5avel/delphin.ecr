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

            

          



            Console.ReadKey();
            Console.WriteLine(ecr.Disconnect());
        } // Main
    } 
}


