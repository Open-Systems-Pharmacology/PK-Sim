using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Services;
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
}

public class OverwriteParameterSetTask : IOverwriteParameterSetTask
{
   private readonly IExecutionContext _executionContext;

   public OverwriteParameterSetTask(IExecutionContext executionContext)
   {
      _executionContext = executionContext;
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
}