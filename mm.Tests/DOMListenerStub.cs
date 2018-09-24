using System;
using mm.Execution;

namespace mm
{
    public class DOMListenerStub : IDOMListener
    {
        public event Action OnDOMChanged;
    }
}