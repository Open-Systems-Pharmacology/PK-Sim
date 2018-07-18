using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public interface IPostLaunchChecker
   {
      Task PerformPostLaunchCheckAsync();
   }
}