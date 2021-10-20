using System;
using System.Runtime.InteropServices;

namespace Delphin
{
    [Guid("bc9d5f55-062c-42ec-8b0f-1c53967e5aee")]
    internal interface IECR
    {

        [DispId(1)]

        // описываем методы которые можно будет вызывать из вне
        bool Connect(string ip, int port);
        bool Disconnect();
        bool Beep(int tone, int len);
        PLU ReadPlu(int pluCode);
        bool DeletingPlu(int firstPlu, int lastPlu);

        bool WritePlu(int plu, byte taxGr, byte dep, byte group, byte priceType, string price, double addQty,
                                double quantity, string barX, string name, int fractionalQty, string customCode);

       


        string GetDataTime();
        bool SetDataTime(string dataTime);

        Check GetCheckByNum(int docNumber);
        


        
    }
}
