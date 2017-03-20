using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IObserverBuilderRepository : IStartableRepository<IPKSimObserverBuilder>
   {
      IEnumerable<IPKSimObserverBuilder> AllFor(ModelProperties modelProperties);
   }
}