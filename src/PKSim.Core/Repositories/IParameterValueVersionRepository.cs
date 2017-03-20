using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IParameterValueVersionRepository : IStartableRepository<ParameterValueVersion>
   {
      ParameterValueVersion FindBy(string name);
   }
}