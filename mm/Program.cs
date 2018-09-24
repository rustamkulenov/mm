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
            c.RegisterInstance<StrategySettings>(new StrategySettings(Instrument.XBTUSD(), 3m, 0.1));

            c.Register<DOMBalancerStrategy>();
            c.Register<IExecutionStrategy, BitmexSimpleExecutionStrategy>();
            c.Register<OrderManager>();

            return c;
        }

        static Container RegisterBitmexAPI(this Container c)
        {
            c.Register<IBitmexApiProxy, BitmexApiProxy>();
            c.Register<IBitmexApiService, BitmexApiService>();

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
        }
    }
}
