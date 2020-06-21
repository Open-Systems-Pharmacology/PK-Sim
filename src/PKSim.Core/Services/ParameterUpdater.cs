using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IParameterUpdater
   {
      /// <summary>
      ///    Update the parameter formula from the source parameter into the target parameter without any other
      ///    checking necessary
      /// </summary>
      /// <param name="sourceParameter">source parameter from which the formula will be taken</param>
      /// <param name="targetParameter">target parameter whose formula will be set</param>
      ICommand UpdateValue(IParameter sourceParameter, IParameter targetParameter);
   }

   public class ParameterUpdater : IParameterUpdater
   {
      private readonly IParameterToFomulaTypeMapper _formulaTypeMapper;
      private readonly IParameterTask _parameterTask;

      public ParameterUpdater(IParameterToFomulaTypeMapper formulaTypeMapper, IParameterTask parameterTask)
      {
         _formulaTypeMapper = formulaTypeMapper;
         _parameterTask = parameterTask;
      }

      public ICommand UpdateValue(IParameter sourceParameter, IParameter targetParameter)
      {
         var targetformulaType = _formulaTypeMapper.MapFrom(targetParameter);
         var sourceformulaType = _formulaTypeMapper.MapFrom(sourceParameter);

         if (targetformulaType == FormulaType.Distribution)
            return setDistributionValue(sourceParameter, targetParameter.DowncastTo<IDistributedParameter>());

         if (targetformulaType == FormulaType.Table)
            return setTableFormulaValue(sourceParameter, targetParameter);

         //source parameter is fixed, target parameter should be fixed as well
         if (sourceParameter.IsFixedValue)
            return setParameterValue(sourceParameter, targetParameter, forceUpdate: true);

         //user did not change the value in the simulation=>no update required
         if (sourceParameter.BuildingBlockType == PKSimBuildingBlockType.Simulation)
            return null;

         //from here source is not fixed and building block type is not simulation
         if (targetformulaType == FormulaType.Constant)
            return setParameterValue(sourceParameter, targetParameter, forceUpdate: false);

         //from here source is not fixed and target parameter is formula

         //target is a formula and source is a constant always update value (hence true) //Permeability
         if (sourceformulaType == FormulaType.Constant)
            return setParameterValue(sourceParameter, targetParameter, forceUpdate: true);

         //both source and target are formula
         //None of the parameters was set by the user, nothing to change
         if (!targetParameter.IsFixedValue)
            return null;

         //only target parameter was changed, reset the parameter
         return resetParameterValue(targetParameter);
      }

      private ICommand setTableFormulaValue(IParameter sourceParameter, IParameter targetParameter)
      {
         //target formula is alwyays a table:
         var targetTableFormula = targetParameter.Formula as DistributedTableFormula;

         //updating a table formula from a distributed parameter? only for distributedTableFormula
         var sourceDistributedParameter = sourceParameter as IDistributedParameter;

         if (targetTableFormula != null && sourceDistributedParameter != null)
            return updateDistributedTableFormula(targetParameter, sourceDistributedParameter);

         //if source parameter as a constant formula, nothing to update as it is a simple growth table
         if (!sourceParameter.Formula.IsTable())
            return null;

         return updateTableFormula(sourceParameter, targetParameter);
      }

      private ICommand updateTableFormula(IParameter sourceParameter, IParameter targetParameter)
      {
         return _parameterTask.UpdateTableFormula(targetParameter, sourceParameter.Formula.DowncastTo<TableFormula>());
      }

      private ICommand updateDistributedTableFormula(IParameter targetParameter, IDistributedParameter sourceDistributedParameter)
      {
         return _parameterTask.UpdateDistributedTableFormula(targetParameter, sourceDistributedParameter);
      }

      private ICommand setDistributionValue(IParameter sourceParameter, IDistributedParameter targetParameter)
      {
         //distributed to distributed
         var sourceDistributedParameter = sourceParameter as IDistributedParameter;
         if (sourceDistributedParameter != null)
            return setParameterValue(sourceParameter, targetParameter, false);

         //source parameter it not distributed. Is the formula a table paramter? In that case, we are most likely
         //updating from simulation table parameter to distributed parameter in individual
         var tableFormula = sourceParameter.Formula as DistributedTableFormula;
         if (tableFormula == null)
            return setParameterValue(sourceParameter, targetParameter, false);

         return _parameterTask.SetParameterPercentile(targetParameter, tableFormula.Percentile);
      }

      private ICommand resetParameterValue(IParameter targetParameter)
      {
         return targetParameter.IsFixedValue ? _parameterTask.ResetParameter(targetParameter) : null;
      }

      private ICommand setParameterValue(IParameter sourceParameter, IParameter targetParameter, bool forceUpdate)
      {
         //If the parameter we are updating from is a default parameter, the target parameter should either remain as is (default or not default)
         bool shouldUpdateDefaultState = !sourceParameter.IsDefault;

         //Always update the default value of the target parameter
         targetParameter.DefaultValue = sourceParameter.DefaultValue;
         if (!forceUpdate && areValuesEqual(targetParameter, sourceParameter))
         {
            //same value but not same display unit, simply update the display unit
            if (areDisplayUnitsEqual(targetParameter, sourceParameter))
               return null;

            return _parameterTask.SetParameterDisplayUnit(targetParameter, sourceParameter.DisplayUnit, shouldUpdateDefaultState);
         }

         var setValueCommand = _parameterTask.SetParameterValue(targetParameter, sourceParameter.Value, shouldUpdateDefaultState);

         //Only value differs
         if (areDisplayUnitsEqual(targetParameter, sourceParameter))
            return setValueCommand;

         //in that case, we create a macro command that updates value and unit
         var setDisplayUnitCommand = _parameterTask.SetParameterDisplayUnit(targetParameter, sourceParameter.DisplayUnit, shouldUpdateDefaultState);
         var macroCommand = new PKSimMacroCommand {CommandType = setValueCommand.CommandType, ObjectType = setValueCommand.ObjectType, Description = PKSimConstants.Command.SetParameterValueAndDisplayUnitDescription};
         macroCommand.Add(setValueCommand);
         macroCommand.Add(setDisplayUnitCommand);
         return macroCommand;
      }

      private bool areValuesEqual(IParameter targetParameter, IParameter sourceParameter)
      {
         return ValueComparer.AreValuesEqual(targetParameter, sourceParameter);
      }

      private bool areDisplayUnitsEqual(IParameter targetParameter, IParameter sourceParameter)
      {
         //We compare name are some dose related units may be the representing the same unit but with different reference
         return string.Equals(targetParameter.DisplayUnit?.Name, sourceParameter.DisplayUnit?.Name);
      }
   }
}