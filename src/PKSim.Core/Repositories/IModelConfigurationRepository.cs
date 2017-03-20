using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IModelConfigurationRepository : IStartableRepository<ModelConfiguration>
   {
      IEnumerable<ModelConfiguration> AllFor(Species species);
      ModelConfiguration DefaultFor(Species species);
      ModelConfiguration FindById(string modelConfigurationId);
   }
}