using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Describes what should be committed for a single compound.
   /// </summary>
   public class CompoundCommitInfo
   {
      /// <summary>
      ///    The simulation compound to commit to.
      /// </summary>
      public Compound SimulationCompound { get; init; }

      /// <summary>
      ///    The project compound to commit to
      /// </summary>
      public Compound TemplateCompound { get; init; }

      /// <summary>
      ///    Parameter paths to commit.
      /// </summary>
      public IReadOnlyList<string> ParameterPaths { get; init; }

      /// <summary>
      ///    If set, update this existing OverwriteParameterSet instead of creating a new one.
      /// </summary>
      public OverwriteParameterSet ExistingOverwriteParameterSet { get; init; }

      /// <summary>
      ///    Name for the new OverwriteParameterSet. Only used when <see cref="ExistingOverwriteParameterSet" /> is null.
      /// </summary>
      public string NewOverwriteParameterSetName { get; init; }

      public bool ShouldCreateNew => ExistingOverwriteParameterSet == null;
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

      public CommitSimulationParametersTask(IExecutionContext executionContext, IContainerTask containerTask)
      {
         _executionContext = executionContext;
         _containerTask = containerTask;
      }

      public ICommand CommitParametersToCompound(Simulation simulation, CompoundCommitInfo commitInfo)
      {
         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);
         var parameterValues = createParameterValuesFor(commitInfo, parameterCache);

         var command = createAndRunCommitCommands(commitInfo, parameterValues, simulation);

         //Only untrack paths that were actually resolved to parameter values
         parameterValues.Each(pv => simulation.ParameterChangeTracker.Untrack(pv.Path.PathAsString));

         return command;
      }

      private PKSimMacroCommand createAndRunCommitCommands(CompoundCommitInfo commitInfo, List<ParameterValue> parameterValues, Simulation simulation)
      {
         var command = commitInfo.ShouldCreateNew
            ? createNewSetCommand(commitInfo, parameterValues)
            : updateExistingSetCommand(commitInfo, parameterValues, simulation);

         command.Run(_executionContext);

         command.All().Each(subCommand => _executionContext.UpdateBuildingBlockPropertiesInCommand(subCommand, commitInfo.SimulationCompound));
         _executionContext.UpdateBuildingBlockPropertiesInCommand(command, commitInfo.SimulationCompound);

         return command;
      }

      private PKSimMacroCommand createNewSetCommand(CompoundCommitInfo commitInfo, List<ParameterValue> parameterValues)
      {
         var command = new PKSimMacroCommand();

         command.Add(addOverwriteParameterSetCommand(commitInfo.SimulationCompound, commitInfo.NewOverwriteParameterSetName, parameterValues));
         command.Add(addOverwriteParameterSetCommand(commitInfo.TemplateCompound, commitInfo.NewOverwriteParameterSetName, parameterValues));

         return command;
      }

      private ICommand addOverwriteParameterSetCommand(Compound compound, string setName, List<ParameterValue> parameterValues)
      {
         var newSet = new OverwriteParameterSet { Name = setName };
         parameterValues.Each(newSet.Add);
         return new AddOverwriteParameterSetToCompoundCommand(newSet, compound);
      }

      private PKSimMacroCommand updateExistingSetCommand(CompoundCommitInfo commitInfo, List<ParameterValue> parameterValues, Simulation simulation)
      {
         var command = new PKSimMacroCommand();
         var pathsToRemove = pathsResetByUser(commitInfo, simulation);

         command.Add(new UpdateOverwriteParameterSetCommand(commitInfo.ExistingOverwriteParameterSet, commitInfo.SimulationCompound, parameterValues, pathsToRemove));
         command.Add(new UpdateOverwriteParameterSetCommand(commitInfo.ExistingOverwriteParameterSet, commitInfo.TemplateCompound, parameterValues, pathsToRemove));

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
      private IReadOnlyList<string> pathsResetByUser(CompoundCommitInfo info, Simulation simulation)
      {
         var committedPaths = new HashSet<string>(info.ParameterPaths);

         return info.ExistingOverwriteParameterSet.ParameterValues
            .Select(pv => pv.Path.PathAsString)
            .Where(path => !committedPaths.Contains(path) && !simulation.ParameterChangeTracker.IsTracked(path))
            .ToList();
      }
   }
}