using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IModelContainerQuery
   {
      IEnumerable<IContainer> SubContainersFor(Species species, ModelConfiguration modelConfiguration, IContainer parentContainer);
   }
}