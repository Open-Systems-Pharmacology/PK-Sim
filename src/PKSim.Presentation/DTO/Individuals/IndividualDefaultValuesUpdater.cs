using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation.DTO.Individuals
{
   public interface IIndividualDefaultValueUpdater
   {
      void UpdateDefaultValueFor(IndividualSettingsDTO individualSettingsDTO);
      void UpdateMeanValueFor(IndividualSettingsDTO individualSettingsDTO);
      void UpdateSettingsAfterSpeciesChange(IndividualSettingsDTO individualSettingsDTO);
   }

   public class IndividualDefaultValuesUpdater : IIndividualDefaultValueUpdater
   {
      private readonly ICalculationMethodToCategoryCalculationMethodDTOMapper _calculationMethodDTOMapper;
      private readonly IPopulationRepository _populationRepository;
      private readonly IIndividualModelTask _individualModelTask;
      private readonly IIndividualSettingsDTOToOriginDataMapper _originDataMapper;
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly ISubPopulationToSubPopulationDTOMapper _subPopulationDTOMapper;
      private readonly IOriginDataTask _originDataTask;

      public IndividualDefaultValuesUpdater(
         IIndividualModelTask individualModelTask,
         IIndividualSettingsDTOToOriginDataMapper originDataMapper,
         IParameterToParameterDTOMapper parameterMapper,
         IOriginDataTask originDataTask,
         ISubPopulationToSubPopulationDTOMapper subPopulationDTOMapper,
         ICalculationMethodToCategoryCalculationMethodDTOMapper calculationMethodDTOMapper,
         IPopulationRepository populationRepository)
      {
         _individualModelTask = individualModelTask;
         _originDataMapper = originDataMapper;
         _parameterMapper = parameterMapper;
         _originDataTask = originDataTask;
         _subPopulationDTOMapper = subPopulationDTOMapper;
         _calculationMethodDTOMapper = calculationMethodDTOMapper;
         _populationRepository = populationRepository;
      }

      public void UpdateDefaultValueFor(IndividualSettingsDTO individualSettingsDTO)
      {
         individualSettingsDTO.SubPopulation = _subPopulationDTOMapper.MapFrom(_originDataTask.DefaultSubPopulationFor(individualSettingsDTO.Species));
         //Default parameter such as age etc.. were not defined yet
         var originData = _originDataMapper.MapFrom(individualSettingsDTO);
         var parameterAge = _individualModelTask.MeanAgeFor(originData);
         if (parameterAge != null)
            originData.Age = new OriginDataParameter(parameterAge.Value);

         var parameterGestationalAge = _individualModelTask.MeanGestationalAgeFor(originData);
         if (parameterGestationalAge != null)
            originData.GestationalAge = new OriginDataParameter(parameterGestationalAge.Value);

         setDefaultValues(individualSettingsDTO, originData, _parameterMapper.MapAsReadWriteFrom(parameterAge), _parameterMapper.MapAsReadWriteFrom(parameterGestationalAge));
      }

      public void UpdateMeanValueFor(IndividualSettingsDTO individualSettingsDTO)
      {
         var originData = _originDataMapper.MapFrom(individualSettingsDTO);
         setDefaultValues(individualSettingsDTO, originData, individualSettingsDTO.ParameterAge, individualSettingsDTO.ParameterGestationalAge);
      }

      private void setDefaultValues(IndividualSettingsDTO individualSettingsDTO, OriginData originData, IParameterDTO parameterAgeDTO, IParameterDTO parameterGestationalAge)
      {
         var parameterWeight = _individualModelTask.MeanWeightFor(originData);
         var parameterHeight = _individualModelTask.MeanHeightFor(originData);
         var parameterBMI = _individualModelTask.BMIBasedOn(originData, parameterWeight, parameterHeight);

         individualSettingsDTO.SetDefaultParameters(parameterAgeDTO,
            _parameterMapper.MapAsReadWriteFrom(parameterHeight),
            _parameterMapper.MapAsReadWriteFrom(parameterWeight),
            _parameterMapper.MapAsReadWriteFrom(parameterBMI),
            parameterGestationalAge);
      }

      public void UpdateSettingsAfterSpeciesChange(IndividualSettingsDTO individualSettingsDTO)
      {
         var species = individualSettingsDTO.Species;
         var population = _populationRepository.DefaultPopulationFor(species);
         individualSettingsDTO.Species = species;
         individualSettingsDTO.Population = population;
         individualSettingsDTO.Gender = population.DefaultGender;
         individualSettingsDTO.CalculationMethods = individualCalculationMethods(species);
      }

      private IEnumerable<CategoryCalculationMethodDTO> individualCalculationMethods(Species species)
      {
         return _originDataTask.DefaultCalculationMethodFor(species).MapAllUsing(_calculationMethodDTOMapper);
      }
   }
}