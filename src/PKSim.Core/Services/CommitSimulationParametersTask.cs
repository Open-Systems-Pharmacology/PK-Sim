using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
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
      ///    The template compound to commit to.
      /// </summary>
      public Compound Compound { get; init; }

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

         var command = commitInfo.ShouldCreateNew
            ? createNewSetCommand(commitInfo, parameterValues)
            : updateExistingSetCommand(commitInfo, parameterValues, simulation);

         command.Run(_executionContext);
         _executionContext.UpdateBuildingBlockPropertiesInCommand(command, commitInfo.Compound);

         //Only untrack paths that were actually resolved to parameter values
         parameterValues.Each(pv => simulation.ParameterChangeTracker.Untrack(pv.Path.PathAsString));

         return command;
      }

      private IPKSimReversibleCommand createNewSetCommand(CompoundCommitInfo info, List<ParameterValue> parameterValues)
      {
         var newSet = new OverwriteParameterSet { Name = info.NewOverwriteParameterSetName };
         parameterValues.Each(pv => newSet.Add(pv));
         return new AddOverwriteParameterSetToCompoundCommand(newSet, info.Compound);
      }

      private IPKSimReversibleCommand updateExistingSetCommand(CompoundCommitInfo info, List<ParameterValue> parameterValues, Simulation simulation)
      {
         var pathsToRemove = pathsResetByUser(info, simulation);
         return new UpdateOverwriteParameterSetCommand(info.ExistingOverwriteParameterSet, info.Compound, parameterValues, pathsToRemove);
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
