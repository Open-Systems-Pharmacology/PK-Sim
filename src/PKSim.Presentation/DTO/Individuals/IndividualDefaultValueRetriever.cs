using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation.DTO.Individuals
{
   public interface IIndividualDefaultValueRetriever
   {
      Species DefaultSpecies();
      SpeciesPopulation DefaultPopulationFor(Species species);
      Gender DefaultGenderFor(SpeciesPopulation speciesPopulation);
      void RetrieveDefaultValueFor(IndividualSettingsDTO individualSettingsDTO);
      void RetrieveMeanValueFor(IndividualSettingsDTO individualSettingsDTO);
      IndividualSettingsDTO DefaultSettings();
      IndividualSettingsDTO DefaultSettingFor(Species species);
      IndividualSettingsDTO DefaultSettingFor(SpeciesPopulation speciesPopulation);
      void UpdateSettingsAfterSpeciesChange(IndividualSettingsDTO individualSettingsDTO);
   }

   public class IndividualDefaultValueRetriever : IIndividualDefaultValueRetriever
   {
      private readonly ICalculationMethodToCategoryCalculationMethodDTOMapper _calculationMethodDTOMapper;
      private readonly IPopulationRepository _populationRepository;
      private readonly IIndividualModelTask _individualModelTask;
      private readonly IIndividualSettingsDTOToOriginDataMapper _originDataMapper;
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ISubPopulationToSubPopulationDTOMapper _subPopulationDTOMapper;
      private readonly IOriginDataTask _originDataTask;

      public IndividualDefaultValueRetriever(IIndividualModelTask individualModelTask,
         IIndividualSettingsDTOToOriginDataMapper originDataMapper,
         IParameterToParameterDTOMapper parameterMapper,
         IOriginDataTask originDataTask,
         ISubPopulationToSubPopulationDTOMapper subPopulationDTOMapper,
         ISpeciesRepository speciesRepository,
         ICalculationMethodToCategoryCalculationMethodDTOMapper calculationMethodDTOMapper, IPopulationRepository populationRepository)
      {
         _individualModelTask = individualModelTask;
         _originDataMapper = originDataMapper;
         _parameterMapper = parameterMapper;
         _originDataTask = originDataTask;
         _subPopulationDTOMapper = subPopulationDTOMapper;
         _speciesRepository = speciesRepository;
         _calculationMethodDTOMapper = calculationMethodDTOMapper;
         _populationRepository = populationRepository;
      }

      public Species DefaultSpecies()
      {
         return _speciesRepository.DefaultSpecies;
      }

      public SpeciesPopulation DefaultPopulationFor(Species species)
      {
         return _populationRepository.DefaultPopulationFor(species);
      }

      public Gender DefaultGenderFor(SpeciesPopulation speciesPopulation)
      {
         return speciesPopulation.Genders.ElementAt(0);
      }

      public void RetrieveDefaultValueFor(IndividualSettingsDTO individualSettingsDTO)
      {
         individualSettingsDTO.SubPopulation = _subPopulationDTOMapper.MapFrom(_originDataTask.DefaultSubPopulationFor(individualSettingsDTO.Species));
         //Default parameter such as age etc.. were not defined yet
         var originData = _originDataMapper.MapFrom(individualSettingsDTO);
         var parameterAge = _parameterMapper.MapAsReadWriteFrom(_individualModelTask.MeanAgeFor(originData));
         originData.Age = parameterAge.KernelValue;

         var parameterGestationalAge = _parameterMapper.MapAsReadWriteFrom(_individualModelTask.MeanGestationalAgeFor(originData));
         originData.GestationalAge = parameterGestationalAge.KernelValue;
         setDefaultValues(individualSettingsDTO, originData, parameterAge, parameterGestationalAge);
      }

      public void RetrieveMeanValueFor(IndividualSettingsDTO individualSettingsDTO)
      {
         OriginData originData = _originDataMapper.MapFrom(individualSettingsDTO);
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

      public IndividualSettingsDTO DefaultSettings()
      {
         return DefaultSettingFor(DefaultSpecies());
      }

      public IndividualSettingsDTO DefaultSettingFor(Species species)
      {
         var speciesPopulation = DefaultPopulationFor(species);
         return defaultSettingsFor(species, speciesPopulation);
      }

      public IndividualSettingsDTO DefaultSettingFor(SpeciesPopulation speciesPopulation)
      {
         var species = _speciesRepository.FindByName(speciesPopulation.Species);
         return defaultSettingsFor(species, speciesPopulation);
      }

      private IndividualSettingsDTO defaultSettingsFor(Species species, SpeciesPopulation speciesPopulation)
      {
         var individualSettingsDTO = new IndividualSettingsDTO();
         updateSettingsFor(individualSettingsDTO, species, speciesPopulation);
         RetrieveDefaultValueFor(individualSettingsDTO);
         return individualSettingsDTO;
      }

      public void UpdateSettingsAfterSpeciesChange(IndividualSettingsDTO individualSettingsDTO)
      {
         updateSettingsFor(individualSettingsDTO, individualSettingsDTO.Species, DefaultPopulationFor(individualSettingsDTO.Species));
      }

      private void updateSettingsFor(IndividualSettingsDTO individualSettingsDTO, Species species, SpeciesPopulation speciesPopulation)
      {
         individualSettingsDTO.Species = species;
         individualSettingsDTO.SpeciesPopulation = speciesPopulation;
         individualSettingsDTO.Gender = DefaultGenderFor(speciesPopulation);
         individualSettingsDTO.CalculationMethods = individualCalculationMethods(species);
      }

      private IEnumerable<CategoryCalculationMethodDTO> individualCalculationMethods(Species species)
      {
         return _originDataTask.DefaultCalculationMethodFor(species).MapAllUsing(_calculationMethodDTOMapper);
      }
   }
}