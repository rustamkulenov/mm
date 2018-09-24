namespace mm
{
    /// <summary>
    /// Settings for MM strategy.
    /// </summary>
    internal sealed class StrategySettings
    {
        /// <summary>
        /// Instrument to run strategy on.
        /// </summary>
        public Instrument Instrument { get; }

        /// <summary>
        /// Liquidity amount required to support.
        /// </summary>
        public decimal RequiredLiquidity { get; }

        /// <summary>
        /// Distance in % of midprice in which orders to take into account.
        /// </summary>
        public double RadiusPercent { get; }

        /// <summary>
        /// Initializes a new instance of the StrategySettings class.
        /// </summary>
        public StrategySettings(Instrument instrument, decimal requiredLiquidity, double radiusPercent)
        {
            Instrument = instrument;
            RequiredLiquidity = requiredLiquidity;
            RadiusPercent = radiusPercent;
        }
    }
}