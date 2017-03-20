using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Repositories
{
   public interface IModelPassiveTransportMoleculeNameRepository : IStartableRepository<MoleculeList>
   {
      MoleculeList MoleculeNamesFor(string model, string transportName, IReadOnlyList<string> compoundNames);
   }
}