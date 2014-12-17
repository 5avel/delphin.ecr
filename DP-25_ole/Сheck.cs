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

        internal Good(uint code, double price, double quantity, double sum)
        {
            this.code = code;
            this.price = price;
            this.quantity = quantity;
            this.sum = sum;
            this.discSurc = 0;
        }
    }

    internal class Payment
    {
        private byte type { get; set; }
        private double sum { get; set; }

        internal Payment(byte type, double sum)
        {
            this.type = type;
            this.sum = sum;
        }
    }
   

    // номер
    //дата время
    //товары со скидками и надбавками на товар, пидсумок со скидкой или надбавкой -  пробегает попредидушим товарам и проставляетим скидку и ли надавку.
    //оплаты
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

        public void AddGood(uint code, double price, double quantity, double sum)
        {
            Good g = new Good(code, price, quantity, sum);
            this.goods.Add(g);
        }

        public void AddPayment(byte type, double sum)
        {
            Payment p = new Payment(type, sum);
            this.payments.Add(p);
        }

    }
}
