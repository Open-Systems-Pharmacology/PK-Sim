using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IModelContainerQuery
   {
      IReadOnlyList<IContainer> SubContainersFor(SpeciesPopulation population, ModelConfiguration modelConfiguration, IContainer parentContainer);
   }
}