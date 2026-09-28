using System.Collections.Generic;
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

         var parameters = changedPaths.Select(path => mapParameter(simulation, templateCompound.Name, path, parameterCache[path])).ToList();
         parameters.AddRange(unchangedParametersOfAppliedSet(simulation, templateCompound.Name, changedPaths, parameterCache));

         return new CompoundCommitDTO
         {
            CompoundName = templateCompound.Name,
            Compound = templateCompound,
            AvailableExistingSets = templateCompound.OverwriteParameterSets,
            CreateNew = selectedSetInTemplate == null,
            SelectedExistingSet = selectedSetInTemplate,
            SetSelectedInSimulation = selectedSetInTemplate,
            NewSetName = templateCompound.Name,
            Parameters = parameters
         };
      }

      private ParameterCommitDTO mapParameter(Simulation simulation, string compoundName, string path, IParameter parameter)
      {
         var isRemoval = simulation.IsOverwrittenParameterPath(compoundName, path) && parameter != null && !parameter.ValueDiffersFromDefault();
         return _parameterCommitDTOMapper.MapFrom(path, parameter, isRemoval, isUnchanged: false);
      }

      /// <summary>
      ///    Returns the entries of the set applied to the compound in the simulation that the user did not change. They are
      ///    carried over into a new set created from the simulation and are listed so that the dialog shows everything the
      ///    new set will contain. Their value is taken from the simulation, which is the value of the set since the path is
      ///    not changed.
      /// </summary>
      private IEnumerable<ParameterCommitDTO> unchangedParametersOfAppliedSet(Simulation simulation, string compoundName,
         IReadOnlyList<string> changedPaths, PathCache<IParameter> parameterCache)
      {
         var appliedSet = simulation.OverwriteParameterSetSelections.SelectedSetFor(compoundName);
         if (appliedSet == null)
            return Enumerable.Empty<ParameterCommitDTO>();

         return appliedSet.ParameterValues
            .Select(x => x.Path.PathAsString)
            .Where(path => !changedPaths.Contains(path))
            .Select(path => _parameterCommitDTOMapper.MapFrom(path, parameterCache[path], isRemoval: false, isUnchanged: true));
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
