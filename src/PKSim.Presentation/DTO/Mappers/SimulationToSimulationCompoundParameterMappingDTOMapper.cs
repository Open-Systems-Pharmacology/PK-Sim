using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISimulationToSimulationCompoundParameterMappingDTOMapper
   {
      SimulationCompoundParameterMappingDTO MapFrom(Simulation simulation, Compound compound);
   }

   public class SimulationToSimulationCompoundParameterMappingDTOMapper : ISimulationToSimulationCompoundParameterMappingDTOMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public SimulationToSimulationCompoundParameterMappingDTOMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public SimulationCompoundParameterMappingDTO MapFrom(Simulation simulation, Compound compound)
      {
         var parameterMappingDTO = new SimulationCompoundParameterMappingDTO();

         var compoundGroupSelections = simulation.CompoundPropertiesFor(compound).CompoundGroupSelections.ToList();

         foreach (var compoundParameter in compound.AllParameterAlternativeGroups().Where(x => x.AllAlternatives.Any()))
         {
            var selectedAlternative = getCurrentAlternativeFor(compoundParameter, compoundGroupSelections);
            parameterMappingDTO.Add(parameterSelectionFrom(compoundParameter, selectedAlternative));
         }

         return parameterMappingDTO;
      }

      private ParameterAlternative getCurrentAlternativeFor(ParameterAlternativeGroup compoundParameter, IEnumerable<CompoundGroupSelection> compoundGroupSelections)
      {
         var currentMapping = compoundGroupSelections.FirstOrDefault(x => string.Equals(x.GroupName, compoundParameter.Name));
         if (currentMapping == null)
            return compoundParameter.DefaultAlternative;

         return compoundParameter.AlternativeByName(currentMapping.AlternativeName) ?? compoundParameter.DefaultAlternative;
      }

      private CompoundParameterSelectionDTO parameterSelectionFrom(ParameterAlternativeGroup compoundParameterGroup, ParameterAlternative parameterAlternative)
      {
         var compoundParameterSelectionDTO = new CompoundParameterSelectionDTO(compoundParameterGroup);
         compoundParameterSelectionDTO.ParameterName = _representationInfoRepository.InfoFor(compoundParameterGroup).DisplayName;
         compoundParameterSelectionDTO.SelectedAlternative = parameterAlternative;
         return compoundParameterSelectionDTO;
      }
   }
}