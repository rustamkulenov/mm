using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace mm
{
    public class DOMBalanceMidPriceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Ensures that for empty DOM nothing is calculated.
        /// </summary>
        [Test]
        public void EmptyDOMTest()
        {
            // Arrange & Act
            var empty = new List<Tuple<decimal, int>>();
            var d = new DOMBalance(Instrument.XBTUSD(), empty, empty, 0.1, 100m);
            // Asert
            Assert.IsNull(d.MidPrice, "Mid price shall be null");

            Assert.AreEqual(0m, d.BuyDisbalance, "Shall not change buy side");
            Assert.AreEqual(0m, d.BuyDisbalance, "Shall not change buy side");

            Assert.AreEqual(0m, d.BuyAmount, "Buy side shall be equal to 0");
            Assert.AreEqual(0m, d.BuyAmount, "Sell side shall be equal to 0");
        }

        /// <summary>
        /// Ensures that for only 1 order in DOM midprice is equal to that order price.
        /// </summary>
        [Test]
        public void SingleBuyMidprice()
        {
            // Arrange
            const decimal PRICE = 1.2345m;
            const int LOT_SIZE = 100;

            var bids = new List<Tuple<decimal, int>>();
            bids.Add(new Tuple<decimal, int>(PRICE, LOT_SIZE));
            var asks = new List<Tuple<decimal, int>>();
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), bids, asks, 0.1, 100m);
            // Asert
            Assert.AreEqual(PRICE, d.MidPrice, "Incorrect mid-price");
        }

        /// <summary>
        /// Ensures that for only 1 order in DOM midprice is equal to that order price.
        /// </summary>
        [Test]
        public void SingleSellMidprice()
        {
            // Arrange
            const decimal PRICE = 1.2345m;
            const int LOT_SIZE = 100;

            var bids = new List<Tuple<decimal, int>>();
            var asks = new List<Tuple<decimal, int>>();
            asks.Add(new Tuple<decimal, int>(PRICE, LOT_SIZE));
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), bids, asks, 0.1, 100m);
            // Asert
            Assert.AreEqual(PRICE, d.MidPrice, "Incorrect mid-price");
        }

        /// <summary>
        /// Ensures that correct mid-price is calculated for simple spread with 2 orders (buy&amp;sell).
        /// </summary>
        [Test]
        public void SimpleSpreadMidprice()
        {
            // Arrange
            const decimal BUY_PRICE = 6004.5m;
            const decimal SELL_PRICE = 6016.0m;
            const decimal EXPECTED_MIDPRICE = 6010.25m;
            const int LOT_SIZE = 100;

            var bids = new List<Tuple<decimal, int>>();
            bids.Add(new Tuple<decimal, int>(BUY_PRICE, LOT_SIZE));
            var asks = new List<Tuple<decimal, int>>();
            asks.Add(new Tuple<decimal, int>(SELL_PRICE, LOT_SIZE));
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), bids, asks, 0.1, 100m);
            // Asert
            Assert.AreEqual(EXPECTED_MIDPRICE, d.MidPrice, $"Incorrect mid-price for spread {BUY_PRICE}..{SELL_PRICE}");
        }

        /// <summary>
        /// Ensures that correct mid-price is calculated for full not ordered DOM book.
        /// </summary>
        [Test]
        public void DOMMidprice()
        {
            // Arrange
            const decimal BUY_PRICE = 6004.5m;
            const decimal SELL_PRICE = 6016.0m;
            const decimal EXPECTED_MIDPRICE = 6010.25m;
            const int LOT_SIZE = 100;

            var bids = new List<Tuple<decimal, int>>();
            bids.Add(new Tuple<decimal, int>(BUY_PRICE - 10m, LOT_SIZE));
            bids.Add(new Tuple<decimal, int>(BUY_PRICE, LOT_SIZE));
            bids.Add(new Tuple<decimal, int>(BUY_PRICE - 100m, LOT_SIZE));

            var asks = new List<Tuple<decimal, int>>();
            asks.Add(new Tuple<decimal, int>(SELL_PRICE + 45m, LOT_SIZE));
            asks.Add(new Tuple<decimal, int>(SELL_PRICE, LOT_SIZE));
            asks.Add(new Tuple<decimal, int>(SELL_PRICE + 167m, LOT_SIZE));
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), bids, asks, 0.1, 100m);
            // Asert
            Assert.AreEqual(EXPECTED_MIDPRICE, d.MidPrice, $"Incorrect mid-price for spread {BUY_PRICE}..{SELL_PRICE}");
        }
    }
}