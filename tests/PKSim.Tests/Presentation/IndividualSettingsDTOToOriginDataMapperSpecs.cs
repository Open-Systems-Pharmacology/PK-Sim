using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;

using PKSim.Presentation.DTO.Mappers;

using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualSettingsDTOToOriginDataMapper : ContextSpecification<IIndividualSettingsDTOToOriginDataMapper>
   {
      protected IndividualSettingsDTO _individualSettingsDTO;

      protected override void Context()
      {
         _individualSettingsDTO = new IndividualSettingsDTO();
         sut = new IndividualSettingsDTOToOriginDataMapper();
      }
   }

   
   public class When_mapping_an_individual_dto_to_origin_data : concern_for_IndividualSettingsDTOToOriginDataMapper
   {
      private OriginData _result;
      private Species _species;
      private SpeciesPopulation _speciesPopulation;
      private Gender _gender;
      private ParameterDTO _parameterHeight;
      private ParameterDTO _parameterWeight;
      private ParameterDTO _parameterAge;
      private ParameterDTO _parameterGestationalAge;
      private IEnumerable<CategoryParameterValueVersionDTO> _subPopulationDTO;
      private ParameterValueVersion _pvv1;
      private ParameterValueVersion _pvv2;
      private ParameterDTO _parameterBMI;

      protected override void Context()
      {
         base.Context();
         _pvv1 =new ParameterValueVersion().WithName("PV1");
         _pvv2 = new ParameterValueVersion().WithName("PV2");
         _parameterHeight = A.Fake<ParameterDTO>();
         A.CallTo(() => _parameterHeight.KernelValue).Returns(5);
         _parameterWeight = A.Fake<ParameterDTO>();
         A.CallTo(() => _parameterWeight.KernelValue).Returns(6);
         _parameterAge = A.Fake<ParameterDTO>();
         A.CallTo(() => _parameterAge.KernelValue).Returns(7);
         _parameterBMI = A.Fake<ParameterDTO>();
         A.CallTo(() => _parameterBMI.KernelValue).Returns(10);
         _parameterGestationalAge = A.Fake<ParameterDTO>();
         A.CallTo(() => _parameterGestationalAge.KernelValue).Returns(25);
         _individualSettingsDTO.SetDefaultParameters(_parameterAge, _parameterHeight, _parameterWeight, _parameterBMI,_parameterGestationalAge);
         _individualSettingsDTO.CalculationMethods = new List<CategoryCalculationMethodDTO>();
         _species = A.Fake<Species>();
         _speciesPopulation = A.Fake<SpeciesPopulation>();
         _gender = A.Fake<Gender>();
         _subPopulationDTO = new List<CategoryParameterValueVersionDTO> { new CategoryParameterValueVersionDTO { ParameterValueVersion = _pvv1 }, new CategoryParameterValueVersionDTO { ParameterValueVersion = _pvv2 } };
         _individualSettingsDTO.Species = _species;
         _individualSettingsDTO.SubPopulation = _subPopulationDTO;
         _individualSettingsDTO.SpeciesPopulation = _speciesPopulation;
         _individualSettingsDTO.Gender = _gender;
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_individualSettingsDTO);
      }

      [Observation]
      public void the_returned_origin_data_should_be_filled_with_the_individual_properties()
      {
         _result.Species.ShouldBeEqualTo(_individualSettingsDTO.Species);
         _result.SpeciesPopulation.ShouldBeEqualTo(_individualSettingsDTO.SpeciesPopulation);
         _result.SubPopulation.ParameterValueVersions.ShouldOnlyContain(_pvv1,_pvv2);
         _result.Gender.ShouldBeEqualTo(_individualSettingsDTO.Gender);
         _result.Age.ShouldBeEqualTo(_parameterAge.KernelValue);
         _result.GestationalAge.ShouldBeEqualTo(_parameterGestationalAge.KernelValue);
         _result.Height.ShouldBeEqualTo(_parameterHeight.KernelValue);
         _result.Weight.ShouldBeEqualTo(_parameterWeight.KernelValue);
         _result.SpeciesPopulation.ShouldBeEqualTo(_individualSettingsDTO.SpeciesPopulation);
      }
   }
}