using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using OSPSuite.Core.Domain;

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

      public IndividualToIIndividualSettingsDTOMapper(IParameterToParameterDTOMapper parameterMapper,
         ISubPopulationToSubPopulationDTOMapper subPopulationDTOMapper,
         ICalculationMethodToCategoryCalculationMethodDTOMapper calculationMethodDTOMapper)
      {
         _parameterMapper = parameterMapper;
         _subPopulationDTOMapper = subPopulationDTOMapper;
         _calculationMethodDTOMapper = calculationMethodDTOMapper;
      }

      public IndividualSettingsDTO MapFrom(Individual individual)
      {
         var individualDTO = new IndividualSettingsDTO
         {
            Species = individual.OriginData.Species,
            SpeciesPopulation = individual.OriginData.SpeciesPopulation,
            SubPopulation = _subPopulationDTOMapper.MapFrom(individual.OriginData.SubPopulation),
            Gender = individual.OriginData.Gender,
            CalculationMethods = individual.OriginData.AllCalculationMethods().MapAllUsing(_calculationMethodDTOMapper)
         };

         individualDTO.SetDefaultParameters(_parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameter.AGE)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameter.HEIGHT)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameter.WEIGHT)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameter.BMI)),
            _parameterMapper.MapAsReadWriteFrom(individual.Organism.Parameter(CoreConstants.Parameter.GESTATIONAL_AGE)));


         return individualDTO;
      }
   }
}