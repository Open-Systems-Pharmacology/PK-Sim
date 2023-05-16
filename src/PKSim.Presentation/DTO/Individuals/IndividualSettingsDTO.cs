using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Individuals
{
   public class IndividualSettingsDTO : IWithValueOrigin
   {
      public IParameterDTO ParameterWeight { get; private set; } = new NullParameterDTO();
      public IParameterDTO ParameterAge { get; private set; } = new NullParameterDTO();
      public IParameterDTO ParameterGestationalAge { get; private set; } = new NullParameterDTO();
      public IParameterDTO ParameterHeight { get; private set; } = new NullParameterDTO();
      public IParameterDTO ParameterBMI { get; private set; } = new NullParameterDTO();
      public ValueOrigin ValueOrigin { get; } = new ValueOrigin();
      public DiseaseStateDTO DiseaseState { get; } = new DiseaseStateDTO();

      public SpeciesPopulation Population { get; set; }
      public IEnumerable<CategoryParameterValueVersionDTO> SubPopulation { get; set; }
      public IEnumerable<CategoryCalculationMethodDTO> CalculationMethods { get; set; }
      public Species Species { get; set; }
      public Gender Gender { get; set; }

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