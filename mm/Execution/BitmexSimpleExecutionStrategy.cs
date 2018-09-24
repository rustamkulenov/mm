using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitmex.NET;
using Bitmex.NET.Models;

namespace mm.Execution
{
    /// <summary>
    /// Simple order execution via Bitmex without complex order strategy.
    /// Large orders sent as a whole.
    /// </summary>
    internal sealed class BitmexSimpleExecutionStrategy : IExecutionStrategy
    {
        private readonly IBitmexApiService _bitmexSvc;

        public BitmexSimpleExecutionStrategy(IBitmexApiService bitmexSvc)
        {
            _bitmexSvc = bitmexSvc;
        }

        async Task Buy(string symbol, int quantity, decimal price)
        {
            var cmd = OrderPOSTRequestParams.CreateSimpleLimit(symbol, quantity, price, OrderSide.Buy);
            await _bitmexSvc.Execute(BitmexApiUrls.Order.PostOrder, cmd);
        }

        async Task Sell(string symbol, int quantity, decimal price)
        {
            var cmd = OrderPOSTRequestParams.CreateSimpleLimit(symbol, quantity, price, OrderSide.Sell);
            await _bitmexSvc.Execute(BitmexApiUrls.Order.PostOrder, cmd);
        }

        async Task IExecutionStrategy.DeleteAllOrders(Instrument instr)
        {
            var del = new OrderAllDELETERequestParams() { Symbol = instr.Symbol };
            await _bitmexSvc.Execute(BitmexApiUrls.Order.DeleteOrderAll, del);
        }

        async Task<SimpleDOM> IExecutionStrategy.GetDOM(Instrument instr, int depth)
        {
            var cmd = new OrderBookL2GETRequestParams() { Symbol = instr.Symbol, Depth = depth };
            var levels = await _bitmexSvc.Execute(BitmexApiUrls.OrderBook.GetOrderBookL2, cmd);

            var bids = new List<Tuple<decimal, decimal>>();
            var asks = new List<Tuple<decimal, decimal>>();
            foreach (var l in levels)
            {
                var t = new Tuple<decimal, decimal>(l.Price, l.Size);
                if (string.Equals("Sell", l.Side, StringComparison.OrdinalIgnoreCase))
                {
                    asks.Add(t);
                }
                else
                {
                    bids.Add(t);
                }
            }
            return await Task.FromResult(new SimpleDOM(bids, asks));
        }

        async Task<Tuple<decimal, decimal>> IExecutionStrategy.GetBBA(Instrument instr)
        {
            // Return hardcoded value because API returns 403 forbidden
            return await Task.FromResult(new Tuple<decimal, decimal>(235.05m, 235.15m));

            var cmd = new QuoteGETRequestParams() { Symbol = instr.Symbol };
            var bbas = await _bitmexSvc.Execute(BitmexApiUrls.Quote.GetQuote, cmd);
            var bba = bbas.FirstOrDefault();
            if (!bba.BidPrice.HasValue || !bba.AskPrice.HasValue)
            {
                return await Task.FromResult((Tuple<decimal, decimal>)null);
            }
            return await Task.FromResult(new Tuple<decimal, decimal>(bba.BidPrice.Value, bba.AskPrice.Value));
        }
    }
}