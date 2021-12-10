using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IDiseaseStateImplementationFactory
   {
      IDiseaseStateImplementation CreateFor(DiseaseState diseaseState);
      IDiseaseStateImplementation CreateFor(Individual individual);
   }
}