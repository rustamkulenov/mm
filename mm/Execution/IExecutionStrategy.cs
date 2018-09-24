using System.Threading.Tasks;

namespace mm.Execution
{
    internal interface IExecutionStrategy
    {
        Task DeleteAllOrders(Instrument instr);
    }
}