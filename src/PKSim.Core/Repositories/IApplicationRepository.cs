using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Repositories
{
   public interface IApplicationRepository : IStartableRepository<ApplicationBuilder>
   {
      ApplicationBuilder ApplicationFrom(string applicationType, string formulationType);
   }
}