﻿using System;
using System.Runtime.InteropServices;

namespace Delphin
{
    [Guid("bc9d5f55-062c-42ec-8b0f-1c53967e5aee")]
    internal interface IECR
    {

        [DispId(1)]

        // описываем методы которые можно будет вызывать из вне
       // bool Connect(string port, int speed, int logNum);
       // bool Connect(string port, int speed);
        bool Disconnect();
        bool Beep(int tone, int len);
        PLU ReadPlu(int pluCode);
        bool DeletingPlu(int firstPlu, int lastPlu);
        bool DeletingPlu(int plu);

        bool WritePlu(int plu, byte taxGr, byte dep, byte group, byte priceType, double price, double addQty, double quantity,
                                string bar1, string bar2, string bar3, string bar4, string name, int connectedPLU);

        string GetDataTime();
        bool SetDataTime(string dataTime);

        Check GetCheckByNum(int docNumber);

        int GetFirstDocNumberByDate(string dateIn, string dateOut);
        int GetFirstDocNumberByDate(string date);

        int GetLastDocNumber();
        


        
    }
}
