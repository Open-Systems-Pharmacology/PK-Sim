using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISpeciesContainerQuery
   {
      IReadOnlyList<IContainer> SubContainersFor(SpeciesPopulation speciesPopulation, IContainer parentContainer);
   }
}