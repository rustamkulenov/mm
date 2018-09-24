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
    }
}