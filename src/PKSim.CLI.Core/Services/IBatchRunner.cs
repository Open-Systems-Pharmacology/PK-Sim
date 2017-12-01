using System.Threading.Tasks;

namespace PKSim.CLI.Core.Services
{
   public interface IBatchRunner<TBatchOptions>
   {
      Task RunBatchAsync(TBatchOptions runOptions);
   }
}