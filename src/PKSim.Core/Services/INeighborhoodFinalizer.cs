using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Services
{
   public interface INeighborhoodFinalizer
   {
      void SetNeighborsIn(Individual individual);
      void SetNeighborsIn(Organism organism, IEnumerable<NeighborhoodBuilder> neighborhoods);
   }
}