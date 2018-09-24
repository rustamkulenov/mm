namespace mm.Execution
{
    /// <summary>
    /// Manages market-maker's orders.
    /// </summary>
    internal interface IOrderManager
    {
        /// <summary>
        /// 'Eats' some bid side orders, e.g. by MKT sell orders.
        /// </summary>
        void EatBidSide(decimal volume);

        /// <summary>
        /// 'Eats' some ask side orders, e.g. by MKT sell orders.
        /// </summary>
        void EatAskSide(decimal volume);

        /// <summary>
        /// Amends size and price of market-maker's orders.
        /// If Size is 0, then order will be canceled.
        /// </summary>
        void AmendMMOrders(decimal bidPrice, decimal bidSize, decimal askPrice, decimal AskSize);
    }
}