namespace mm.Execution
{
    /// <summary>
    /// Stores info about MM orders and executes new orders.
    /// </summary>
    internal sealed class OrderManager : IOrderManager
    {
        private readonly IExecutionStrategy _exec;
        private readonly StrategySettings _settings;

        private Order _buyOrder;
        private Order _sellOrder;

        public OrderManager(IExecutionStrategy exec, StrategySettings settings)
        {
            _exec = exec;
            _settings = settings;
            _exec.DeleteAllOrders(_settings.Instrument).Wait();
        }

        void IOrderManager.EatBidSide(decimal volume)
        {
            // TODO: send MKT SELL
        }

        void IOrderManager.EatAskSide(decimal volume)
        {
            // TODO: send MKT BUY
        }

        void IOrderManager.AmendMMOrders(decimal bidPrice, decimal bidSize, decimal askPrice, decimal AskSize)
        {
            if (bidSize == 0m && _buyOrder != null)
            {
                // Cancel BUY order
                _buyOrder = null;
            }
            if (AskSize == 0m && _sellOrder != null)
            {
                // Cancel SELL order
                _sellOrder = null;
            }

            if (bidSize > 0)
            {
                if (_buyOrder == null)
                {
                    // Open BUY order at bidPrice
                    _buyOrder = new Order() { Price = bidPrice, Qty = bidSize };
                }
                else
                {
                    // Amend BUY order
                    _buyOrder.Price = bidPrice;
                    _buyOrder.Qty = bidSize;
                }
            }

            if (AskSize > 0)
            {
                if (_sellOrder == null)
                {
                    // Open SELL order at askPrice
                    _sellOrder = new Order() { Price = askPrice, Qty = AskSize };
                }
                else
                {
                    // Amend SELL order
                    _sellOrder.Price = askPrice;
                    _sellOrder.Qty = AskSize;
                }
            }
        }
    }
}