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

         var selectedSetInTemplate = selectedSetInTemplateFor(simulation, compound.Name, templateCompound);

         return new CompoundCommitDTO
         {
            CompoundName = compound.Name,
            Compound = templateCompound,
            AvailableExistingSets = templateCompound.OverwriteParameterSets,
            CreateNew = selectedSetInTemplate == null,
            SelectedExistingSet = selectedSetInTemplate,
            NewSetName = compound.Name,
            Parameters = changedPaths.Select(path => _parameterCommitDTOMapper.MapFrom(path, parameterCache[path])).ToList()
         };
      }

      /// <summary>
      ///    Returns the set of <paramref name="templateCompound" /> selected for the compound in the simulation, matched by
      ///    name. The selection holds the set of the compound in the simulation when it was made in the simulation
      ///    configuration, and the set of the template compound when it was restored from a snapshot, so the two cannot be
      ///    compared by reference.
      /// </summary>
      private OverwriteParameterSet selectedSetInTemplateFor(Simulation simulation, string compoundName, Compound templateCompound)
      {
         var selection = simulation.OverwriteParameterSetSelections.SelectedSetFor(compoundName);
         return selection == null ? null : templateCompound.OverwriteParameterSets.FindByName(selection.Name);
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
