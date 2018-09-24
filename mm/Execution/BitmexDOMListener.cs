using System;
using Bitmex.NET;

namespace mm.Execution
{
    /// <summary>
    /// Listens for changes in DOM of the strategy instrument and notifies listeners.
    /// </summary>
    internal sealed class BitmexDOMListener : IDOMListener
    {
        private readonly IBitmexApiSocketService _svc;
        public event Action OnDOMChanged;

        public BitmexDOMListener(IBitmexApiSocketService svc, StrategySettings settings)
        {
            _svc = svc;
            _svc.Connect();
            _svc.Subscribe(BitmetSocketSubscriptions.CreateOrderBookL2Subsription(
                msg =>
                {
                    foreach (var dto in msg.Data)
                    {
                        if (!string.Equals(dto.Symbol, settings.Instrument.Symbol, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var tmp = OnDOMChanged;
                        if (tmp != null)
                        {
                            tmp();
                        }
                    }
                }
            ));
        }
    }
}