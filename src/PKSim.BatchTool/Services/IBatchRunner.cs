using System.Threading.Tasks;

namespace PKSim.BatchTool.Services
{
   public interface IBatchRunner<TBatchOptions>
   {
      Task RunBatch(TBatchOptions parameters);
   }
}