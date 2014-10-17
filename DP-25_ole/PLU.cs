using System;
using System.Text;

namespace Delphin
{
    public class PLU
    {

        private int code;

        public int Code
        {
            get { return code; }
            internal set { code = value; }
        }
        private byte taxGr;

        public byte TaxGr
        {
            get { return taxGr; }
            internal set { taxGr = value; }
        }
        private byte dep;

        public byte Dep
        {
            get { return dep; }
            internal set { dep = value; }
        }
        private byte group;

        public byte Group
        {
            get { return group; }
            internal set { group = value; }
        }
        private byte priceType;

        public byte PriceType
        {
            get { return priceType; }
            internal set { priceType = value; }
        }
        private double price;

        public double Price
        {
            get { return price; }
            internal set { price = value; }
        }
        private double turnover;

        public double Turnover
        {
            get { return turnover; }
            internal set { turnover = value; }
        }
        private double soldQty;

        public double SoldQty
        {
            get { return soldQty; }
            internal set { soldQty = value; }
        }
        private double stockQty;

        public double StockQty
        {
            get { return stockQty; }
            internal set { stockQty = value; }
        }
        private string bar1;

        public string Bar1
        {
            get { return bar1; }
            internal set { bar1 = value; }
        }
        private string bar2;

        public string Bar2
        {
            get { return bar2; }
            internal set { bar2 = value; }
        }
        private string bar3;

        public string Bar3
        {
            get { return bar3; }
            internal set { bar3 = value; }
        }
        private string bar4;

        public string Bar4
        {
            get { return bar4; }
            internal set { bar4 = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }
        private int connectedPLU;

        public int ConnectedPLU
        {
            get { return connectedPLU; }
            internal set { connectedPLU = value; }
        }

        
    }
}
