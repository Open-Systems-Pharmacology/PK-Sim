using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualToIIndividualSettingsDTOMapper : IMapper<Individual, IndividualSettingsDTO>
   {
   }

   public class IndividualToIIndividualSettingsDTOMapper : IIndividualToIIndividualSettingsDTOMapper
   {
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly ISubPopulationToSubPopulationDTOMapper _subPopulationDTOMapper;
      private readonly ICalculationMethodToCategoryCalculationMethodDTOMapper _calculationMethodDTOMapper;
      private readonly IDiseaseStateRepository _diseaseStateRepository;

      public IndividualToIIndividualSettingsDTOMapper(
         IParameterToParameterDTOMapper parameterMapper,
         ISubPopulationToSubPopulationDTOMapper subPopulationDTOMapper,
         ICalculationMethodToCategoryCalculationMethodDTOMapper calculationMethodDTOMapper,
         IDiseaseStateRepository diseaseStateRepository
         )
      {
         _parameterMapper = parameterMapper;
         _subPopulationDTOMapper = subPopulationDTOMapper;
         _calculationMethodDTOMapper = calculationMethodDTOMapper;
         _diseaseStateRepository = diseaseStateRepository;
      }

      public IndividualSettingsDTO MapFrom(Individual individual)
      {
         var individualDTO = new IndividualSettingsDTO
         {
            Species = individual.OriginData.Species,
            Population = individual.OriginData.SpeciesPopulation,
            SubPopulation = _subPopulationDTOMapper.MapFrom(individual.OriginData.SubPopulation),
            Gender = individual.OriginData.Gender,
            CalculationMethods = individual.OriginData.AllCalculationMethods().MapAllUsing(_calculationMethodDTOMapper),
            //TODO

            DiseaseState = _diseaseStateRepository.HealthyState
         };

         individualDTO.UpdateValueOriginFrom(individual.OriginData.ValueOrigin);

         individualDTO.SetDefaultParameters(
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameters.AGE)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameters.HEIGHT)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameters.WEIGHT)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameters.BMI)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(Constants.Parameters.GESTATIONAL_AGE)));


         return individualDTO;
      }
   }
}