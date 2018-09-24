using System;
using System.Collections.Generic;

namespace mm.Execution
{
    internal sealed class SimpleDOM
    {

        public SimpleDOM(List<Tuple<decimal, decimal>> bids, List<Tuple<decimal, decimal>> asks)
        {
            Bids = bids;
            Asks = asks;
        }

        public List<Tuple<decimal, decimal>> Bids { get; }
        public List<Tuple<decimal, decimal>> Asks { get; }
    }

}