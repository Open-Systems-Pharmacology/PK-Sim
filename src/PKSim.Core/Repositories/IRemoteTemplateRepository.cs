using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IRemoteTemplateRepository : IStartableRepository<Template>
   {
      string Version { get; }
   }
}