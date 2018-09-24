using System;
using System.Threading.Tasks;
using mm.Execution;

namespace mm{
    public class ExecutionStrategyStub : IExecutionStrategy
    {
        Task IExecutionStrategy.DeleteAllOrders(Instrument instr)
        {
            return Task.FromResult(0);
        }
    }
}