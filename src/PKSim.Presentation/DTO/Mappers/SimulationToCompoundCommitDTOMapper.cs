using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISimulationToCompoundCommitDTOMapper
   {
      CompoundCommitDTO MapFrom(Simulation simulation, Compound compound);
   }

   public class SimulationToCompoundCommitDTOMapper : ISimulationToCompoundCommitDTOMapper
   {
      private readonly IContainerTask _containerTask;
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private readonly IParameterToParameterCommitDTOMapper _parameterCommitDTOMapper;

      public SimulationToCompoundCommitDTOMapper(
         IContainerTask containerTask,
         IBuildingBlockInProjectManager buildingBlockInProjectManager,
         IParameterToParameterCommitDTOMapper parameterCommitDTOMapper)
      {
         _containerTask = containerTask;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _parameterCommitDTOMapper = parameterCommitDTOMapper;
      }

      public CompoundCommitDTO MapFrom(Simulation simulation, Compound compound)
      {
         var templateCompound = templateCompoundFor(simulation, compound.Name);
         if (templateCompound == null)
            return null;

         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);

         var changedPaths = simulation.ParameterChangeTracker.ChangedPaths
            .Select(p => p.PathAsString)
            .Where(path => simulation.CompoundNameForParameterPath(path) == compound.Name)
            .ToList();

         if (!changedPaths.Any())
            return null;

         var existingSelection = simulation.OverwriteParameterSetSelections.SelectedSetFor(compound.Name);
         var hasExistingSelection = existingSelection != null && templateCompound.OverwriteParameterSets.Contains(existingSelection);

         return new CompoundCommitDTO
         {
            CompoundName = compound.Name,
            TemplateCompound = templateCompound,
            AvailableExistingSets = templateCompound.OverwriteParameterSets,
            CreateNew = !hasExistingSelection,
            SelectedExistingSet = hasExistingSelection ? existingSelection : null,
            NewSetName = compound.Name,
            Parameters = changedPaths.Select(path => _parameterCommitDTOMapper.MapFrom(path, parameterCache[path])).ToList()
         };
      }

      private Compound templateCompoundFor(Simulation simulation, string compoundName)
      {
         var simulationCompound = simulation.Compounds.FindByName(compoundName);
         if (simulationCompound == null)
            return null;

         return _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy<Compound>(simulation, simulationCompound);
      }
   }
}
