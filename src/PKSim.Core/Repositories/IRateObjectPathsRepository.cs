using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IRateObjectPathsRepository : IStartableRepository<IRateObjectPaths>
   {
      IEnumerable<IFormulaUsablePath> ObjectPathsFor(string rate, string calculationMethod);
      IEnumerable<IFormulaUsablePath> ObjectPathsFor(RateKey rateKey);

      /// <summary>
      ///    returns the first path defined for the ratekey with the given alias. If not path is found, returns null
      /// </summary>
      IFormulaUsablePath PathWithAlias(RateKey rateKey, string alias);
   }
}