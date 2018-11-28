using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;

namespace PKSim.Core.Services
{
   public interface IParameterTask : ISetParameterTask
   {
      /// <summary>
      ///    Sets the value in the parameter. Value will be converted in kernel unit
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="valueToSetInGuiUnit">Value in display unit</param>
      ICommand SetParameterDisplayValue(IParameter parameter, double valueToSetInGuiUnit);

      /// <summary>
      ///    Sets the value in the parameter. Value will be converted in kernel unit
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="enumValue">Enumeration value that will be converted to a double</param>
      ICommand SetParameterDisplayValue(IParameter parameter, Enum enumValue);

      /// <summary>
      ///    Sets the value in the parameter. Value will be converted in kernel unit
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="value">Boolean value that will be converted to 1 if value==true or 0 otherwise </param>
      ICommand SetParameterDisplayValue(IParameter parameter, bool value);

      /// <summary>
      ///    Sets the value in the parameter. Value will be converted in kernel unit
      ///    The command will be a structure change command
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="valueToSetInGuiUnit">Value in display unit</param>
      ICommand SetParameterDisplayValueAsStructureChange(IParameter parameter, double valueToSetInGuiUnit);

      /// <summary>
      ///    Sets the value in the parameter. Value will be converted in kernel unit
      ///    The command will be a structure change command
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="value">Boolean value that will be converted to 1 if value==true or 0 otherwise </param>
      ICommand SetParameterDisplayValueAsStructureChange(IParameter parameter, bool value);

      /// <summary>
      ///    Sets the value in the parameter. Value will be converted in kernel unit
      ///    The command will not induce a change in the vrsion of the containing building block
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="valueToSetInGuiUnit">Value in display unit</param>
      ICommand SetParameterDisplayValueWithoutBuildingBlockChange(IParameter parameter, double valueToSetInGuiUnit);

      /// <summary>
      ///    Sets the display unit of the parameter. Value will not be updated (3 mg=> 3l)
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="displayUnit">displayUnit</param>
      /// <param name="shouldUpdateDefaultStateAndValueOriginForDefaultParameter">
      ///    If set to <c>true</c> default, default state
      ///    and value origin of parameter will be updated as well
      /// </param>
      ICommand SetParameterDisplayUnit(IParameter parameter, Unit displayUnit, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter = true);

      /// <summary>
      ///    Sets the value in the parameter. Value will be take as is
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="value">Value in kernel unit</param>
      /// <param name="shouldUpdateDefaultStateAndValueOriginForDefaultParameter">
      ///    If set to <c>true</c> default, default state
      ///    and value origin of parameter will be updated as well
      /// </param>
      ICommand SetParameterValue(IParameter parameter, double value, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter = true);

      /// <summary>
      ///    Sets the value in the parameter. Value will be taken as is
      ///    The command will be a structure change command
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="value">Value in kernel unit</param>
      /// <param name="shouldUpdateDefaultStateAndValueOrigin">
      ///    If set to <c>true</c> default, default state and value origin of
      ///    parameter will be updated as well
      /// </param>
      ICommand SetParameterValueAsStructureChange(IParameter parameter, double value, bool shouldUpdateDefaultStateAndValueOrigin = true);

      /// <summary>
      ///    Updates the value origin in the parameter. The default flag will be updated
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="valueOrigin">Value origin</param>
      ICommand SetParameterValueOrigin(IParameter parameter, ValueOrigin valueOrigin);

      /// <summary>
      ///    Updates the value origin of all <paramref name="parameters" /> in one single command. In that case, the default flag
      ///    is not updated
      /// </summary>
      /// <param name="parameters">Parameters</param>
      /// <param name="valueOrigin">Value origin</param>
      ICommand SetParametersValueOrigin(IEnumerable<IParameter> parameters, ValueOrigin valueOrigin);

      /// <summary>
      ///    Sets the percentile in the parameter.
      /// </summary>
      /// <param name="parameter">Distribued Parameter</param>
      /// <param name="percentile">Percentile</param>
      ICommand SetParameterPercentile(IParameter parameter, double percentile);

      /// <summary>
      ///    Sets the current unit in the parameter.
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="displayUnit">Display unit</param>
      ICommand SetParameterUnit(IParameter parameter, Unit displayUnit);

      /// <summary>
      ///    Sets the current unit in the parameter and triggers a structural change command
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="displayUnit">Display unit</param>
      ICommand SetParameterUnitAsStructuralChange(IParameter parameter, Unit displayUnit);

      /// <summary>
      ///    Sets the current unit in the parameter. The version of the building block containing the parameter will not be
      ///    changed
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="displayUnit">Display unit</param>
      ICommand SetParameterUnitWithoutBuildingBlockChange(IParameter parameter, Unit displayUnit);

      /// <summary>
      ///    Renames the parameter
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="name">New name for the parameter</param>
      ICommand SetParameterName(IParameter parameter, string name);

      /// <summary>
      ///    Resets all parameters to their default values
      /// </summary>
      /// <param name="parameters">Parameters to be reseted</param>
      ICommand ResetParameters(IEnumerable<IParameter> parameters);

      /// <summary>
      ///    Resets the parameter to its default value
      /// </summary>
      /// <param name="parameter">Parameter to be reseted</param>
      ICommand ResetParameter(IParameter parameter);

      /// <summary>
      ///    Scales the value of the given parameters with the factor
      /// </summary>
      /// <param name="parametersToScale">Parameters whose values will be multipled with the factor</param>
      /// <param name="factor">Scaling factor</param>
      ICommand ScaleParameters(IEnumerable<IParameter> parametersToScale, double factor);

      /// <summary>
      ///    Sets the value in an advanced parameter. Value will be converted in kernel unit
      ///    On top of setting the value in the parameter, a new distribution will be created for the parent advanced parameter
      ///    container
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="valueToSetInGuiUnit">Value in display unit</param>
      ICommand SetAdvancedParameterDisplayValue(IParameter parameter, double valueToSetInGuiUnit);

      /// <summary>
      ///    Sets the display unit in an advanced parameter.
      ///    On top of setting the value in the parameter, a new distribution will be created for the parent advanced parameter
      ///    container
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="displayUnit">Display unit</param>
      ICommand SetAdvancedParameterUnit(IParameter parameter, Unit displayUnit);

      /// <summary>
      ///    Assuming that the given parameters represents all expression parameters, returns a cache containing
      ///    as key an expression parameter, and as value the corresponding expression parameter norm
      /// </summary>
      /// <param name="allExpressionParameters">all expression parameters</param>
      ICache<IParameter, IParameter> GroupExpressionParameters(IReadOnlyList<IParameter> allExpressionParameters);

      /// <summary>
      ///    Adds the parameter to the favorite, or remove the parameter from the favorite
      /// </summary>
      void SetParameterFavorite(IParameter parameter, bool isFavorite);

      /// <summary>
      ///    Sets the given compound type to the compountType Parameter
      /// </summary>
      ICommand SetCompoundType(IParameter compountTypeParameter, CompoundType newCompoundType);

      PathCache<IParameter> PathCacheFor(IEnumerable<IParameter> parameters);

      /// <summary>
      ///    Returns the full path of the <paramref name="parameter" />
      /// </summary>
      string PathFor(IParameter parameter);

      /// <summary>
      ///    Sets the given formula as formula in the parameter.
      /// </summary>
      /// <remarks>
      ///    This should not be used with building block parameters defined in simulation as a reference will be saved in both
      ///    simulation parameters and used building block
      /// </remarks>
      ICommand SetParameterFomula(IParameter parameter, IFormula formula);

      /// <summary>
      ///    Updates the table formula defined in the tableParameter with the valus of the tableFormula! This implicitely
      ///    performs a clone of the table formula and does not update the references in the tableParameter
      /// </summary>
      /// <param name="tableParameter">
      ///    Parameter whose formula should be updated. It is assumed that its formula is a TableFormula
      /// </param>
      /// <param name="tableFormula">TableFormula from which the value will be take</param>
      ICommand UpdateTableFormula(IParameter tableParameter, TableFormula tableFormula);

      /// <summary>
      ///    Updates the table formula defined in the tableParameter with the valus of the tableFormula! This implicitely
      ///    performs a clone of the table formula and does not update the references in the tableParameter. The version of the
      ///    building block containing the parameter will not be changed
      /// </summary>
      /// <param name="tableParameter">
      ///    Parameter whose formula should be updated. It is assumed that its formula is a TableFormula
      /// </param>
      /// <param name="tableFormula">TableFormula from which the value will be take</param>
      ICommand UpdateTableFormulaWithoutBuildingBlockChange(IParameter tableParameter, TableFormula tableFormula);

      /// <summary>
      ///    Updates the distributed table formula defines in the <paramref name="tableParameter" /> using the percentile of the
      ///    given <paramref name="distributedParameter" />. If the formula is not a distributed table formula,
      ///    an empty command is returned.
      /// </summary>
      /// <param name="tableParameter">
      ///    Table parameter containg the <see cref="DistributedTableFormula" /> that will be updated
      /// </param>
      /// <param name="distributedParameter">
      ///    Distributed parameter containg the percentile use to update the <paramref name="tableParameter" /> formula
      /// </param>
      ICommand UpdateDistributedTableFormula(IParameter tableParameter, IDistributedParameter distributedParameter);
   }

   public class ParameterTask : IParameterTask
   {
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IExecutionContext _executionContext;

      private readonly IFavoriteTask _favoriteTask;

      public ParameterTask(IEntityPathResolver entityPathResolver, IExecutionContext executionContext, IFavoriteTask favoriteTask)
      {
         _entityPathResolver = entityPathResolver;
         _executionContext = executionContext;
         _favoriteTask = favoriteTask;
      }

      public ICommand SetParameterDisplayValue(IParameter parameter, double valueToSetInGuiUnit)
      {
         return SetParameterValue(parameter, parameter.ConvertToBaseUnit(valueToSetInGuiUnit));
      }

      public ICommand SetParameterDisplayValue(IParameter parameter, Enum enumValue)
      {
         var oldValue = Enum.ToObject(enumValue.GetType(), (int) parameter.Value);
         var command = SetParameterDisplayValue(parameter, Convert.ToInt32(enumValue));
         return commandWithUpdatedDescription(command, parameter, oldValue, enumValue);
      }

      private ICommand commandWithUpdatedDescription(ICommand command, IParameter parameter, object oldValue, object newValue)
      {
         command.Description = ParameterMessages.SetParameterValue(parameter, _executionContext.DisplayNameFor(parameter), oldValue.ToString(), newValue.ToString());
         return command;
      }

      public ICommand SetParameterDisplayValue(IParameter parameter, bool value)
      {
         var oldValue = (parameter.Value == 1);
         var command = SetParameterDisplayValue(parameter, value ? 1 : 0);
         return commandWithUpdatedDescription(command, parameter, oldValue, value);
      }

      public ICommand SetParameterDisplayValueAsStructureChange(IParameter parameter, double valueToSetInGuiUnit)
      {
         return SetParameterValueAsStructureChange(parameter, parameter.ConvertToBaseUnit(valueToSetInGuiUnit));
      }

      public ICommand SetParameterValueAsStructureChange(IParameter parameter, double value, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter = true)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetParameterValueStructureChangeCommand(parameter, value), parameter, shouldUpdateDefaultStateAndValueOriginForDefaultParameter: shouldUpdateDefaultStateAndValueOriginForDefaultParameter);
      }

      public ICommand SetParameterDisplayValueAsStructureChange(IParameter parameter, bool value)
      {
         var oldValue = (parameter.Value == 1);
         var command = SetParameterDisplayValueAsStructureChange(parameter, value ? 1 : 0);
         return commandWithUpdatedDescription(command, parameter, oldValue, value);
      }

      public ICommand SetParameterDisplayValueWithoutBuildingBlockChange(IParameter parameter, double valueToSetInGuiUnit)
      {
         return setParameterValue(parameter, parameter.ConvertToBaseUnit(valueToSetInGuiUnit), shouldChangeVersion: false, shouldUpdateDefaultStateAndValueOriginForDefaultParameter: true);
      }

      public ICommand SetParameterValue(IParameter parameter, double value, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter = true)
      {
         var shouldChangeVersion = true;

         if (parameter.IsExpression())
            return withUpdatedDefaultStateAndValue(commandForRelativeExpressionParameter(parameter, value).Run(_executionContext), parameter, shouldChangeVersion, shouldUpdateDefaultStateAndValueOriginForDefaultParameter);

         if (parameter.IsStructural())
            return SetParameterValueAsStructureChange(parameter, value, shouldUpdateDefaultStateAndValueOriginForDefaultParameter);

         return setParameterValue(parameter, value, shouldChangeVersion, shouldUpdateDefaultStateAndValueOriginForDefaultParameter);
      }

      private IPKSimCommand commandForRelativeExpressionParameter(IParameter parameter, double value)
      {
         //Normalized expression parameters are readonly and will be updated when setting relative expression
         if (parameter.IsExpressionNorm())
            return new SetRelativeExpressionFromNormalizedCommand(parameter, value);

         var expressionContainer = parameter.ParentContainer;
         switch (expressionContainer.ParentContainer)
         {
            case IndividualMolecule individualMolecule:
               return new SetRelativeExpressionAndNormalizeCommand(individualMolecule, parameter, value);
            default:
               return new SetRelativeExpressionInSimulationAndNormalizedCommand(parameter, value);
         }
      }

      private IOSPSuiteCommand setParameterValue(IParameter parameter, double value, bool shouldChangeVersion, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter)
      {
         return executeAndUpdatedDefaultStateAndValue(
            new SetParameterValueCommand(parameter, value),
            parameter,
            shouldChangeVersion,
            shouldUpdateDefaultStateAndValueOriginForDefaultParameter);
      }

      public ICommand SetParameterDisplayUnit(IParameter parameter, Unit displayUnit, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetParameterDisplayUnitCommand(parameter, displayUnit), parameter, shouldUpdateDefaultStateAndValueOriginForDefaultParameter: shouldUpdateDefaultStateAndValueOriginForDefaultParameter);
      }

      private IOSPSuiteCommand executeAndUpdatedDefaultStateAndValue(BuildingBlockChangeCommand commandToExecute, IParameter parameter, bool shouldChangeVersion = true, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter = true)
      {
         commandToExecute.ShouldChangeVersion = shouldChangeVersion;
         return withUpdatedDefaultStateAndValue(commandToExecute.Run(_executionContext), parameter, shouldChangeVersion, shouldUpdateDefaultStateAndValueOriginForDefaultParameter);
      }

      private IOSPSuiteCommand withUpdatedDefaultStateAndValue(IOSPSuiteCommand executedCommand, IParameter parameter, bool shouldChangeVersion = true, bool shouldUpdateDefaultStateAndValueOriginForDefaultParameter = true)
      {
         if (!shouldUpdateDefaultStateAndValueOriginForDefaultParameter)
            return executedCommand;

         if (!parameter.IsDefault)
            return executedCommand;

         if (executedCommand.IsEmpty())
            return executedCommand;

         var macroCommand = new PKSimMacroCommand().WithHistoryEntriesFrom(executedCommand);
         macroCommand.Add(executedCommand);
         macroCommand.Add(new SetParameterDefaultStateCommand(parameter, isDefault: false) {ShouldChangeVersion = shouldChangeVersion}.Run(_executionContext).AsHidden());

         var setValueOriginCommand = setParameterValueOrigin(parameter, ValueOrigin.Unknown, shouldChangeVersion).AsHidden();
         macroCommand.Add(setValueOriginCommand);
         return macroCommand;
      }

      public ICommand SetParameterPercentile(IParameter parameter, double percentile)
      {
         var distributedParameter = parameter as IDistributedParameter;
         if (distributedParameter == null || ValueComparer.ArePercentilesEqual(distributedParameter.Percentile, percentile))
            return new PKSimEmptyCommand();

         return executeAndUpdatedDefaultStateAndValue(new SetParameterPercentileCommand(distributedParameter, percentile), distributedParameter);
      }

      public ICommand SetParameterUnit(IParameter parameter, Unit displayUnit)
      {
         if (parameter.IsStructural())
            return SetParameterUnitAsStructuralChange(parameter, displayUnit);

         return executeAndUpdatedDefaultStateAndValue(new SetParameterUnitCommand(parameter, displayUnit), parameter);
      }

      public ICommand SetParameterUnitAsStructuralChange(IParameter parameter, Unit displayUnit)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetParameterUnitStructureChangeCommand(parameter, displayUnit), parameter);
      }

      public ICommand SetParameterUnitWithoutBuildingBlockChange(IParameter parameter, Unit displayUnit)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetParameterUnitCommand(parameter, displayUnit), parameter, shouldChangeVersion: false);
      }

      public ICommand UpdateTableFormula(IParameter tableParameter, TableFormula tableFormula)
      {
         return executeAndUpdatedDefaultStateAndValue(new UpdateParameterTableFormulaCommand(tableParameter, tableFormula), tableParameter);
      }

      public ICommand UpdateTableFormulaWithoutBuildingBlockChange(IParameter tableParameter, TableFormula tableFormula)
      {
         return executeAndUpdatedDefaultStateAndValue(new UpdateParameterTableFormulaCommand(tableParameter, tableFormula), tableParameter, shouldChangeVersion: false);
      }

      public ICommand SetParameterName(IParameter parameter, string name)
      {
         return new RenameEntityCommand(parameter, name, _executionContext).Run(_executionContext);
      }

      public ICommand ResetParameters(IEnumerable<IParameter> parameters)
      {
         return new ResetParametersCommand(parameters).Run(_executionContext);
      }

      public ICommand ResetParameter(IParameter parameter)
      {
         return new ResetParameterCommand(parameter).Run(_executionContext);
      }

      public ICommand ScaleParameters(IEnumerable<IParameter> parametersToScale, double factor)
      {
         return new ScaleParametersCommand(parametersToScale, factor).Run(_executionContext);
      }

      public ICommand SetAdvancedParameterDisplayValue(IParameter parameter, double valueToSetInGuiUnit)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetAdvancedParameterValueCommand(parameter, parameter.ConvertToBaseUnit(valueToSetInGuiUnit)), parameter);
      }

      public ICommand SetAdvancedParameterUnit(IParameter parameter, Unit displayUnit)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetAdvancedParameterUnitCommand(parameter, displayUnit), parameter);
      }

      public ICache<IParameter, IParameter> GroupExpressionParameters(IReadOnlyList<IParameter> allExpressionParameters)
      {
         var relativeExpressionsParameters = allExpressionParameters
            .Where(x => !x.Name.Contains(CoreConstants.Parameters.NORM_SUFFIX))
            .Where(x => x.Name.StartsWith(CoreConstants.Parameters.REL_EXP));

         var results = new Cache<IParameter, IParameter>();
         var query = from parameter in relativeExpressionsParameters
            let normParameter = findNormParameterFor(parameter)
            where normParameter != null
            select new {parameter, normParameter};

         query.Each(x => results.Add(x.parameter, x.normParameter));
         return results;
      }

      public void SetParameterFavorite(IParameter parameter, bool isFavorite)
      {
         _favoriteTask.SetParameterFavorite(parameter, isFavorite);
      }

      public ICommand SetCompoundType(IParameter compountTypeParameter, CompoundType newCompoundType)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetCompoundTypeParameterCommand(compountTypeParameter, newCompoundType), compountTypeParameter);
      }

      public PathCache<IParameter> PathCacheFor(IEnumerable<IParameter> parameters)
      {
         return new PathCache<IParameter>(_entityPathResolver).For(parameters);
      }

      public string PathFor(IParameter parameter) => _entityPathResolver.PathFor(parameter);

      public ICommand SetParameterFomula(IParameter parameter, IFormula formula)
      {
         return executeAndUpdatedDefaultStateAndValue(new SetParameterFormulaCommand(parameter, formula), parameter);
      }

      public ICommand UpdateDistributedTableFormula(IParameter tableParameter, IDistributedParameter distributedParameter)
      {
         var distributedTableFormula = tableParameter.Formula as DistributedTableFormula;
         if (distributedTableFormula == null)
            return new PKSimEmptyCommand();

         if (distributedParameter.Formula.DistributionType() == DistributionTypes.Discrete)
         {
            if (ValueComparer.AreValuesEqual(distributedParameter.Value, tableParameter.Value, CoreConstants.DOUBLE_RELATIVE_EPSILON))
               return new PKSimEmptyCommand();

            return executeAndUpdatedDefaultStateAndValue(new UpdateDistributedTableFormulaRatioCommand(tableParameter, distributedParameter.Value / tableParameter.Value), tableParameter);
         }

         if (ValueComparer.ArePercentilesEqual(distributedTableFormula.Percentile, distributedParameter.Percentile))
            return new PKSimEmptyCommand();

         return executeAndUpdatedDefaultStateAndValue(new UpdateDistributedTableFormulaPercentileCommand(tableParameter, distributedParameter.Percentile), tableParameter);
      }

      private IParameter findNormParameterFor(IParameter relativeExpression)
      {
         return relativeExpression.ParentContainer.Parameter(CoreConstants.Parameters.NormParameterFor(relativeExpression.Name));
      }

      public ICommand SetParameterValue(IParameter parameter, double value, ISimulation simulation)
      {
         return SetParameterValue(parameter, value);
      }

      public ICommand UpdateParameterValueOrigin(IParameter parameter, ValueOrigin valueOrigin, ISimulation simulation)
      {
         return SetParameterValueOrigin(parameter, valueOrigin);
      }

      public ICommand SetParameterValueOrigin(IParameter parameter, ValueOrigin newValueOrigin)
      {
         return setParameterValueOrigin(parameter, newValueOrigin, shouldChangeVersion: true);
      }

      private ICommand setParameterValueOrigin(IParameter parameter, ValueOrigin newValueOrigin, bool shouldChangeVersion)
      {
         return new UpdateParameterValueOriginCommand(parameter, newValueOrigin) {ShouldChangeVersion = shouldChangeVersion}.Run(_executionContext);
      }

      public ICommand SetParametersValueOrigin(IEnumerable<IParameter> parameters, ValueOrigin valueOrigin)
      {
         return new UpdateParametersValueOriginCommand(parameters, valueOrigin).Run(_executionContext);
      }
   }
}