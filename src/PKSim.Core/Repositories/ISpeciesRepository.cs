using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ISpeciesRepository : IStartableRepository<Species>
   {
      Species DefaultSpecies { get; }
   }
}