using System.Collections.Generic;
using System.Data;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IDiseaseStateRepository : IStartableRepository<DiseaseState>
   {
      IReadOnlyList<DiseaseState> AllFor(SpeciesPopulation population);
   }
}