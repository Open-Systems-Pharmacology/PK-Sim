using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IDiseaseStateRepository : IStartableRepository<DiseaseState>
   {
      IReadOnlyList<DiseaseState> AllFor(SpeciesPopulation population);
      IReadOnlyList<DiseaseState> AllForExpressionProfile(Species species, QuantityType moleculeType);

      DiseaseState HealthyState { get; }
   }
}