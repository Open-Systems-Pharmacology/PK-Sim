using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Individuals
{
   public class IndividualSettingsDTO : IWithValueOrigin
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
      public ValueOrigin ValueOrigin { get; }

      public IndividualSettingsDTO()
      {
         ParameterWeight = new NullParameterDTO();
         ParameterAge = new NullParameterDTO();
         ParameterGestationalAge = new NullParameterDTO();
         ParameterHeight = new NullParameterDTO();
         ParameterBMI = new NullParameterDTO();
         ValueOrigin = new ValueOrigin();
      }

      public void SetDefaultParameters(IParameterDTO parameterAge, IParameterDTO parameterHeight, IParameterDTO parameterWeight, IParameterDTO parameterBMI, IParameterDTO parameterGestationalAge)
      {
         ParameterWeight = parameterWeight;
         ParameterAge = parameterAge;
         ParameterHeight = parameterHeight;
         ParameterBMI = parameterBMI;
         ParameterGestationalAge = parameterGestationalAge;
      }

      public void UpdateValueOriginFrom(ValueOrigin sourceValueOrigin) => ValueOrigin.UpdateFrom(sourceValueOrigin);
   }
}