# mm
Market Maker algos

This algo tries to maintain balance equation (13M ETH liquidity) for provided instrument (XBTUSD) within provided radius (0.05%) around mid-price:

![](https://github.com/rustamkulenov/mm/blob/master/eq1.gif)

![](https://github.com/rustamkulenov/mm/blob/master/eq2.gif)

![](https://github.com/rustamkulenov/mm/blob/master/eq3.gif)

, where:
- L - liquidity (in ETH) to maintain within the order book;
- p - price from DOM book level;
- v - volume from DOM book level;
- p_mid - mid-price for the spread;
- r - radius in % og the mid-price where to maintain balance;

Algo places 2 large orders at prices p_mid*(1+r) and p_mid*(1-r). Volue is calculated based on disbalace between buy and sell sides.

If liquidity on some side (buy or sell) is more than L/2 (half of all liquidity), then corresponging side will be eaten' by market orders (MKT BUY or MKT SELL).

Execution is implemented via ![BitMEX](https://www.bitmex.com) connector.
