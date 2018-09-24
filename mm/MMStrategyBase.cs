using System;
using System.Threading;
using mm.Execution;

namespace mm
{
    /// <summary>
    /// Base class for all MM strategies.
    /// </summary>
    internal abstract class MMStrategyBase : IDisposable
    {
        private static readonly TimeSpan CHECK_INTERVAL = TimeSpan.FromSeconds(5);
        private readonly Thread _thread;
        private readonly AutoResetEvent _evt;
        private bool _active = true;

        public MMStrategyBase()
        {
            _thread = new Thread(ThreadMethod);
            _thread.IsBackground = true;
            _evt = new AutoResetEvent(false);
        }

        private void ThreadMethod(object obj)
        {
            while (_active)
            {
                Do();
                _evt.WaitOne(CHECK_INTERVAL);
            }
        }

        /// <summary>
        /// Implementation of MM strategy step.
        /// </summary>
        protected abstract void Do();

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _active = false;
            SetEvent();
            _thread.Join();
        }

        public void SetEvent()
        {
            _evt.Set();
        }

        #region IDisposable
        private bool _isDisposed = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Stop();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}