using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class AucMolarToAucMassDimensionForParameterConverter : PKAnalysesMolarToMassConverter
   {
      public AucMolarToAucMassDimensionForParameterConverter(IParameter parameter, IDimensionRepository dimensionRepository) :
         base(parameter, dimensionRepository.AucMolar, dimensionRepository.Auc)
      {
      }
   }
}