using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Parameters;

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
      private readonly IDiseaseStateUpdater _diseaseStateUpdater;

      public IndividualToIIndividualSettingsDTOMapper(
         IParameterToParameterDTOMapper parameterMapper,
         ISubPopulationToSubPopulationDTOMapper subPopulationDTOMapper,
         ICalculationMethodToCategoryCalculationMethodDTOMapper calculationMethodDTOMapper,
         IDiseaseStateUpdater diseaseStateUpdater
      )
      {
         _parameterMapper = parameterMapper;
         _subPopulationDTOMapper = subPopulationDTOMapper;
         _calculationMethodDTOMapper = calculationMethodDTOMapper;
         _diseaseStateUpdater = diseaseStateUpdater;
      }

      public IndividualSettingsDTO MapFrom(Individual individual)
      {
         var originData = individual.OriginData;
         var individualDTO = new IndividualSettingsDTO
         {
            Species = originData.Species,
            Population = originData.Population,
            SubPopulation = _subPopulationDTOMapper.MapFrom(originData.SubPopulation),
            Gender = originData.Gender,
            CalculationMethods = originData.AllCalculationMethods().MapAllUsing(_calculationMethodDTOMapper),
         };

         _diseaseStateUpdater.UpdateDiseaseStateDTO(individualDTO.DiseaseState, originData);

         individualDTO.UpdateValueOriginFrom(originData.ValueOrigin);

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