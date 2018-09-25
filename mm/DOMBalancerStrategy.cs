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
        private readonly IOrderManager _om;
        private readonly IExecutionStrategy _exec;
        private readonly StrategySettings _settings;
        private readonly IDOMListener _domListener;

        public DOMBalancerStrategy(IOrderManager om, IExecutionStrategy exec, StrategySettings settings, IDOMListener domListener) : base()
        {
            _om = om;
            _exec = exec;
            _settings = settings;
            _domListener = domListener;

            _domListener.OnDOMChanged += () =>
            {
                // Force calculation on DOM change
                base.SetEvent();
            };
        }

        protected override void Do()
        {
            var bba = _exec.GetBBA(Instrument.ETHUSD()).Result;
            var kethusd = 1m;
            if (bba != null)
            {
                kethusd = (bba.Item1 + bba.Item2) / 2.0m;
                _settings.ETHUSD = kethusd; // Update ETHUSD quote in settings
                kethusd *= 1000.0m;
            }

            var dom = _exec.GetDOM(_settings.Instrument).Result;
            //TODO: Remove own orders from DOM before calcs

            var balance = new DOMBalance(_settings.Instrument, dom, _settings.RadiusPercent, _settings.RequiredLiquidityUSD);

            Console.WriteLine($"midPrice=${balance.MidPrice:0.#} low=${balance.MidPrice - balance.MidPrice * (decimal)_settings.RadiusPercent:0.#} high=${balance.MidPrice + balance.MidPrice * (decimal)_settings.RadiusPercent:0.#}");
            Console.WriteLine($"buyAmount={balance.BuyAmount / balance.MidPrice / 1000.0m:0.#}K XBT sellAmount={balance.SellAmount / balance.MidPrice / 1000.0m:0.#}K XBT "
            + $"buyDisb={balance.BuyDisbalance / kethusd:0.#}K ETH sellDisb={balance.SellDisbalance / kethusd:0.#}K ETH");

            // If buy side volume is more than expected then fill some orders
            if (balance.BuyDisbalance < 0)
            {
                _om.EatBidSide(-balance.BuyDisbalance);
            }

            // If sell side volume is more than expected then fill some orders
            if (balance.SellDisbalance < 0)
            {
                _om.EatAskSide(-balance.SellDisbalance);
            }

            var buyPrice = balance.MidPrice - balance.MidPrice * (decimal)_settings.RadiusPercent;
            var sellPrice = balance.MidPrice + balance.MidPrice * (decimal)_settings.RadiusPercent;
            if (balance.MidPrice.HasValue)
            {
                _om.AmendMMOrders(buyPrice ?? 0, balance.BuyDisbalance, sellPrice ?? 0, balance.SellDisbalance);
            }
        }
    }
}