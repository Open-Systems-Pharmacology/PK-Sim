using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISimulationToCompoundCommitDTOMapper
   {
      CompoundCommitDTO MapFrom(Simulation simulation, Compound templateCompound);
   }

   public class SimulationToCompoundCommitDTOMapper : ISimulationToCompoundCommitDTOMapper
   {
      private readonly IContainerTask _containerTask;
      private readonly IParameterToParameterCommitDTOMapper _parameterCommitDTOMapper;

      public SimulationToCompoundCommitDTOMapper(
         IContainerTask containerTask,
         IParameterToParameterCommitDTOMapper parameterCommitDTOMapper)
      {
         _containerTask = containerTask;
         _parameterCommitDTOMapper = parameterCommitDTOMapper;
      }

      public CompoundCommitDTO MapFrom(Simulation simulation, Compound templateCompound)
      {
         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);

         var changedPaths = simulation.ParameterChangeTracker.ChangedPaths
            .Select(p => p.PathAsString)
            .Where(path => simulation.CompoundNameForParameterPath(path) == templateCompound.Name)
            .ToList();

         if (!changedPaths.Any())
            return null;

         var selectedSetInTemplate = selectedSetInTemplateFor(simulation, templateCompound);

         return new CompoundCommitDTO
         {
            CompoundName = templateCompound.Name,
            Compound = templateCompound,
            AvailableExistingSets = templateCompound.OverwriteParameterSets,
            CreateNew = selectedSetInTemplate == null,
            SelectedExistingSet = selectedSetInTemplate,
            SetSelectedInSimulation = selectedSetInTemplate,
            NewSetName = templateCompound.Name,
            Parameters = changedPaths.Select(path => mapParameter(simulation, templateCompound.Name, path, parameterCache[path])).ToList()
         };
      }

      private ParameterCommitDTO mapParameter(Simulation simulation, string compoundName, string path, IParameter parameter)
      {
         var isRemoval = simulation.IsOverwrittenParameterPath(compoundName, path) && parameter is { IsDefault: true };
         return _parameterCommitDTOMapper.MapFrom(path, parameter, isRemoval);
      }

      /// <summary>
      ///    Returns the set of <paramref name="templateCompound" /> selected for the compound in the simulation, matched by
      ///    name. The selection holds the set of the compound in the simulation when it was made in the simulation
      ///    configuration, and the set of the template compound when it was restored from a snapshot, so the two cannot be
      ///    compared by reference.
      /// </summary>
      private OverwriteParameterSet selectedSetInTemplateFor(Simulation simulation, Compound templateCompound)
      {
         var selection = simulation.OverwriteParameterSetSelections.SelectedSetFor(templateCompound.Name);
         return selection == null ? null : templateCompound.OverwriteParameterSets.FindByName(selection.Name);
      }
   }
}
