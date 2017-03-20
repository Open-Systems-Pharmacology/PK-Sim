using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IPopulationRepository : IStartableRepository<SpeciesPopulation>
   {
      SpeciesPopulation DefaultPopulationFor(Species species);
      SpeciesPopulation FindByIndex(int raceIndex);
   }
}