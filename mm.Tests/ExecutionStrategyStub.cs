using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mm.Execution;

namespace mm
{
    public class ExecutionStrategyStub : IExecutionStrategy
    {
        Task IExecutionStrategy.DeleteAllOrders(Instrument instr)
        {
            return Task.FromResult(0);
        }

        Task<Tuple<decimal, decimal>> IExecutionStrategy.GetBBA(Instrument instr)
        {
            throw new NotImplementedException();
        }

        Task<SimpleDOM> IExecutionStrategy.GetDOM(Instrument instr, int depth)
        {
            var bids = new List<Tuple<decimal, decimal>>();
            var asks = new List<Tuple<decimal, decimal>>();
            return Task.FromResult(new SimpleDOM(bids, asks));
        }
    }
}