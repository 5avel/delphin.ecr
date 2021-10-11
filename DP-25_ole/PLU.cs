
namespace Delphin
{
    public class PLU
    {
        /// <summary>
        /// Item number ( For ECRs 1...100000; For FPs 1...3000 );
        /// </summary>
        public int Code { get; internal set; }

        /// <summary>
        /// VAT group (letter 'A'...'H' or cyrillic 'А'...'З');
        /// </summary>
        public byte TaxGr  { get; internal set; }

        /// <summary>
        /// Department ( 0...99 );
        /// </summary>
        public byte Dep { get; internal set; }

        /// <summary>
        /// Stock group (1...99);
        /// </summary>
        public byte Group  { get; internal set; }

        /// <summary>
        /// Price type ('0' - fixed price, '1' - free price, '2' - max price;) ;
        /// </summary>
        public byte PriceType { get; internal set; }

        /// <summary>
        /// Price ( 0.00...9999999.99 or 0...999999999 depending dec point position );
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        /// Accumulated amount of the item ( 0.00...9999999.99 or 0...999999999 depending dec point position );
        /// </summary>
        public double Turnover { get;  internal set; }

        /// <summary>
        /// Sold out quantity ( 0.001...99999.999 );
        /// </summary>
        public double SoldQty { get; internal set; }

        /// <summary>
        /// Current quantity ( 0.001...99999.999 );
        /// </summary>
        public double StockQty { get; internal set; }

        /// <summary>
        ///  Item name ( up to 72 symbols );
        /// </summary>
        public string Name{ get; internal set; }

        /// <summary>
        /// Barcode X ( up to 13 digits );
        /// </summary>
        public string BarX { get; internal set; }
        /// <summary>
        /// Дробное количество 1 true, 0 false
        /// </summary>
        public string FractionalQty { get; internal set; }
        /// <summary>
        /// Код УКТЗЕД (up to 10 digits)
        /// </summary>
        public string CustomCode { get; internal set; }
    }
}
