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
      private readonly IObjectBaseFactory _objectBaseFactory;

      public CommitSimulationParametersTask(IExecutionContext executionContext, IContainerTask containerTask, IBuildingBlockRepository buildingBlockRepository, IObjectBaseFactory objectBaseFactory)
      {
         _executionContext = executionContext;
         _containerTask = containerTask;
         _buildingBlockRepository = buildingBlockRepository;
         _objectBaseFactory = objectBaseFactory;
      }

      public ICommand CommitParametersToCompound(Simulation simulation, CompoundCommitInfo commitInfo)
      {
         var templateCompound = _buildingBlockRepository.ById<Compound>(commitInfo.TemplateCompoundId);

         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);
         var parameterValues = createParameterValuesFor(commitInfo, parameterCache);

         var command = commitInfo.ShouldCreateNew
            ? createNewSetCommand(templateCompound, commitInfo.OverwriteParameterSetName, parameterValues)
            : updateExistingSetCommand(templateCompound, commitInfo.OverwriteParameterSetName, parameterValues, simulation, commitInfo.ParameterPaths, parameterCache);

         //Only untrack paths that were actually resolved to parameter values
         command.Add(new SetSimulationParameterTrackingCommand(simulation, parameterValues.Select(pv => pv.Path.PathAsString).ToList(), tracked: false));

         command.Run(_executionContext);

         _executionContext.UpdateBuildingBlockPropertiesInCommand(command, templateCompound);

         return command;
      }

      private PKSimMacroCommand createNewSetCommand(Compound templateCompound, string setName, List<ParameterValue> parameterValues)
      {
         var command = new PKSimMacroCommand
         {
            ObjectType = PKSimConstants.ObjectTypes.OverwriteParameterSet,
            CommandType = PKSimConstants.Command.CommandTypeAdd,
            Description = PKSimConstants.Command.CommitSimulationParametersToCompound(setName, templateCompound.Name)
         };

         var newSet = _objectBaseFactory.Create<OverwriteParameterSet>().WithName(setName);
         parameterValues.Each(newSet.Add);
         command.Add(new AddOverwriteParameterSetToCompoundCommand(newSet, templateCompound));

         return command;
      }

      /// <summary>
      ///    Creates a macro command that updates the existing OverwriteParameterSet (identified by
      ///    <paramref name="setName" />) in the template compound. Parameters that were previously in the set but have
      ///    been reset by the user (no longer differ from their original/default value) are removed from the set. Entries
      ///    the user has not touched since the previous commit are preserved.
      /// </summary>
      /// <param name="templateCompound">The project template compound whose OverwriteParameterSet will be updated.</param>
      /// <param name="setName">Name of the existing OverwriteParameterSet to update.</param>
      /// <param name="parameterValues">The new parameter values to apply to the set.</param>
      /// <param name="simulation">The simulation, used to check which paths are still tracked.</param>
      /// <param name="parameterPaths">The parameter paths being committed, used to determine which paths the user has reset.</param>
      /// <param name="parameterCache">Cache of simulation parameters, used to compare current values against the set's stored values when detecting resets.</param>
      /// <returns>A macro command containing the update command for the template compound.</returns>
      private PKSimMacroCommand updateExistingSetCommand(Compound templateCompound, string setName,
         List<ParameterValue> parameterValues, Simulation simulation, IReadOnlyList<string> parameterPaths, PathCache<IParameter> parameterCache)
      {
         var command = new PKSimMacroCommand
         {
            ObjectType = PKSimConstants.ObjectTypes.OverwriteParameterSet,
            CommandType = PKSimConstants.Command.CommandTypeUpdate,
            Description = PKSimConstants.Command.CommitSimulationParametersToCompound(setName, templateCompound.Name)
         };

         var existingTemplateSet = templateCompound.OverwriteParameterSets.FindByName(setName);
         var pathsToRemove = pathsResetByUser(existingTemplateSet, parameterPaths, simulation, parameterCache);

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
               //user is committing this path: it will be re-added with a new value, not reset
               if (committedPaths.Contains(path))
                  return false;
               //path is still tracked as changed: user has uncommitted changes for it, not reset
               if (simulation.ParameterChangeTracker.IsTracked(path))
                  return false;
               var parameter = parameterCache[path];
               //parameter is no longer present in the simulation: preserve the stored entry
               if (parameter == null)
                  return false;
               //parameter's current value no longer matches the value stored in the set: user has reset it
               return !ValueComparer.AreValuesEqual(parameter.Value, pv.Value.GetValueOrDefault());
            })
            .Select(pv => pv.Path.PathAsString)
            .ToList();
      }
   }
}