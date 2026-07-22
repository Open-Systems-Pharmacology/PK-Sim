using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IOverwriteParameterSetApplicationTask
   {
      /// <summary>
      ///    Applies the values of the <see cref="OverwriteParameterSet" /> selected for each compound in the
      ///    <paramref name="simulation" /> to the matching simulation parameters (matched by path). Affected parameters
      ///    become <see cref="PKSimBuildingBlockType.Compound" /> so that subsequent changes follow normal compound
      ///    parameter logic, and their value origin is set to identify the originating overwrite parameter set.
      ///    Overwritten parameters are also flagged <see cref="IParameter.CanBeVariedInPopulation" />=false so that a
      ///    population simulation keeps the overwritten value fixed; any advanced parameter already defined for such a
      ///    path in a <see cref="PopulationSimulation" /> is removed so it cannot override the value at run time.
      ///    Compounds whose selection is "None" are skipped.
      ///    Throws a <see cref="CannotApplyOverwriteParameterSetException" /> if any path cannot be resolved in the
      ///    simulation, in which case no value is applied.
      /// </summary>
      void ApplyOverwriteParameterSetsTo(Simulation simulation);

      /// <summary>
      ///    Adds the values of the <see cref="OverwriteParameterSet" /> selected for each compound in the
      ///    <paramref name="simulation" /> into <paramref name="parameterValues" /> so that they are carried over when the
      ///    simulation is exported to MoBi. The matching simulation parameter provides the entry metadata (dimension, unit)
      ///    and the overwrite set provides the value. An existing entry for a path is updated; otherwise a new one is created.
      ///    Compounds whose selection is "None" are skipped.
      /// </summary>
      void ApplyOverwriteParameterSetsTo(ParameterValuesBuildingBlock parameterValues, Simulation simulation);
   }

   public class OverwriteParameterSetApplicationTask : IOverwriteParameterSetApplicationTask
   {
      private readonly IContainerTask _containerTask;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterValuesCreator _parameterValuesCreator;

      public OverwriteParameterSetApplicationTask(IContainerTask containerTask, IEntityPathResolver entityPathResolver, IParameterValuesCreator parameterValuesCreator)
      {
         _containerTask = containerTask;
         _entityPathResolver = entityPathResolver;
         _parameterValuesCreator = parameterValuesCreator;
      }

      public void ApplyOverwriteParameterSetsTo(Simulation simulation)
      {
         var resolvedValues = resolveOverwriteValues(simulation);
         applyValues(resolvedValues);
         removeAdvancedParametersFor(simulation, resolvedValues);
      }

      private static void applyValues(IReadOnlyList<(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)> resolvedValues) =>
         resolvedValues.Each(x => applyValue(x.parameter, x.parameterValue, x.compound, x.overwriteParameterSet));

      private void removeAdvancedParametersFor(Simulation simulation,
         IReadOnlyList<(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)> resolvedValues)
      {
         if (simulation is not PopulationSimulation populationSimulation)
            return;

         resolvedValues.Each(x =>
         {
            var advancedParameter = populationSimulation.AdvancedParameterFor(_entityPathResolver, x.parameter);
            if (advancedParameter != null)
               populationSimulation.RemoveAdvancedParameter(advancedParameter);
         });
      }

      public void ApplyOverwriteParameterSetsTo(ParameterValuesBuildingBlock parameterValues, Simulation simulation) => resolveOverwriteValues(simulation).Each(x => addToParameterValues(x.parameter, x.parameterValue, parameterValues));

      private IReadOnlyList<(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)> resolveOverwriteValues(Simulation simulation)
      {
         var resolvedValues = new List<(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)>();

         var selections = simulation.OverwriteParameterSetSelections.Selections
            .Where(x => x.OverwriteParameterSet != null)
            .ToList();

         if (!selections.Any())
            return resolvedValues;

         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);
         var unresolvedPaths = new List<string>();

         selections.Each(selection => resolveSelection(selection, simulation, parameterCache, resolvedValues, unresolvedPaths));

         if (unresolvedPaths.Any())
            throw new CannotApplyOverwriteParameterSetException(unresolvedPaths);

         return resolvedValues;
      }

      private void resolveSelection(OverwriteParameterSetSelection selection, Simulation simulation, PathCache<IParameter> parameterCache,
         List<(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)> resolvedValues, List<string> unresolvedPaths)
      {
         var compound = simulation.Compounds.FindByName(selection.CompoundName);

         selection.OverwriteParameterSet.ParameterValues.Each(parameterValue =>
            resolveParameterValue(parameterValue, compound, selection.OverwriteParameterSet, parameterCache, resolvedValues, unresolvedPaths));
      }

      private static void resolveParameterValue(ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet, PathCache<IParameter> parameterCache,
         List<(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)> resolvedValues, List<string> unresolvedPaths)
      {
         var path = parameterValue.Path.PathAsString;
         var parameter = parameterCache[path];
         if (parameter == null)
            unresolvedPaths.Add(path);
         else
            resolvedValues.Add((parameter, parameterValue, compound, overwriteParameterSet));
      }

      private static void applyValue(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)
      {
         parameter.Value = parameterValue.Value.Value;
         parameter.BuildingBlockType = PKSimBuildingBlockType.Compound;
         parameter.Origin.BuilingBlockId = compound.Id;
         parameter.ValueOrigin.Source = ValueOriginSources.Other;
         parameter.ValueOrigin.Description = $"{PKSimConstants.ObjectTypes.OverwriteParameterSet} '{overwriteParameterSet.Name}'";
         parameter.CanBeVariedInPopulation = false;
      }

      private void addToParameterValues(IParameter parameter, ParameterValue overwriteValue, ParameterValuesBuildingBlock buildingBlock)
      {
         var objectPath = _entityPathResolver.ObjectPathFor(parameter);
         var parameterValue = buildingBlock[objectPath];
         if (parameterValue == null)
         {
            parameterValue = _parameterValuesCreator.CreateParameterValue(objectPath, parameter);
            buildingBlock.Add(parameterValue);
         }

         parameterValue.Value = overwriteValue.Value;
         parameterValue.Formula = null;
      }
   }
}