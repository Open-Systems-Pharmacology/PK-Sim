using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
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
      ICommand SetParameterDisplayUnit(IParameter parameter, Unit displayUnit);

      /// <summary>
      ///    Sets the value in the parameter. Value will be take as is. If <paramref name="scaleRelatedParameters"/> is set to <c>true</c>, related parameters will also be adjusted. 
      ///    This is typically the case when the parameter is updated by the user
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="value">Value in kernel unit</param>
      /// <param name="scaleRelatedParameters">If <c>true</c>, depending parameters will also be adjusted (for example relative expression norms).</param>
      ICommand SetParameterValue(IParameter parameter, double value, bool scaleRelatedParameters = true);

      /// <summary>
      ///    Sets the value in the parameter. Value will be taken as is
      ///    The command will be a structure change command
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="value">Value in kernel unit</param>
      ICommand SetParameterValueAsStructureChange(IParameter parameter, double value);

      /// <summary>
      ///    Sets the value description in the parameter.
      /// </summary>
      /// <param name="parameter">Parameter</param>
      /// <param name="valueDescription">Value description</param>
      ICommand SetParameterValueDescription(IParameter parameter, string valueDescription);

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
      ///    Sets the current unit in the parameter. The version of the building block containing the parmaeter will not be
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
      ///    sets the given formula as formula in the parameter
      /// </summary>
      ICommand SetParameterFomula(IParameter parameter, IFormula formula);

      /// <summary>
      ///    Updates the table formula defined in the tableParameter with the valus of the tableFormula! This implicitely
      ///    performs
      ///    a clone of the table formula and does not update the references in the tableParameter
      /// </summary>
      /// <param name="tableParameter">
      ///    Parameter whose formula should be updated. It is assumed that the formula is a
      ///    TableFormula
      /// </param>
      /// <param name="tableFormula">TableFormula from which the value will be take</param>
      ICommand UpdateTableFormula(IParameter tableParameter, TableFormula tableFormula);

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

      public ICommand SetParameterValueAsStructureChange(IParameter parameter, double value)
      {
         return new SetParameterValueStructureChangeCommand(parameter, value).Run(_executionContext);
      }

      public ICommand SetParameterDisplayValueAsStructureChange(IParameter parameter, bool value)
      {
         var oldValue = (parameter.Value == 1);
         var command = SetParameterDisplayValueAsStructureChange(parameter, value ? 1 : 0);
         return commandWithUpdatedDescription(command, parameter, oldValue, value);
      }

      public ICommand SetParameterDisplayValueWithoutBuildingBlockChange(IParameter parameter, double valueToSetInGuiUnit)
      {
         return setParameterValue(parameter, parameter.ConvertToBaseUnit(valueToSetInGuiUnit), shouldChangeVersion: false);
      }

      public ICommand SetParameterValue(IParameter parameter, double value, bool scaleRelatedParameters = true)
      {
         if (scaleRelatedParameters && parameter.IsExpression() )
            return commandForRelativeExpressionParameter(parameter, value).Run(_executionContext);

         if (parameter.IsStructural())
            return SetParameterValueAsStructureChange(parameter, value);

         return setParameterValue(parameter, value, shouldChangeVersion: true);
      }

      private ICommand<IExecutionContext> commandForRelativeExpressionParameter(IParameter parameter, double value)
      {
         if (!parameter.IsExpressionNorm())
            return new SetRelativeExpressionInSimulationAndNormalizedCommand(parameter, value);

         return new SetRelativeExpressionFromNormalizedCommand(parameter, value);
      }

      public ICommand SetParameterValueDescription(IParameter parameter, string valueDescription)
      {
         //no command required here
         parameter.ValueDescription = valueDescription;
         var originParameter = _executionContext.Get<IParameter>(parameter.Origin.ParameterId);
         if (originParameter != null)
            originParameter.ValueDescription = valueDescription;

         return new PKSimEmptyCommand();
      }

      private ICommand setParameterValue(IParameter parameter, double value, bool shouldChangeVersion)
      {
         return new SetParameterValueCommand(parameter, value) {ShouldChangeVersion = shouldChangeVersion}.Run(_executionContext);
      }

      public ICommand SetParameterDisplayUnit(IParameter parameter, Unit displayUnit)
      {
         return new SetParameterDisplayUnitCommand(parameter, displayUnit).Run(_executionContext);
      }

      public ICommand SetParameterPercentile(IParameter parameter, double percentile)
      {
         var distributedParameter = parameter as IDistributedParameter;
         if (distributedParameter == null || ValueComparer.ArePercentilesEqual(distributedParameter.Percentile, percentile))
            return new PKSimEmptyCommand();

         return new SetParameterPercentileCommand(distributedParameter, percentile).Run(_executionContext);
      }

      public ICommand SetParameterUnit(IParameter parameter, Unit displayUnit)
      {
         if (parameter.IsStructural())
            return SetParameterUnitAsStructuralChange(parameter, displayUnit);

         return new SetParameterUnitCommand(parameter, displayUnit).Run(_executionContext);
      }

      public ICommand SetParameterUnitAsStructuralChange(IParameter parameter, Unit displayUnit)
      {
         return new SetParameterUnitStructureChangeCommand(parameter, displayUnit).Run(_executionContext);
      }

      public ICommand SetParameterUnitWithoutBuildingBlockChange(IParameter parameter, Unit displayUnit)
      {
         return new SetParameterUnitCommand(parameter, displayUnit) {ShouldChangeVersion = false}.Run(_executionContext);
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
         return new SetAdvancedParameterValueCommand(parameter, parameter.ConvertToBaseUnit(valueToSetInGuiUnit)).Run(_executionContext);
      }

      public ICommand SetAdvancedParameterUnit(IParameter parameter, Unit displayUnit)
      {
         return new SetAdvancedParameterUnitCommand(parameter, displayUnit).Run(_executionContext);
      }

      public ICache<IParameter, IParameter> GroupExpressionParameters(IReadOnlyList<IParameter> allExpressionParameters)
      {
         var relativeExpressionsParameters = allExpressionParameters
            .Where(x => !x.Name.Contains(CoreConstants.Parameter.NORM_SUFFIX))
            .Where(x => x.Name.StartsWith(CoreConstants.Parameter.REL_EXP));

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
         return new SetCompoundTypeParameterCommand(compountTypeParameter, newCompoundType).Run(_executionContext);
      }

      public PathCache<IParameter> PathCacheFor(IEnumerable<IParameter> parameters)
      {
         return new PathCache<IParameter>(_entityPathResolver).For(parameters);
      }

      public ICommand SetParameterFomula(IParameter parameter, IFormula formula)
      {
         return new SetParameterFormulaCommand(parameter, formula).Run(_executionContext);
      }

      public ICommand UpdateTableFormula(IParameter tableParameter, TableFormula tableFormula)
      {
         return new UpdateParameterTableFormulaCommand(tableParameter, tableFormula).Run(_executionContext);
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

            return new UpdateDistributedTableFormulaRatioCommand(tableParameter, distributedParameter.Value / tableParameter.Value).Run(_executionContext);
         }

         if (ValueComparer.ArePercentilesEqual(distributedTableFormula.Percentile, distributedParameter.Percentile))
            return new PKSimEmptyCommand();

         return new UpdateDistributedTableFormulaPercentileCommand(tableParameter, distributedParameter.Percentile).Run(_executionContext);
      }

      private IParameter findNormParameterFor(IParameter relativeExpression)
      {
         return relativeExpression.ParentContainer.Parameter(CoreConstants.Parameter.NormParameterFor(relativeExpression.Name));
      }

      public ICommand SetParameterValue(IParameter parameter, double value, ISimulation simulation)
      {
         return SetParameterValue(parameter, value);
      }
   }
}