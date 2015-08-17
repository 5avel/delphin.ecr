using System;
using System.Collections.ObjectModel;

namespace Delphin
{
    public class Good
    {
        public uint code { get; set; }
        public double price { get; set; }
        public double quantity { get; set; }
        public double sum { get; set; }
        public double discSurc { get; set; }
        public double discSum { get; set; }
        public string name { get; set; }
        public bool isVoid { get; set; }

        internal Good(uint code, double price, double quantity, double sum, string name, bool isVoid)
        {
            this.code = code;
            this.price = price;
            this.quantity = quantity;
            this.sum = sum;
            this.discSurc = 0;
            this.name = name;
            this.isVoid = isVoid;
        }

    }

    public class Payment
    {
        public byte type { get; set; } // Тип оплаты 0 - нал, 1 - карта, 2- кредит и тд.
        public double pay { get; set; } // сумма платежа
        public double change { get; set; }   // сжача
        public double sum { get; set; }     // сумма 

        internal Payment(byte type, double pay, double change)
        {
            this.type = type;
            this.pay = pay;
            this.change = change;
            this.sum = pay - change;
        }
    }
   

    public class Check
    {
        public DateTime dateTime { get; set; }
        public uint num { get; set; }
        public int zNumber { get; set; }
        public bool isReturnCheck { get; set; }
        public bool isVoidCheck { get; set; }
        public Collection<Good> goods { get; set; }
        public Collection<Payment> payments { get; set; }
        public double discSurc { get; set; }
        public double CheckSum { get; set; }
        public double CheckTax1Sam { get; set; }
        public double CheckTax1ZbirSam { get; set; }

        internal Check(DateTime dt, uint num, int zNumber, bool isReturnCheck = false)
        {
            this.dateTime = dt;
            this.num = num;
            this.zNumber = zNumber;
            this.isReturnCheck = isReturnCheck;
            this.goods = new Collection<Good>();
            this.payments = new Collection<Payment>();
        }

        public void AddGood(uint code, double price, double quantity, double sum, string name, bool isCanceled = false)
        {
            Good g = new Good(code, price, quantity, sum, name, isCanceled);
            this.goods.Add(g);
        }

        public void AddPayment(byte type, double pay, double change)
        {
            Payment p = new Payment(type, pay, change);
            this.payments.Add(p);
        }

    }
}
