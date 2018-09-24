using System;
using System.Threading.Tasks;

namespace mm.Execution
{
    internal interface IExecutionStrategy
    {
        /// <summary>
        /// Delete all own orders for the instrument.
        /// </summary>
        Task DeleteAllOrders(Instrument instr);

        /// <summary>
        /// Gets DOM levels.
        /// </summary>
        Task<SimpleDOM> GetDOM(Instrument instr, int depth = 25);

        /// <summary>
        /// Gets best bid\ask for the instrument.
        /// </summary>
        Task<Tuple<decimal, decimal>> GetBBA(Instrument instr);
    }
}