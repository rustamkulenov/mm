using System;
using System.Threading;
using mm.Execution;

namespace mm
{
    /// <summary>
    /// DOM rebalancing strategy.
    /// Checks bid vs ask amounts and amends orders to have balance between buy and sell within specified % of midprice.
    /// </summary>
    internal sealed class DOMBalancerStrategy : MMStrategyBase
    {
        public DOMBalancerStrategy(OrderManager om) : base(om) { }

        protected override void Do()
        {

        }
    }
}