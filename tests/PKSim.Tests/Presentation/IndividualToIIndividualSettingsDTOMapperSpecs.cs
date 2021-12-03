using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualToIIndividualSettingsDTOMapper : ContextSpecification<IIndividualToIIndividualSettingsDTOMapper>
   {
      protected IParameterToParameterDTOMapper _parameterMapper;
      protected IGenderRepository _genderRepository;
      protected Species _species;
      protected SpeciesPopulation _speciesPopulation;
      protected ISubPopulationToSubPopulationDTOMapper _subPopulationDTOMapper;
      protected ICalculationMethodToCategoryCalculationMethodDTOMapper _calculationMethodDTOMapper;
      protected IDiseaseStateRepository _diseaseStateRepository;
      protected IOriginDataParameterToParameterDTOMapper _originDataParameterMapper;

      protected override void Context()
      {
         _parameterMapper = A.Fake<IParameterToParameterDTOMapper>();
         _subPopulationDTOMapper = A.Fake<ISubPopulationToSubPopulationDTOMapper>();
         _calculationMethodDTOMapper = A.Fake<ICalculationMethodToCategoryCalculationMethodDTOMapper>();
         _diseaseStateRepository = A.Fake<IDiseaseStateRepository>();
         _originDataParameterMapper = A.Fake<IOriginDataParameterToParameterDTOMapper>();

         _species = new Species {Name = "species"};
         _speciesPopulation = new SpeciesPopulation {Name = "population"};
         _speciesPopulation.AddGender(new Gender {Name = "gender"});
         _species.AddPopulation(_speciesPopulation);
         sut = new IndividualToIIndividualSettingsDTOMapper(
            _parameterMapper,
            _subPopulationDTOMapper,
            _calculationMethodDTOMapper,
            _diseaseStateRepository,
            _originDataParameterMapper
         );
      }
   }

   public class When_mapping_an_individual_to_an_individual_dto : concern_for_IndividualToIIndividualSettingsDTOMapper
   {
      private Individual _individual;
      private IndividualSettingsDTO _result;
      private OriginData _origin;
      private Organism _organism;
      private IParameter _parameterHeight;
      private IParameter _parameterWeight;
      private IParameter _parameterAge;
      private ParameterDTO _parameterAgeDTO;
      private ParameterDTO _parameterHeightDTO;
      private ParameterDTO _parameterWeightDTO;
      private IEnumerable<CategoryParameterValueVersionDTO> _subPopulationDTO;
      private CategoryCalculationMethodDTO _cmDTO1;

      protected override void Context()
      {
         base.Context();
         _organism = new Organism();
         _individual = A.Fake<Individual>();
         _origin = new OriginData();
         var cm1 = new CalculationMethod {Id = "1", Name = "1"};
         _cmDTO1 = new CategoryCalculationMethodDTO();
         A.CallTo(_calculationMethodDTOMapper).WithReturnType<CategoryCalculationMethodDTO>().Returns(_cmDTO1);
         _origin.Species = new Species();
         _origin.Population = new SpeciesPopulation();
         _origin.SubPopulation = new SubPopulation();
         _origin.AddCalculationMethod(cm1);
         _origin.Gender = new Gender();
         _subPopulationDTO = A.Fake<IEnumerable<CategoryParameterValueVersionDTO>>();
         _parameterAge = A.Fake<IParameter>().WithName(CoreConstants.Parameters.AGE);
         _parameterHeight = A.Fake<IParameter>().WithName(CoreConstants.Parameters.HEIGHT);
         _parameterWeight = A.Fake<IParameter>().WithName(CoreConstants.Parameters.WEIGHT);
         _parameterAgeDTO = A.Fake<ParameterDTO>();
         _parameterHeightDTO = A.Fake<ParameterDTO>();
         _parameterWeightDTO = A.Fake<ParameterDTO>();
         A.CallTo(() => _subPopulationDTOMapper.MapFrom(_origin.SubPopulation)).Returns(_subPopulationDTO);
         A.CallTo(() => _parameterMapper.MapAsReadWriteFrom(_parameterAge)).Returns(_parameterAgeDTO);
         A.CallTo(() => _parameterMapper.MapAsReadWriteFrom(_parameterHeight)).Returns(_parameterHeightDTO);
         A.CallTo(() => _parameterMapper.MapAsReadWriteFrom(_parameterWeight)).Returns(_parameterWeightDTO);
         A.CallTo(() => _individual.Organism).Returns(_organism);

         _individual.OriginData = _origin;
         _organism.AddChildren(_parameterWeight, _parameterHeight, _parameterAge);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_individual);
      }

      [Observation]
      public void should_map_the_elements_of_the_individual_correctly_to_the_individual_dto_properties()
      {
         _result.Species.ShouldBeEqualTo(_individual.OriginData.Species);
         _result.Population.ShouldBeEqualTo(_individual.OriginData.Population);
         _result.SubPopulation.ShouldBeEqualTo(_subPopulationDTO);
         _result.Gender.ShouldBeEqualTo(_individual.OriginData.Gender);
         _result.ParameterWeight.ShouldBeEqualTo(_parameterWeightDTO);
         _result.ParameterHeight.ShouldBeEqualTo(_parameterHeightDTO);
         _result.ParameterAge.ShouldBeEqualTo(_parameterAgeDTO);
         _result.CalculationMethods.ShouldOnlyContain(_cmDTO1);
      }
      
      [Observation]
      public void should_set_a_disease_state_to_healthy_for_an_individual_without_disease_state()
      {
         _result.DiseaseState.ShouldBeEqualTo(_diseaseStateRepository.HealthyState);
      }

      [Observation]
      public void should_set_the_disease_state_parameter_to_a_null_parameter()
      {
         _result.DiseaseStateParameter.IsNull().ShouldBeTrue();
      }
   }

   public class When_mapping_an_individual_with_disease_state_parameter_to_an_individual_dto : concern_for_IndividualToIIndividualSettingsDTOMapper
   {
      private Individual _individual;
      private IndividualSettingsDTO _result;
      private OriginData _origin;
      private Organism _organism;
      private IParameter _parameterHeight;
      private IParameter _parameterWeight;
      private IParameter _parameterAge;
      private OriginDataParameter _diseaseStateParameter;
      private IParameterDTO _diseaseStateParameterDTO;

      protected override void Context()
      {
         base.Context();
         _organism = new Organism();
         _individual = A.Fake<Individual>();
         _diseaseStateParameterDTO= A.Fake<IParameterDTO>();
         A.CallTo(() => _individual.Organism).Returns(_organism);
         _origin = new OriginData
         {
            DiseaseState = new DiseaseState {Name = "MyDiseaseState"}
         };
         _diseaseStateParameter = new OriginDataParameter();
         _origin.AddDiseaseStateParameter(_diseaseStateParameter);
         A.CallTo(() => _originDataParameterMapper.MapFrom(_diseaseStateParameter)).Returns(_diseaseStateParameterDTO);
         _individual.OriginData = _origin;
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_individual);
      }
      
      [Observation]
      public void should_set_take_the_disease_state_from_the_individual()
      {
         _result.DiseaseState.ShouldBeEqualTo(_origin.DiseaseState);
      }

      [Observation]
      public void should_update_the_disease_state_parameter()
      {
         _result.DiseaseStateParameter.ShouldBeEqualTo(_diseaseStateParameterDTO);
      }
   }

}