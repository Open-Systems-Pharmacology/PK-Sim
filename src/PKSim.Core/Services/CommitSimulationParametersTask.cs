using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using IBuildingBlockRepository = PKSim.Core.Repositories.IBuildingBlockRepository;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Describes what should be committed for a single compound.
   /// </summary>
   public class CompoundCommitInfo
   {
      /// <summary>
      ///    The template Id of the compound in the project.
      /// </summary>
      public string TemplateCompoundId { get; init; }

      /// <summary>
      ///    Parameter paths to commit.
      /// </summary>
      public IReadOnlyList<string> ParameterPaths { get; init; }

      /// <summary>
      ///    Name of the OverwriteParameterSet. When <see cref="ShouldCreateNew" /> is true, this is the name for
      ///    the new set. Otherwise, it identifies the existing set to update (by name) in both compounds.
      /// </summary>
      public string OverwriteParameterSetName { get; init; }

      /// <summary>
      ///    Whether to create a new OverwriteParameterSet or update an existing one.
      /// </summary>
      public bool ShouldCreateNew { get; init; }
   }

   public interface ICommitSimulationParametersTask
   {
      /// <summary>
      ///    Creates and executes a command that commits the specified parameter changes to the compound
      ///    and clears the committed paths from the tracker.
      /// </summary>
      ICommand CommitParametersToCompound(Simulation simulation, CompoundCommitInfo commitInfo);
   }

   public class CommitSimulationParametersTask : ICommitSimulationParametersTask
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainerTask _containerTask;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public CommitSimulationParametersTask(IExecutionContext executionContext, IContainerTask containerTask, IBuildingBlockRepository buildingBlockRepository)
      {
         _executionContext = executionContext;
         _containerTask = containerTask;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public ICommand CommitParametersToCompound(Simulation simulation, CompoundCommitInfo commitInfo)
      {
         var simulationCompound = simulation.BuildingBlockByTemplateId<Compound>(commitInfo.TemplateCompoundId);
         var templateCompound = _buildingBlockRepository.ById<Compound>(commitInfo.TemplateCompoundId);

         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);
         var parameterValues = createParameterValuesFor(commitInfo, parameterCache);

         var command = commitInfo.ShouldCreateNew
            ? createNewSetsCommand(simulationCompound, templateCompound, commitInfo.OverwriteParameterSetName, parameterValues)
            : updateExistingSetsCommand(simulationCompound, templateCompound, commitInfo.OverwriteParameterSetName, parameterValues, simulation, commitInfo.ParameterPaths, parameterCache);

         command.Run(_executionContext);

         command.All().Each(subCommand => _executionContext.UpdateBuildingBlockPropertiesInCommand(subCommand, simulationCompound));
         _executionContext.UpdateBuildingBlockPropertiesInCommand(command, simulationCompound);

         //Only untrack paths that were actually resolved to parameter values
         parameterValues.Each(pv => simulation.ParameterChangeTracker.Untrack(pv.Path));

         _executionContext.PublishEvent(new SimulationStatusChangedEvent(simulation));
         
         return command;
      }

      private PKSimMacroCommand createNewSetsCommand(Compound simulationCompound, Compound templateCompound, string setName, List<ParameterValue> parameterValues)
      {
         var command = new PKSimMacroCommand();

         command.Add(addOverwriteParameterSetCommand(simulationCompound, setName, parameterValues));
         command.Add(addOverwriteParameterSetCommand(templateCompound, setName, parameterValues));

         return command;
      }

      private ICommand addOverwriteParameterSetCommand(Compound compound, string setName, List<ParameterValue> parameterValues)
      {
         var newSet = new OverwriteParameterSet { Name = setName };
         parameterValues.Each(newSet.Add);
         return new AddOverwriteParameterSetToCompoundCommand(newSet, compound);
      }

      /// <summary>
      ///    Creates a macro command that updates existing OverwriteParameterSets (identified by <paramref name="setName" />)
      ///    in both the simulation and template compounds. Parameters that were previously in the set but have been
      ///    reset by the user (no longer differ from their original/default value) are removed from the set. Entries
      ///    the user has not touched since the previous commit are preserved.
      /// </summary>
      private PKSimMacroCommand updateExistingSetsCommand(Compound simulationCompound, Compound templateCompound, string setName,
         List<ParameterValue> parameterValues, Simulation simulation, IReadOnlyList<string> parameterPaths, PathCache<IParameter> parameterCache)
      {
         var command = new PKSimMacroCommand();

         var existingSimulationSet = simulationCompound.OverwriteParameterSets.FindByName(setName);
         var existingTemplateSet = templateCompound.OverwriteParameterSets.FindByName(setName);
         var pathsToRemove = pathsResetByUser(existingSimulationSet, parameterPaths, simulation, parameterCache);

         command.Add(new UpdateOverwriteParameterSetCommand(existingSimulationSet, simulationCompound, parameterValues, pathsToRemove));
         command.Add(new UpdateOverwriteParameterSetCommand(existingTemplateSet, templateCompound, parameterValues, pathsToRemove));

         return command;
      }

      private List<ParameterValue> createParameterValuesFor(CompoundCommitInfo info, PathCache<IParameter> parameterCache)
      {
         return info.ParameterPaths
            .Select(path =>
            {
               var parameter = parameterCache[path];
               if (parameter == null)
                  return null;

               return new ParameterValue
               {
                  Path = path.ToObjectPath(),
                  Value = parameter.Value
               };
            })
            .Where(pv => pv != null)
            .ToList();
      }

      /// <summary>
      ///    When updating an existing set, find entries the user has reset to the parameter's original value.
      ///    An entry is considered reset when the user is not committing the path, the path is no longer tracked
      ///    as changed, and the simulation parameter's current value no longer matches the value stored in the
      ///    set. Entries the user has not touched (parameter value still matches the set's stored value) are
      ///    preserved so that they are not stripped from the set on a subsequent update commit.
      /// </summary>
      private IReadOnlyList<string> pathsResetByUser(OverwriteParameterSet existingSet, IReadOnlyList<string> parameterPaths,
         Simulation simulation, PathCache<IParameter> parameterCache)
      {
         var committedPaths = new HashSet<string>(parameterPaths);

         return existingSet.ParameterValues
            .Where(pv =>
            {
               var path = pv.Path.PathAsString;
               if (committedPaths.Contains(path))
                  return false;
               if (simulation.ParameterChangeTracker.IsTracked(path))
                  return false;
               var parameter = parameterCache[path];
               if (parameter == null)
                  return false;
               return !ValueComparer.AreValuesEqual(parameter.Value, pv.Value.GetValueOrDefault());
            })
            .Select(pv => pv.Path.PathAsString)
            .ToList();
      }
   }
}