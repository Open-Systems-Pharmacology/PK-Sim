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
      ///    Creates and executes a macro command that commits the specified parameter changes to compounds
      ///    and clears the committed paths from the tracker.
      /// </summary>
      IMacroCommand CommitParametersToCompounds(Simulation simulation, IReadOnlyList<CompoundCommitInfo> commitInfos);
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

      public IMacroCommand CommitParametersToCompounds(Simulation simulation, IReadOnlyList<CompoundCommitInfo> commitInfos)
      {
         var macroCommand = new PKSimMacroCommand
         {
            CommandType = PKSimConstants.Command.CommandTypeEdit,
            ObjectType = PKSimConstants.ObjectTypes.Simulation,
            Description = PKSimConstants.Command.CommitSimulationParametersDescription
         };

         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);

         commitInfos.Each(info =>
         {
            var parameterValues = createParameterValuesFor(info, parameterCache);

            if (info.ShouldCreateNew)
               addCreateNewSetCommand(macroCommand, info, parameterValues);
            else
               addUpdateExistingSetCommand(macroCommand, info, parameterValues, simulation);
         });

         macroCommand.Run(_executionContext);

         var firstCompound = commitInfos.FirstOrDefault()?.Compound;
         if (firstCompound != null)
            _executionContext.UpdateBuildingBlockPropertiesInCommand(macroCommand, firstCompound);

         //Clear committed paths from tracker
         commitInfos.Each(info => info.ParameterPaths.Each(path => simulation.ParameterChangeTracker.Untrack(path)));

         return macroCommand;
      }

      private void addCreateNewSetCommand(PKSimMacroCommand macroCommand, CompoundCommitInfo info, List<ParameterValue> parameterValues)
      {
         var newSet = new OverwriteParameterSet {Name = info.NewOverwriteParameterSetName};
         parameterValues.Each(pv => newSet.Add(pv));
         macroCommand.Add(new AddOverwriteParameterSetToCompoundCommand(newSet, info.Compound));
      }

      private void addUpdateExistingSetCommand(PKSimMacroCommand macroCommand, CompoundCommitInfo info, List<ParameterValue> parameterValues, Simulation simulation)
      {
         var pathsToRemove = pathsResetByUser(info, simulation);
         macroCommand.Add(new UpdateOverwriteParameterSetCommand(info.ExistingOverwriteParameterSet, info.Compound, parameterValues, pathsToRemove));
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