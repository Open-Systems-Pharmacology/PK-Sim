using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;

using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualDefaultValueRetriever : ContextSpecification<IIndividualDefaultValueUpdater>
   {
      protected IIndividualSettingsDTOToOriginDataMapper _originDataMapper;
      protected IIndividualModelTask _individualModelTask;
      protected IParameterToParameterDTOMapper _parameterMapper;
      protected IOriginDataTask _originDataTask;
      protected ISubPopulationToSubPopulationDTOMapper _subPopulationMapper;
      private ICalculationMethodToCategoryCalculationMethodDTOMapper _calculationMethodMapper;
      protected IPopulationRepository _populationRepository;
      protected ICloner _cloner;
      protected IDiseaseStateRepository _diseaseStateRepository;

      protected override void Context()
      {
         _originDataMapper = A.Fake<IIndividualSettingsDTOToOriginDataMapper>();
         _individualModelTask = A.Fake<IIndividualModelTask>();
         _parameterMapper = A.Fake<IParameterToParameterDTOMapper>();
         _originDataTask = A.Fake<IOriginDataTask>();
         _subPopulationMapper = A.Fake<ISubPopulationToSubPopulationDTOMapper>();
         _calculationMethodMapper = A.Fake<ICalculationMethodToCategoryCalculationMethodDTOMapper>();
         _populationRepository = A.Fake<IPopulationRepository>();
         _cloner= A.Fake<ICloner>();
         _diseaseStateRepository= A.Fake<IDiseaseStateRepository>(); 

         sut = new IndividualDefaultValuesUpdater(
            _individualModelTask,
            _originDataMapper,
            _parameterMapper,
            _originDataTask,
            _subPopulationMapper,
            _calculationMethodMapper,
            _populationRepository,
            _cloner,
            _diseaseStateRepository);
      }
   }

   public class When_retrieving_the_default_value_for_an_individual_dto : concern_for_IndividualDefaultValueRetriever
   {
      private IndividualSettingsDTO _individualSettingsDTO;
      private OriginData _originData;
      private IParameter _meanAge;
      private IParameter _meanWeight;
      private IParameter _meanHeight;
      private ParameterDTO _meanAgeDTO;
      private ParameterDTO _meanWeightDTO;
      private ParameterDTO _meanHeightDTO;
      private SubPopulation _subPopulation;
      private IEnumerable<CategoryParameterValueVersionDTO> _subPopulationDTO;
      private IParameter _meanGestationalAge;
      private ParameterDTO _meanGestationalAgeDTO;
      private const double _ageKernelValue = 15;
      private const double _gestationalAgeKernelValue = 24;

      protected override void Context()
      {
         base.Context();
         _subPopulation = new SubPopulation();
         _individualSettingsDTO = new IndividualSettingsDTO();
         _originData = new OriginData {Population = new SpeciesPopulation()};
         _meanAge = A.Fake<IParameter>();
         _meanWeight = A.Fake<IParameter>();
         _meanHeight = A.Fake<IParameter>();
         _meanGestationalAge = A.Fake<IParameter>();
         _meanAgeDTO = A.Fake<ParameterDTO>();
         _meanWeightDTO = A.Fake<ParameterDTO>();
         _meanHeightDTO = A.Fake<ParameterDTO>();
         _meanGestationalAgeDTO = A.Fake<ParameterDTO>();
         _individualSettingsDTO.Species = A.Fake<Species>();
         A.CallTo(() => _meanAge.Value).Returns(_ageKernelValue);
         A.CallTo(() => _meanGestationalAge.Value).Returns(_gestationalAgeKernelValue);
         A.CallTo(() => _originDataMapper.MapFrom(_individualSettingsDTO)).Returns(_originData);
         A.CallTo(() => _originDataTask.DefaultSubPopulationFor(_individualSettingsDTO.Species)).Returns(_subPopulation);
         A.CallTo(() => _individualModelTask.MeanAgeFor(_originData)).Returns(_meanAge);
         A.CallTo(() => _individualModelTask.MeanWeightFor(_originData)).Returns(_meanWeight);
         A.CallTo(() => _individualModelTask.MeanHeightFor(_originData)).Returns(_meanHeight);
         A.CallTo(() => _individualModelTask.MeanGestationalAgeFor(_originData)).Returns(_meanGestationalAge);
         _subPopulationDTO = A.Fake<IEnumerable<CategoryParameterValueVersionDTO>>();
         A.CallTo(() => _subPopulationMapper.MapFrom(_subPopulation)).Returns(_subPopulationDTO);
         A.CallTo(() => _parameterMapper.MapAsReadWriteFrom((_meanAge))).Returns(_meanAgeDTO);
         A.CallTo(() => _parameterMapper.MapAsReadWriteFrom(_meanWeight)).Returns(_meanWeightDTO);
         A.CallTo(() => _parameterMapper.MapAsReadWriteFrom(_meanHeight)).Returns(_meanHeightDTO);
         A.CallTo(() => _parameterMapper.MapAsReadWriteFrom(_meanGestationalAge)).Returns(_meanGestationalAgeDTO);
      }

      protected override void Because()
      {
         sut.UpdateDefaultValueFor(_individualSettingsDTO);
      }

      [Observation]
      public void should_retrieve_for_the_individual_the_default_values()
      {
         _individualSettingsDTO.ParameterAge.ShouldBeEqualTo(_meanAgeDTO);
         _individualSettingsDTO.ParameterHeight.ShouldBeEqualTo(_meanHeightDTO);
         _individualSettingsDTO.ParameterWeight.ShouldBeEqualTo(_meanWeightDTO);
      }

      [Observation]
      public void should_retrieve_the_default_population_for_the_species()
      {
         _individualSettingsDTO.SubPopulation.ShouldBeEqualTo(_subPopulationDTO);
      }

      [Observation]
      public void should_have_used_the_default_kernel_value_for_age_and_gestational_age_to_retrieve_the_default_parameter_values()
      {
         _originData.Age.Value.ShouldBeEqualTo(_ageKernelValue);
         _originData.GestationalAge.Value.ShouldBeEqualTo(_gestationalAgeKernelValue);
      }
   }
}