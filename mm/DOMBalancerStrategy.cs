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
        private readonly IExecutionStrategy _exec;
        private readonly StrategySettings _settings;

        public DOMBalancerStrategy(OrderManager om, IExecutionStrategy exec, StrategySettings settings) : base(om)
        {
            _exec = exec;
            _settings = settings;
        }

        protected override void Do()
        {
            var bba = _exec.GetBBA(Instrument.ETHUSD()).Result;
            var kethusd = 1m;
            if (bba != null)
            {
                kethusd = (bba.Item1 + bba.Item2) / 2.0m;
                _settings.ETHUSD = kethusd; // Update UTHUSD quote in settings
                kethusd *= 1000.0m;
            }

            var dom = _exec.GetDOM(_settings.Instrument).Result;
            var balance = new DOMBalance(_settings.Instrument, dom, _settings.RadiusPercent, _settings.RequiredLiquidityUSD);

            Console.WriteLine($"midPrice=${balance.MidPrice:0.#} low=${balance.MidPrice - balance.MidPrice * (decimal)_settings.RadiusPercent:0.#} high=${balance.MidPrice + balance.MidPrice * (decimal)_settings.RadiusPercent:0.#}");
            Console.WriteLine($"buyAmount={balance.BuyAmount / balance.MidPrice / 1000.0m:0.#}K XBT sellAmount={balance.SellAmount / balance.MidPrice / 1000.0m:0.#}K XBT "
            + $"buyDisb={balance.BuyDisbalance / kethusd:0.#}K ETH sellDisb={balance.SellDisbalance / kethusd:0.#}K ETH");
        }
    }
}