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
      ///    Compounds whose selection is "None" are skipped.
      ///    Throws a <see cref="CannotApplyOverwriteParameterSetException" /> if any path cannot be resolved in the
      ///    simulation, in which case no value is applied.
      /// </summary>
      void ApplyOverwriteParameterSetsTo(Simulation simulation);
   }

   public class OverwriteParameterSetApplicationTask : IOverwriteParameterSetApplicationTask
   {
      private readonly IContainerTask _containerTask;

      public OverwriteParameterSetApplicationTask(IContainerTask containerTask)
      {
         _containerTask = containerTask;
      }

      public void ApplyOverwriteParameterSetsTo(Simulation simulation)
      {
         var selections = simulation.OverwriteParameterSetSelections.Selections
            .Where(x => x.OverwriteParameterSet != null)
            .ToList();

         if (!selections.Any())
            return;

         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);
         var unresolvedPaths = new List<string>();
         var resolvedValues = new List<(IParameter parameter, ParameterValue parameterValue, Compound compound, OverwriteParameterSet overwriteParameterSet)>();

         selections.Each(selection => resolveSelection(selection, simulation, parameterCache, resolvedValues, unresolvedPaths));

         if (unresolvedPaths.Any())
            throw new CannotApplyOverwriteParameterSetException(unresolvedPaths);

         resolvedValues.Each(x => applyValue(x.parameter, x.parameterValue, x.compound, x.overwriteParameterSet));
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
      }
   }
}
