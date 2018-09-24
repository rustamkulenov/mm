using System;
using System.Collections.Generic;
using System.Linq;

namespace mm
{
    /// <summary>
    /// Describes DOM balance around mid-price.
    /// </summary>
    internal sealed class DOMBalance
    {
        public Instrument Instrument { get; private set; }

        /// <summary>
        /// Midprice for the spread. Is not rounded to tick size.
        /// </summary>
        /// <value></value>
        public Decimal? MidPrice { get; private set; }

        /// <summary>
        /// Sum(Size*Price) for all buy-orders within spreadPercent around midPrice.
        /// </summary>
        public Decimal BuyAmount { get; private set; }

        /// <summary>
        /// Sum(Size*Price) for all sell-orders within spreadPercent around midPrice.
        /// </summary>
        public Decimal SellAmount { get; private set; }

        /// <summary>
        /// Amount required to buy to have requiredLiquidity/2 on buy-side.
        /// Positive value means that new buy-orders shall be posted on this amount.
        /// Negative value means that existing buy-orders shall be closed on this amount.
        /// </summary>
        public Decimal BuyDisbalance {get; private set;}

        /// <summary>
        /// Amount required to sell to have requiredLiquidity/2 on sell-side.
        /// Positive value means that new sell-orders shall be posted on this amount.
        /// Negative value means that existing sell-orders shall be closed on this amount.
        /// </summary>
        public Decimal SellDisbalance {get; private set;}

        /// <summary>
        /// Calcs balance and mid-price for the book. Provided bids and asks shall not contain own order.
        /// </summary>
        /// <param name="instr">Instrument</param>
        /// <param name="bids">Bids of DOM</param>
        /// <param name="asks">Asks of DOM</param>
        /// <param name="radiusFromMidpricePercent">Percent around mid-price to filter asks and bids</param>
        /// <param name="requiredLiquidity">Required liquidity to have in DOM within radiusFromMidpricePercent</param>
        public DOMBalance(Instrument instr, IList<Tuple<decimal, int>> bids, IList<Tuple<decimal, int>> asks, double radiusFromMidpricePercent, decimal requiredLiquidity)
        {
            if (requiredLiquidity < 0 ) throw new ArgumentOutOfRangeException();
            if (radiusFromMidpricePercent <= 0 ) throw new ArgumentOutOfRangeException();
            if (bids==null || asks==null) throw new ArgumentNullException("Please provide empty list instead of null");

            Instrument = instr;

            // BBA
            var bestBid = bids.OrderBy(b => b.Item1).LastOrDefault();
            decimal? bidPrice = null;
            if (bestBid != null) bidPrice = bestBid.Item1;

            var bestAsk = asks.OrderBy(b => b.Item1).FirstOrDefault();
            decimal? askPrice = null;
            if (bestAsk != null) askPrice = bestAsk.Item1;

            // Mid price
            if (bidPrice == null && askPrice != null) MidPrice = askPrice;
            if (bidPrice != null && askPrice == null) MidPrice = bidPrice;
            if (bidPrice != null && askPrice != null) MidPrice = (askPrice + bidPrice) / 2.0m;

            if (!MidPrice.HasValue) return;

            foreach (var b in bids)
            {
                if (b.Item1 >= MidPrice - MidPrice * (decimal)radiusFromMidpricePercent)
                {
                    BuyAmount += b.Item1 * b.Item2;
                }
            }

            foreach (var a in asks)
            {
                if (a.Item1 <= MidPrice + MidPrice * (decimal)radiusFromMidpricePercent)
                {
                    SellAmount += a.Item1 * a.Item2;
                }
            }

            BuyDisbalance = requiredLiquidity/2.0m - BuyAmount;
            SellDisbalance = requiredLiquidity/2.0m - SellAmount;

        }
    }
}