namespace mm
{
    /// <summary>
    /// Settings for MM strategy.
    /// </summary>
    internal sealed class StrategySettings
    {
        private decimal _ethusd;

        /// <summary>
        /// Instrument to run strategy on.
        /// </summary>
        public Instrument Instrument { get; }

        /// <summary>
        /// Liquidity amount required to support in ETH.
        /// </summary>
        public decimal RequiredLiquidityETH { get; }

        /// <summary>
        /// Liquidity amount required to support in USD.
        /// </summary>
        public decimal RequiredLiquidityUSD { get { return RequiredLiquidityETH * _ethusd; } }

        /// <summary>
        /// ETHUSD quote used to calc required liquidity in USD.
        /// </summary>
        public decimal ETHUSD
        {
            get { return _ethusd; }
            set { _ethusd = value; }
        }

        /// <summary>
        /// Distance in % of midprice in which orders to take into account.
        /// </summary>
        public double RadiusPercent { get; }

        /// <summary>
        /// Initializes a new instance of the StrategySettings class.
        /// </summary>
        public StrategySettings(Instrument instrument, decimal requiredLiquidityETH, decimal ethusd, double radiusPercent)
        {
            Instrument = instrument;
            RequiredLiquidityETH = requiredLiquidityETH;
            _ethusd = ethusd;
            RadiusPercent = radiusPercent;
        }
    }
}