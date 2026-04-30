using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services;

public interface IOverwriteParameterSetTask
{
   /// <summary>
   ///    Updates the value of a parameter (identified by path) in an <see cref="OverwriteParameterSet" />.
   ///    Returns an empty command if the parameter is not in the set or the value is unchanged.
   /// </summary>
   ICommand UpdateParameterValue(OverwriteParameterSet overwriteParameterSet, Compound compound, string parameterPath, double newValue);

   /// <summary>
   ///    Removes a parameter (identified by path) from an <see cref="OverwriteParameterSet" />.
   ///    Returns an empty command if the parameter is not in the set.
   /// </summary>
   ICommand RemoveParameterValue(OverwriteParameterSet overwriteParameterSet, Compound compound, string parameterPath);

   ICommand SetIsDefault(OverwriteParameterSet overwriteParameterSet, Compound compound, bool isDefault);

   ICommand RemoveSet(OverwriteParameterSet overwriteParameterSet, Compound compound);
}

public class OverwriteParameterSetTask : IOverwriteParameterSetTask
{
   private readonly IExecutionContext _executionContext;
   private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
   private readonly ILazyLoadTask _lazyLoadTask;

   public OverwriteParameterSetTask(
      IExecutionContext executionContext,
      IBuildingBlockInProjectManager buildingBlockInProjectManager,
      ILazyLoadTask lazyLoadTask)
   {
      _executionContext = executionContext;
      _buildingBlockInProjectManager = buildingBlockInProjectManager;
      _lazyLoadTask = lazyLoadTask;
   }

   public ICommand UpdateParameterValue(OverwriteParameterSet overwriteParameterSet, Compound compound, string parameterPath, double newValue)
   {
      var existing = overwriteParameterSet.ParameterValueByPath(parameterPath);
      if (existing == null || ValueComparer.AreValuesEqual(existing.Value.GetValueOrDefault(), newValue))
         return new PKSimEmptyCommand();

      return new UpdateParameterValueInOverwriteSetCommand(overwriteParameterSet, compound, parameterPath, newValue).Run(_executionContext);
   }

   public ICommand RemoveParameterValue(OverwriteParameterSet overwriteParameterSet, Compound compound, string parameterPath)
   {
      if (overwriteParameterSet.ParameterValueByPath(parameterPath) == null)
         return new PKSimEmptyCommand();

      return new RemoveParameterValueFromOverwriteSetCommand(overwriteParameterSet, compound, parameterPath).Run(_executionContext);
   }

   public ICommand SetIsDefault(OverwriteParameterSet overwriteParameterSet, Compound compound, bool isDefault)
   {
      if (overwriteParameterSet.IsDefault == isDefault)
         return new PKSimEmptyCommand();

      if (isDefault)
         return new SetDefaultOverwriteParameterSetCommand(overwriteParameterSet, compound).Run(_executionContext);

      return new ClearDefaultOverwriteParameterSetCommand(overwriteParameterSet, compound).Run(_executionContext);
   }

   public ICommand RemoveSet(OverwriteParameterSet overwriteParameterSet, Compound compound)
   {
      var blockingSimulations = simulationsUsing(overwriteParameterSet, compound);
      if (blockingSimulations.Any())
         throw new CannotDeleteOverwriteParameterSetException(overwriteParameterSet.Name, compound.Name, blockingSimulations.Select(x => x.Name).ToList());

      return new RemoveOverwriteParameterSetFromCompoundCommand(overwriteParameterSet, compound).Run(_executionContext);
   }
   
   private IReadOnlyList<Simulation> simulationsUsing(OverwriteParameterSet overwriteParameterSet, Compound compound)
   {
      var buildingBlocksUsingCompound = _buildingBlockInProjectManager.SimulationsUsing(compound);
      buildingBlocksUsingCompound.Each(_lazyLoadTask.Load);
      return buildingBlocksUsingCompound
         .Where(simulation => simulation.OverwriteParameterSetSelections.Selections.Any(x =>
            string.Equals(x.CompoundName, compound.Name) &&
            string.Equals(x.OverwriteParameterSet?.Name, overwriteParameterSet.Name)))
         .ToList();
   }
}