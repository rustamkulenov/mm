using System;
using System.Collections.Generic;
using mm.Execution;
using NUnit.Framework;

namespace mm
{
    public class DOMDisbalanceTests
    {
        /// <summary>
        /// Ensures that far from mid-price orders are filtered out (amounts==0), but midprice is still calculated.
        /// </summary>
        [Test]
        public void FilterOutFarOrders()
        {
            // Arrange
            const decimal EXPECTED_MIDPRICE = 4510.25m;
            const double RADIUS_FROM_MIDPRICE_PERCENT = 0.5; // 50% from mid price
            const decimal BUY_PRICE = EXPECTED_MIDPRICE - EXPECTED_MIDPRICE * (decimal)(RADIUS_FROM_MIDPRICE_PERCENT) - 0.5m; // 0.5 ticks below lower border
            const decimal SELL_PRICE = EXPECTED_MIDPRICE + EXPECTED_MIDPRICE * (decimal)(RADIUS_FROM_MIDPRICE_PERCENT) + 0.5m; // 0.5 ticks over upper border
            const decimal EXPECTED_LIQUIDITY = 47m;
            const decimal LOTS = 100m;

            var bids = new List<Tuple<decimal, decimal>>();
            bids.Add(new Tuple<decimal, decimal>(BUY_PRICE, LOTS));
            var asks = new List<Tuple<decimal, decimal>>();
            asks.Add(new Tuple<decimal, decimal>(SELL_PRICE, LOTS));
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, RADIUS_FROM_MIDPRICE_PERCENT, EXPECTED_LIQUIDITY);
            // Asert
            Assert.AreEqual(EXPECTED_MIDPRICE, d.MidPrice, $"Incorrect mid-price for spread {BUY_PRICE}..{SELL_PRICE}");

            // Since all far orders are filtered out, so we need to create buy and sell orders to make liquidity
            Assert.AreEqual(0m, d.SellAmount, "Sell not filtered out");
            Assert.AreEqual(0m, d.BuyAmount, "Buy not filtered out");
            Assert.AreEqual(EXPECTED_LIQUIDITY / 2.0m, d.BuyDisbalance, "Expected to buy");
            Assert.AreEqual(EXPECTED_LIQUIDITY / 2.0m, d.SellDisbalance, "Expected to sell");
        }

        /// <summary>
        /// Ensures that if single buy order is opened and it's size is half of required liquidity (L/2), then
        /// opposite sell order will be opened with size L/2.
        /// </summary>
        [Test]
        public void OppositeOrderToSingleBuy()
        {
            // Arrange
            const decimal PRICE = 6056.0m;
            const int LOT_SIZE = 2;
            const decimal REQUIRED_LIQUIDITY_IN_DOM = PRICE * LOT_SIZE * 2; // 2 orders with the same price*vol

            var bids = new List<Tuple<decimal, decimal>>();
            bids.Add(new Tuple<decimal, decimal>(PRICE, LOT_SIZE));
            var asks = new List<Tuple<decimal, decimal>>();
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, 1, REQUIRED_LIQUIDITY_IN_DOM);
            // Asert
            Assert.AreEqual(PRICE, d.MidPrice, "Incorrect mid-price");
            Assert.AreEqual(0m, d.SellAmount, "Incorrect sell amount");
            Assert.AreEqual(0m, d.BuyDisbalance, "Not expected to buy");

            Assert.AreEqual(PRICE * LOT_SIZE, d.BuyAmount, "Incorrect buy amount");
            Assert.AreEqual(PRICE * LOT_SIZE, d.SellDisbalance, "Expected to sell");
        }

        /// <summary>
        /// Ensures that if single sell order is opened and it's size is half of required liquidity (L/2), then
        /// opposite buy order will be opened with size L/2.
        /// </summary>
        [Test]
        public void OppositeOrderToSingleSell()
        {
            // Arrange
            const decimal PRICE = 6056.0m;
            const int LOT_SIZE = 2;
            const decimal REQUIRED_LIQUIDITY_IN_DOM = PRICE * LOT_SIZE * 2; // 2 orders with the same price*vol

            var bids = new List<Tuple<decimal, decimal>>();
            var asks = new List<Tuple<decimal, decimal>>();
            asks.Add(new Tuple<decimal, decimal>(PRICE, LOT_SIZE));
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, 1, REQUIRED_LIQUIDITY_IN_DOM);
            // Asert
            Assert.AreEqual(PRICE, d.MidPrice, "Incorrect mid-price");
            Assert.AreEqual(0m, d.BuyAmount, "Incorrect buy amount");
            Assert.AreEqual(0m, d.SellDisbalance, "Not expected to sell");

            Assert.AreEqual(PRICE * LOT_SIZE, d.SellAmount, "Incorrect sell amount");
            Assert.AreEqual(PRICE * LOT_SIZE, d.BuyDisbalance, "Expected to buy");
        }

        /// <summary>
        /// Ensures that if single large sell order is opened and it's size is more than required liquidity, then
        /// opposite buy order will be opened with size L/2 and some sell orders need to be eaten.
        /// </summary>
        [Test]
        public void SingleLargeSell()
        {
            // Arrange
            const decimal PRICE = 6056.0m;
            const int LOTS = 2;
            const decimal REQUIRED_LIQUIDITY_IN_DOM = PRICE * LOTS * 0.8m;

            var bids = new List<Tuple<decimal, decimal>>();
            var asks = new List<Tuple<decimal, decimal>>();
            asks.Add(new Tuple<decimal, decimal>(PRICE, LOTS));
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, 1, REQUIRED_LIQUIDITY_IN_DOM);
            // Asert
            Assert.AreEqual(PRICE, d.MidPrice, "Incorrect mid-price");
            Assert.AreEqual(0m, d.BuyAmount, "Incorrect buy amount");
            Assert.Less(d.SellDisbalance, 0m, "Sell disbalance shall be negative (need close/eat some sell LMTs)");

            Assert.AreEqual(PRICE * LOTS, d.SellAmount, "Incorrect sell amount");
            Assert.AreEqual(REQUIRED_LIQUIDITY_IN_DOM / 2.0m, d.BuyDisbalance, "Expected to buy");
        }
    }
}