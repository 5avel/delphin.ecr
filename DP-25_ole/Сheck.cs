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
        public string name { get; set; }

        internal Good(uint code, double price, double quantity, double sum, string name)
        {
            this.code = code;
            this.price = price;
            this.quantity = quantity;
            this.sum = sum;
            this.discSurc = 0;
            this.name = name;
        }

    }

    public class Payment
    {
        public byte type { get; set; }
        public double pay { get; set; }
        public double change { get; set; }
        public double sum { get; set; }

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
        public Collection<Good> goods { get; set; }
        public Collection<Payment> payments { get; set; }

        internal Check(DateTime dt, uint num)
        {
            this.dateTime = dt;
            this.num = num;
            this.goods = new Collection<Good>();
            this.payments = new Collection<Payment>();
        }

        public void AddGood(uint code, double price, double quantity, double sum, string name)
        {
            Good g = new Good(code, price, quantity, sum, name);
            this.goods.Add(g);
        }

        public void AddPayment(byte type, double pay, double change)
        {
            Payment p = new Payment(type, pay, change);
            this.payments.Add(p);
        }

    }
}
