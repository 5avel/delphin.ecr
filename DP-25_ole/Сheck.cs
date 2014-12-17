using System;
using System.Collections.ObjectModel;

namespace Delphin
{
    private class Good
    {
        private int code { get; set; }
        private double price { get; set; }
        private double quantity { get; set; }
        private double sum { get; set; }
        private double discSurc { get; set; }

        internal Good(int code, double price, double quantity, double sum)
        {
            this.code = code;
            this.price = price;
            this.quantity = quantity;
            this.sum = sum;
            this.discSurc = 0;
        }
    }

    private class Payment
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
    class Сheck
    {
        private DateTime dateTime {get; set;}
        private int num { get; set; }
        private Collection<Good> goods {get; set;}
        private Collection<Payment> payments { get; set; }

        public void AddGood(int code, double price, double quantity, double sum)
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
