using NUnit.Framework;
using SimpleInjector;
using mm.Execution;

namespace mm
{
    public class DOMBalanceStrategyTests
    {
        private Container _container;

        [OneTimeSetUp]
        public void Setup()
        {
            _container = new Container();
            _container.Register<DOMBalancerStrategy>();
            _container.Register<IOrderManager, OrderManager>();
            _container.Register<IExecutionStrategy, ExecutionStrategyStub>();
            _container.RegisterInstance<StrategySettings>(new StrategySettings(Instrument.XBTUSD(), 300m, 235, 0.1));
        }

        /// <summary>
        /// Tests creation of DOMBalancerStrategy class instance via DI container.
        /// </summary>
        [Test]
        public void DOMBalancerStrategyResolutionTest()
        {
            var b = _container.GetInstance<DOMBalancerStrategy>();

            Assert.Pass();
        }
    }
}