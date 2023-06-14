using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IRateObjectPathsRepository : IStartableRepository<IRateObjectPaths>
   {
      IEnumerable<FormulaUsablePath> ObjectPathsFor(string rate, string calculationMethod);
      IEnumerable<FormulaUsablePath> ObjectPathsFor(RateKey rateKey);

      /// <summary>
      ///    Returns a clone of the first path defined for the rateKey with the given alias. If not path is found, returns null.
      ///   A clone is returned to ensure that the original path is not modified
      /// </summary>
      FormulaUsablePath PathWithAlias(RateKey rateKey, string alias);
   }
}