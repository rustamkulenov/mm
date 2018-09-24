namespace mm
{
    internal sealed class Instrument
    {
        public string Symbol;
        public string PositionCCY;

        public string UnderlyingCCY;

        public string QuoteCCY;

        public int MaxOrderQty;

        public int MaxPrice;

        public int LotSize;

        public double TickSize;

        public static Instrument XBTUSD()
        {
            var i = new Instrument()
            {
                Symbol = "XBTUSD",
                PositionCCY = "USD",
                QuoteCCY = "USD",
                UnderlyingCCY = "XBT",
                MaxOrderQty = 10000000,
                MaxPrice = 1000000,
                LotSize = 1,
                TickSize = 0.5

            };
            return i;

        }
    }
}