using System.Threading.Tasks;

namespace PKSim.BatchTool.Services
{
   public interface IBatchRunner
   {
      Task RunBatch(dynamic parameters);
   }
}