using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delphin
{
    class ByteHelper
    {

        public static T[] Merge<T>(T[] mass1, T[] mass2)
        {
            int a = 0, b = 0; // счетчики
            T[] merged = new T[mass1.Length + mass2.Length];
            for (int i = 0; i < mass1.Length + mass2.Length; i++)
            {
                if (a < mass1.Length)
                {
                    merged[i] = mass1[a++];
                }
                else
                {
                    merged[i] = mass2[b++];
                }
            }
            return merged;
        }
    }
}
