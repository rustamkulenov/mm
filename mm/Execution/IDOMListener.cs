using System;

namespace mm.Execution
{
    internal interface IDOMListener
    {
        event Action OnDOMChanged;
    }
}