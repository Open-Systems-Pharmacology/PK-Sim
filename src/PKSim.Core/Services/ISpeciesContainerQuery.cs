using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISpeciesContainerQuery
   {
      IEnumerable<IContainer> SubContainersFor(Species species, IContainer parentContainer);
   }
}