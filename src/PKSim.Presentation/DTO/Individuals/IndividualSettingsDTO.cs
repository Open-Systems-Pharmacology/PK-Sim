using System.Collections.Generic;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Individuals
{
   public class IndividualSettingsDTO
   {
      public IParameterDTO ParameterWeight { get; private set; }
      public IParameterDTO ParameterAge { get; private set; }
      public IParameterDTO ParameterGestationalAge { get; private set; }
      public IParameterDTO ParameterHeight { get; private set; }
      public IParameterDTO ParameterBMI { get; private set; }

      public SpeciesPopulation SpeciesPopulation { get; set; }
      public IEnumerable<CategoryParameterValueVersionDTO> SubPopulation { get; set; }
      public IEnumerable<CategoryCalculationMethodDTO> CalculationMethods { get; set; }
      public Species Species { get; set; }
      public Gender Gender { get; set; }

      public IndividualSettingsDTO()
      {
         ParameterWeight = new NullParameterDTO();
         ParameterAge = new NullParameterDTO();
         ParameterGestationalAge = new NullParameterDTO();
         ParameterHeight = new NullParameterDTO();
         ParameterBMI = new NullParameterDTO();
      }

      public void SetDefaultParameters(IParameterDTO parameterAge, IParameterDTO parameterHeight, IParameterDTO parameterWeight, IParameterDTO parameterBMI, IParameterDTO parameterGestationalAge)
      {
         ParameterWeight = parameterWeight;
         ParameterAge = parameterAge;
         ParameterHeight = parameterHeight;
         ParameterBMI = parameterBMI;
         ParameterGestationalAge = parameterGestationalAge;
      }
   }
}