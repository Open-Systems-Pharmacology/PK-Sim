using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class MolarToMassConcentrationDimensionForParameterConverter : PKAnalysesMolarToMassConverter
   {
      public MolarToMassConcentrationDimensionForParameterConverter(IParameter parameter, IDimensionRepository dimensionRepository)
         : base(parameter, dimensionRepository.MolarConcentration, dimensionRepository.MassConcentration)
      {
      }
   }
}