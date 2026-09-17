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
      ///    Parameter paths whose current simulation values are committed to the set.
      /// </summary>
      public IReadOnlyList<string> ParameterPaths { get; init; }

      /// <summary>
      ///    Parameter paths that the user reset in the simulation and that are removed from the existing set.
      ///    Only meaningful when <see cref="ShouldCreateNew" /> is false.
      /// </summary>
      public IReadOnlyList<string> ParameterPathsToRemove { get; init; } = new List<string>();

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
      private readonly ICloner _cloner;

      public CommitSimulationParametersTask(IExecutionContext executionContext, IContainerTask containerTask, IBuildingBlockRepository buildingBlockRepository, IObjectBaseFactory objectBaseFactory, ICloner cloner)
      {
         _executionContext = executionContext;
         _containerTask = containerTask;
         _buildingBlockRepository = buildingBlockRepository;
         _objectBaseFactory = objectBaseFactory;
         _cloner = cloner;
      }

      public ICommand CommitParametersToCompound(Simulation simulation, CompoundCommitInfo commitInfo)
      {
         var templateCompound = _buildingBlockRepository.ById<Compound>(commitInfo.TemplateCompoundId);

         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);
         var parameterValues = createParameterValuesFor(commitInfo, parameterCache);

         var command = commitInfo.ShouldCreateNew
            ? createNewSetCommand(templateCompound, commitInfo.OverwriteParameterSetName, unionWithSetAppliedTo(simulation, templateCompound.Name, parameterValues, commitInfo.ParameterPathsToRemove))
            : updateExistingSetCommand(templateCompound, commitInfo.OverwriteParameterSetName, parameterValues, commitInfo.ParameterPathsToRemove);

         //Only untrack paths that were actually resolved to parameter values
         var committedPaths = parameterValues.Select(pv => pv.Path.PathAsString).Concat(commitInfo.ParameterPathsToRemove).ToList();
         command.Add(new SetSimulationParameterTrackingCommand(simulation, committedPaths, tracked: false));

         command.Run(_executionContext);

         _executionContext.UpdateBuildingBlockPropertiesInCommand(command, templateCompound);

         return command;
      }

      /// <summary>
      ///    Returns <paramref name="committedValues" /> together with the entries of the set applied to the compound in
      ///    <paramref name="simulation" /> that the user neither committed nor removed, so that a new set created from a
      ///    simulation using a set also contains the values of that set. The entries are taken from the applied set and not
      ///    from the simulation so that a change the user did not commit keeps the value of the set.
      /// </summary>
      private List<ParameterValue> unionWithSetAppliedTo(Simulation simulation, string compoundName, List<ParameterValue> committedValues, IReadOnlyList<string> pathsToRemove)
      {
         var appliedSet = simulation.OverwriteParameterSetSelections.SelectedSetFor(compoundName);
         if (appliedSet == null)
            return committedValues;

         var pathsAlreadyHandled = committedValues.Select(x => x.Path.PathAsString).Concat(pathsToRemove).ToHashSet();

         var inheritedValues = appliedSet.ParameterValues
            .Where(x => !pathsAlreadyHandled.Contains(x.Path.PathAsString))
            .Select(_cloner.Clone);

         return committedValues.Concat(inheritedValues).ToList();
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
      ///    <paramref name="setName" />) in the template compound. Entries the user has not touched since the previous
      ///    commit are preserved.
      /// </summary>
      /// <param name="templateCompound">The project template compound whose OverwriteParameterSet will be updated.</param>
      /// <param name="setName">Name of the existing OverwriteParameterSet to update.</param>
      /// <param name="parameterValues">The new parameter values to apply to the set.</param>
      /// <param name="pathsToRemove">The parameter paths reset by the user that are removed from the set.</param>
      /// <returns>A macro command containing the update command for the template compound.</returns>
      private PKSimMacroCommand updateExistingSetCommand(Compound templateCompound, string setName,
         List<ParameterValue> parameterValues, IReadOnlyList<string> pathsToRemove)
      {
         var command = new PKSimMacroCommand
         {
            ObjectType = PKSimConstants.ObjectTypes.OverwriteParameterSet,
            CommandType = PKSimConstants.Command.CommandTypeUpdate,
            Description = PKSimConstants.Command.CommitSimulationParametersToCompound(setName, templateCompound.Name)
         };

         var existingTemplateSet = templateCompound.OverwriteParameterSets.FindByName(setName);
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

               var parameterValue = new ParameterValue
               {
                  Path = path.ToObjectPath(),
                  Value = parameter.Value,
                  Dimension = parameter.Dimension,
                  DisplayUnit = parameter.DisplayUnit,
                  Info = parameter.Info.Clone()
               };

               parameterValue.ValueOrigin.UpdateAllFrom(parameter.ValueOrigin);
               return parameterValue;
            })
            .Where(pv => pv != null)
            .ToList();
      }
   }
}