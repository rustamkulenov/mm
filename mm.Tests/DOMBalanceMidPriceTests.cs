using System;
using System.Collections.Generic;
using mm.Execution;
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
            var empty = new List<Tuple<decimal, decimal>>();
            var d = new DOMBalance(Instrument.XBTUSD(), new SimpleDOM(empty, empty), 0.1, 100m);
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
            const decimal LOTS = 100.0m;

            var bids = new List<Tuple<decimal, decimal>>();
            bids.Add(new Tuple<decimal, decimal>(PRICE, LOTS));
            var asks = new List<Tuple<decimal, decimal>>();
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, 0.1, 100m);
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
            const decimal LOTS = 100m;

            var bids = new List<Tuple<decimal, decimal>>();
            var asks = new List<Tuple<decimal, decimal>>();
            asks.Add(new Tuple<decimal, decimal>(PRICE, LOTS));
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, 0.1, 100m);
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
            const decimal LOTS = 100m;

            var bids = new List<Tuple<decimal, decimal>>();
            bids.Add(new Tuple<decimal, decimal>(BUY_PRICE, LOTS));
            var asks = new List<Tuple<decimal, decimal>>();
            asks.Add(new Tuple<decimal, decimal>(SELL_PRICE, LOTS));
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, 0.1, 100m);
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
            const decimal LOTS = 100m;

            var bids = new List<Tuple<decimal, decimal>>();
            bids.Add(new Tuple<decimal, decimal>(BUY_PRICE - 10m, LOTS));
            bids.Add(new Tuple<decimal, decimal>(BUY_PRICE, LOTS));
            bids.Add(new Tuple<decimal, decimal>(BUY_PRICE - 100m, LOTS));

            var asks = new List<Tuple<decimal, decimal>>();
            asks.Add(new Tuple<decimal, decimal>(SELL_PRICE + 45m, LOTS));
            asks.Add(new Tuple<decimal, decimal>(SELL_PRICE, LOTS));
            asks.Add(new Tuple<decimal, decimal>(SELL_PRICE + 167m, LOTS));
            var dom = new SimpleDOM(bids, asks);
            // Act
            var d = new DOMBalance(Instrument.XBTUSD(), dom, 0.1, 100m);
            // Asert
            Assert.AreEqual(EXPECTED_MIDPRICE, d.MidPrice, $"Incorrect mid-price for spread {BUY_PRICE}..{SELL_PRICE}");
        }
    }
}