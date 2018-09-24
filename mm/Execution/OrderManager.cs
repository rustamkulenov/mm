namespace mm.Execution
{
    /// <summary>
    /// Stores info about MM orders and executes new orders.
    /// </summary>
    internal sealed class OrderManager
    {
        private readonly IExecutionStrategy _exec;
        private readonly StrategySettings _settings;

        public OrderManager(IExecutionStrategy exec, StrategySettings settings)
        {
            _exec = exec;
            _settings = settings;
            _exec.DeleteAllOrders(_settings.Instrument).Wait();
        }
    }
}