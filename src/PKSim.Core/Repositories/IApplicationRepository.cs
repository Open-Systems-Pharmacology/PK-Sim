using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Repositories
{
   public interface IApplicationRepository : IStartableRepository<IApplicationBuilder>
   {
      IApplicationBuilder ApplicationFrom(string applicationType, string formulationType);
   }
}