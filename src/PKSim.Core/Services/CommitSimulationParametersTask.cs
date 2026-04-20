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
            : updateExistingSetsCommand(simulationCompound, templateCompound, commitInfo.OverwriteParameterSetName, parameterValues, simulation, commitInfo.ParameterPaths);

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
      ///    in both the simulation and template compounds. Parameters that were previously in the set but are no longer
      ///    tracked (i.e., reset by the user) are removed from the set.
      /// </summary>
      /// <param name="simulationCompound">The compound used in the simulation whose OverwriteParameterSet will be updated.</param>
      /// <param name="templateCompound">The project template compound whose corresponding OverwriteParameterSet will be updated.</param>
      /// <param name="setName">Name of the existing OverwriteParameterSet to update in both compounds.</param>
      /// <param name="parameterValues">The new parameter values to apply to the set.</param>
      /// <param name="simulation">The simulation, used to check which paths are still tracked.</param>
      /// <param name="parameterPaths">The parameter paths being committed, used to determine which paths the user has reset.</param>
      /// <returns>A macro command containing the update commands for both compounds.</returns>
      private PKSimMacroCommand updateExistingSetsCommand(Compound simulationCompound, Compound templateCompound, string setName,
         List<ParameterValue> parameterValues, Simulation simulation, IReadOnlyList<string> parameterPaths)
      {
         var command = new PKSimMacroCommand();

         var existingSimulationSet = simulationCompound.OverwriteParameterSets.FindByName(setName);
         var existingTemplateSet = templateCompound.OverwriteParameterSets.FindByName(setName);
         var pathsToRemove = pathsResetByUser(existingSimulationSet, parameterPaths, simulation);

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
      ///    When updating an existing set, find paths that were in the set but are no longer tracked
      ///    (i.e., the user reset those parameters). These should be removed from the set.
      /// </summary>
      private IReadOnlyList<string> pathsResetByUser(OverwriteParameterSet existingSet, IReadOnlyList<string> parameterPaths, Simulation simulation)
      {
         var committedPaths = new HashSet<string>(parameterPaths);

         return existingSet.ParameterValues
            .Select(pv => pv.Path.PathAsString)
            .Where(path => !committedPaths.Contains(path) && !simulation.ParameterChangeTracker.IsTracked(path))
            .ToList();
      }
   }
}