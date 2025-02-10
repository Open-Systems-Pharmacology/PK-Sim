using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IDiseaseStateImplementationRepository
   {
      IDiseaseStateImplementation FindFor(DiseaseState diseaseState);
      IDiseaseStateImplementation FindFor(Individual individual);
   }
}