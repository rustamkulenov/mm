using System;
using Bitmex.NET;
using SimpleInjector;
using System.Runtime.CompilerServices;
using mm.Execution;

[assembly: InternalsVisibleTo("mm.Tests")]
namespace mm
{
    static class Program
    {
        const string KEY = "IWJmYC4VUt20Y0iaFehIOqWy";
        const string SECRET = "U_xc9OjtdJckRLKwTlc4w50WAq-QoP2MK0OdRIevKGHTBkMC";

        static readonly Container _container;

        static Program()
        {
            _container = new Container();
            _container.Options.ConstructorResolutionBehavior = new MostResolvableParametersConstructorResolutionBehavior(_container);

            _container
                .RegisterBitmexAPI()
                .RegisterMMTypes();

            _container.Verify();
        }

        static Container RegisterMMTypes(this Container c)
        {
            var liquidityETH = 13 * 1000 * 1000; // 13M ETH liquidity to support in DOM
            var ethusd = 235m; // Hardcoded ETHUSD quote)
            var percent = 0.05 / 100.0; // 0.05%
            c.RegisterInstance<StrategySettings>(new StrategySettings(Instrument.XBTUSD(), liquidityETH, ethusd, percent));

            c.Register<DOMBalancerStrategy>(Lifestyle.Singleton);
            c.Register<IExecutionStrategy, BitmexSimpleExecutionStrategy>(Lifestyle.Singleton);
            c.Register<OrderManager>(Lifestyle.Singleton);

            return c;
        }

        static Container RegisterBitmexAPI(this Container c)
        {
            c.Register<IBitmexApiProxy, BitmexApiProxy>(Lifestyle.Singleton);
            c.Register<IBitmexApiService, BitmexApiService>(Lifestyle.Singleton);

            var authorization = new BitmexAuthorization
            {
                BitmexEnvironment = Bitmex.NET.Models.BitmexEnvironment.Test,
                Key = KEY,
                Secret = SECRET
            };

            c.RegisterInstance<IBitmexAuthorization>(authorization);
            return c;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            using (var b = _container.GetInstance<DOMBalancerStrategy>())
            {
                b.Start();
                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
                Console.WriteLine("Stopping...");
            }
            _container.Dispose();
        }
    }
}
