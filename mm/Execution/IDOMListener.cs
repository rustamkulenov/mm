using System;

namespace mm.Execution
{
    /// <summary>
    /// Listener of DOM changes.null Notifies subscibers when change happens.
    /// </summary>
    internal interface IDOMListener
    {
        event Action OnDOMChanged;
    }
}