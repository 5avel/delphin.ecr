using System;
using System.Collections.ObjectModel;

namespace Delphin
{
    internal class Good
    {
        internal uint code { get; set; }
        internal double price { get; set; }
        internal double quantity { get; set; }
        internal double sum { get; set; }
        internal double discSurc { get; set; }
        internal string name { get; set; }

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

    internal class Payment
    {
        private byte type { get; set; }
        private double pay { get; set; }
        private double change { get; set; }
        private double sum { get; set; }

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
        internal DateTime dateTime { get; set; }
        internal uint num { get; set; }
        internal Collection<Good> goods { get; set; }
        internal Collection<Payment> payments { get; set; }

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
