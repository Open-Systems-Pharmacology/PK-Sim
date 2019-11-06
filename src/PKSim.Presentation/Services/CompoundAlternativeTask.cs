using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Importer;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using Unit = OSPSuite.Core.Domain.UnitSystem.Unit;

namespace PKSim.Presentation.Services
{
   public class CompoundAlternativeTask : ICompoundAlternativeTask
   {
      private readonly IParameterAlternativeFactory _parameterAlternativeFactory;
      private readonly IApplicationController _applicationController;
      private readonly IExecutionContext _executionContext;
      private readonly ICompoundFactory _compoundFactory;
      private readonly IEntityTask _entityTask;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IParameterTask _parameterTask;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IDataImporter _dataImporter;

      public CompoundAlternativeTask(
         IParameterAlternativeFactory parameterAlternativeFactory,
         IApplicationController applicationController,
         IExecutionContext executionContext,
         ICompoundFactory compoundFactory,
         IEntityTask entityTask,
         IFormulaFactory formulaFactory,
         IParameterTask parameterTask,
         IBuildingBlockRepository buildingBlockRepository,
         IDimensionRepository dimensionRepository,
         IDataImporter dataImporter)
      {
         _parameterAlternativeFactory = parameterAlternativeFactory;
         _applicationController = applicationController;
         _executionContext = executionContext;
         _compoundFactory = compoundFactory;
         _entityTask = entityTask;
         _formulaFactory = formulaFactory;
         _parameterTask = parameterTask;
         _buildingBlockRepository = buildingBlockRepository;
         _dimensionRepository = dimensionRepository;
         _dataImporter = dataImporter;
      }

      public ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup)
      {
         var parameterAlternative = compoundParameterGroup.IsNamed(CoreConstants.Groups.COMPOUND_SOLUBILITY) ? 
            createSolubilityCompoundAlternativeFor(compoundParameterGroup) : 
            createStandardParameterAlternativeFor(compoundParameterGroup);

         if (parameterAlternative == null)
            return new PKSimEmptyCommand();

         return AddParameterGroupAlternativeTo(compoundParameterGroup, parameterAlternative);
      }

      private ParameterAlternative createStandardParameterAlternativeFor(ParameterAlternativeGroup compoundParameterGroup)
      {
         using (var presenter = _applicationController.Start<IParameterAlternativeNamePresenter>())
         {
            //canceled by user - nothing to do
            if (!presenter.Edit(compoundParameterGroup))
               return null;

            return createAlternative(compoundParameterGroup, presenter.Name);
         }
      }

      private ParameterAlternative createSolubilityCompoundAlternativeFor(ParameterAlternativeGroup solubilityAlternativeGroup)
      {
         using (var presenter = _applicationController.Start<ISolubilityAlternativeNamePresenter>())
         {
            //canceled by user - nothing to do
            if (!presenter.Edit(solubilityAlternativeGroup))
               return null;

            if (presenter.CreateAsTable)
               return createSolubilityTableAlternativeFor(solubilityAlternativeGroup, presenter.Name);

            return createAlternative(solubilityAlternativeGroup, presenter.Name);
         }
      }

      private ParameterAlternative createSolubilityTableAlternativeFor(ParameterAlternativeGroup solubilityAlternativeGroup, string name)
      {
         var tableAlternative = _parameterAlternativeFactory.CreateTableAlternativeFor(solubilityAlternativeGroup, CoreConstants.Parameters.SOLUBILITY_TABLE)
            .WithName(name);

         PrepareSolubilityAlternativeForTableSolubility(tableAlternative);

         var tableParameter = tableAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);
         var phParameter = tableAlternative.Parameter(CoreConstants.Parameters.REFERENCE_PH);

         var tableFormula = tableParameter.Formula.DowncastTo<TableFormula>();
         initializeSolubiltyTableFormula(tableFormula, phParameter.Dimension, tableParameter.Dimension);

         tableFormula.AddPoint(0, 0);
         return tableAlternative;
      }

      public void PrepareSolubilityAlternativeForTableSolubility(ParameterAlternative solubilityAlternative)
      {
         var solubilityAtRefPhParameter = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH);
         var phParameter = solubilityAlternative.Parameter(CoreConstants.Parameters.REFERENCE_PH);
         var solubilityGainParameter = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE);
         resetParameters(solubilityGainParameter, solubilityAtRefPhParameter, phParameter);
         solubilityAtRefPhParameter.Value = 0;

         var tableParameter = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);
         tableParameter.Visible = true;
         tableParameter.IsDefault = false;
      }

      private static void resetParameters(params IParameter[] parameters) => parameters.Each(p =>
      {
         p.IsDefault = true;
         p.Visible = false;
      });

      private ParameterAlternative createAlternative(ParameterAlternativeGroup compoundParameterGroup, string name) => _parameterAlternativeFactory.CreateAlternativeFor(compoundParameterGroup).WithName(name);

      public ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup, ParameterAlternative parameterAlternative)
      {
         return new AddParameterAlternativeCommand(parameterAlternative, compoundParameterGroup, _executionContext).Run(_executionContext);
      }

      public ICommand RemoveParameterGroupAlternative(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative)
      {
         if (parameterAlternative.IsDefault)
            throw new CannotDeleteDefaultParameterAlternativeException();

         if (parameterGroup.AllAlternatives.Count() == 1)
            throw new CannotDeleteParameterAlternativeException();

         return new RemoveParameterAlternativeCommand(parameterAlternative, parameterGroup, _executionContext).Run(_executionContext);
      }

      public ICommand RenameParameterAlternative(ParameterAlternative parameterAlternative)
      {
         return _entityTask.StructuralRename(parameterAlternative);
      }

      public ICommand SetAlternativeParameterValue(IParameter parameter, double valueInDisplayUnit)
      {
         if (simulationsAreUsingAlternativeContaining(parameter))
            return _parameterTask.SetParameterDisplayValue(parameter, valueInDisplayUnit);

         return _parameterTask.SetParameterDisplayValueWithoutBuildingBlockChange(parameter, valueInDisplayUnit);
      }

      public ICommand SetAlternativeParameterUnit(IParameter parameter, Unit newUnit)
      {
         if (simulationsAreUsingAlternativeContaining(parameter))
            return _parameterTask.SetParameterUnit(parameter, newUnit);

         return _parameterTask.SetParameterUnitWithoutBuildingBlockChange(parameter, newUnit);
      }

      public ICommand SetAlternativeParameterTable(IParameter parameter, TableFormula formula)
      {
         if (simulationsAreUsingAlternativeContaining(parameter))
            return _parameterTask.UpdateTableFormula(parameter, formula);

         return _parameterTask.UpdateTableFormulaWithoutBuildingBlockChange(parameter, formula);
      }

      public ICommand EditSolubilityTableFor(IParameter parameter)
      {
         using (var tablePresenter = _applicationController.Start<IEditTableSolubilityParameterPresenter>())
         {
            if (!tablePresenter.Edit(parameter))
               return new PKSimEmptyCommand();

            return SetAlternativeParameterTable(parameter, tablePresenter.EditedFormula);
         }
      }

      private bool simulationsAreUsingAlternativeContaining(IParameter parameter)
      {
         var alternative = parameter.ParentContainer as ParameterAlternative;
         return alternative != null && simulationsAreUsingAlternative(alternative);
      }

      private bool simulationsAreUsingAlternative(ParameterAlternative alternative)
      {
         var allCompoundGroupSelectionForAlternativeGroup = _buildingBlockRepository.All<Simulation>()
            .SelectMany(x => x.CompoundPropertiesList)
            .SelectMany(x => x.CompoundGroupSelections
               .Where(groupSel => string.Equals(groupSel.GroupName, alternative.GroupName))
            );

         return allCompoundGroupSelectionForAlternativeGroup.Any(x => string.Equals(x.AlternativeName, alternative.Name));
      }

      public ICommand UpdateValueOrigin(ParameterAlternative parameterAlternative, ValueOrigin newValueOrigin)
      {
         var shouldChangeBuildingBlockVersion = simulationsAreUsingAlternative(parameterAlternative);
         var updateValueOriginCommand = new UpdateParametersValueOriginCommand(parameterAlternative.AlllParametersWithSameValueOrigin, newValueOrigin, shouldChangeBuildingBlockVersion);

         return updateValueOriginCommand.Run(_executionContext);
      }

      public IEnumerable<IParameter> PermeabilityValuesFor(Compound compound)
      {
         return permeabilityParametersFor(compound, CoreConstants.Parameters.PERMEABILITY);
      }

      public IEnumerable<IParameter> IntestinalPermeabilityValuesFor(Compound compound)
      {
         return permeabilityParametersFor(compound, CoreConstants.Parameters.SPECIFIC_INTESTINAL_PERMEABILITY);
      }

      public ICommand SetDefaultAlternativeFor(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative)
      {
         return new SetDefaultAlternativeParameterCommand(parameterGroup, parameterAlternative, _executionContext).Run(_executionContext);
      }

      public ICommand SetSpeciesForAlternative(ParameterAlternativeWithSpecies parameterAlternative, Species species)
      {
         return new SetSpeciesInSpeciesDependentEntityCommand(parameterAlternative, species, _executionContext).Run(_executionContext);
      }

      public TableFormula ImportSolubilityTableFormula()
      {
         var dataImporterSettings = new DataImporterSettings
         {
            Caption = $"{CoreConstants.ProductDisplayName} - {PKSimConstants.UI.ImportSolubilityTable}",
            IconName = ApplicationIcons.Compound.IconName
         };

         var importedFormula = _dataImporter.ImportDataSet(new List<MetaDataCategory>(), getColumnInfos(), dataImporterSettings);
         return importedFormula == null ? null : formulaFrom(importedFormula);
      }

      private TableFormula formulaFrom(DataRepository dataRepository)
      {
         var baseGrid = dataRepository.BaseGrid;
         var valueColumn = dataRepository.AllButBaseGrid().Single();
         var formula = _formulaFactory.CreateTableFormula(useDerivedValues: false);
         initializeSolubiltyTableFormula(formula, baseGrid.Dimension, valueColumn.Dimension);
         formula.XDisplayUnit = baseGrid.Dimension.Unit(baseGrid.DataInfo.DisplayUnitName);
         formula.YDisplayUnit = valueColumn.Dimension.Unit(valueColumn.DataInfo.DisplayUnitName);

         foreach (var timeValue in baseGrid.Values)
         {
            formula.AddPoint(timeValue, valueColumn.GetValue(timeValue).ToDouble());
         }

         return formula;
      }

      private IReadOnlyList<ColumnInfo> getColumnInfos()
      {
         var columns = new List<ColumnInfo>();

         var phColumn = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.NoDimension,
            Name = PKSimConstants.UI.pH,
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
         };


         phColumn.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.NoDimension, IsMainDimension = true});
         columns.Add(phColumn);

         var solubilityColumn = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.MassConcentration,
            Name = PKSimConstants.UI.Solubility,
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
            BaseGridName = phColumn.Name,
         };

         solubilityColumn.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.MassConcentration, IsMainDimension = true});
         columns.Add(solubilityColumn);

         return columns;
      }

      public TableFormula SolubilityTableForPh(ParameterAlternative solubilityAlternative, Compound compound)
      {
         //Already a table formula. Use as IS!
         var tableSolubility = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);
         if (tableSolubility.Formula.IsTable())
            return tableSolubility.Formula.DowncastTo<TableFormula>();

         //Sol(pH) = ref_Solubility * Solubility_Factor (ref_pH) / Solubility_Factor(pH) 
         //Solubility_pKa_pH_Factor

         var refSolubility = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH);
         var refPh = solubilityAlternative.Parameter(CoreConstants.Parameters.REFERENCE_PH);
         var gainPerCharge = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE);
         var refSolubilityValue = refSolubility.Value;

         var compoundForCalculation = _executionContext.Clone(compound);
         var formula = initializeSolubiltyTableFormula(_formulaFactory.CreateTableFormula(), refPh.Dimension, refSolubility.Dimension);
         compoundForCalculation.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value = refPh.Value;
         compoundForCalculation.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE).Value = gainPerCharge.Value;

         double solFactorRefpH = compoundForCalculation.Parameter(CoreConstants.Parameters.SOLUBILITY_P_KA__P_H_FACTOR).Value;
         var allPh = new List<double>();
         int ph = 0;
         while (ph <= 13)
         {
            allPh.AddRange(new[] {ph, ph + 0.5});
            ph++;
         }

         allPh.Add(14);
         foreach (var pH in allPh)
         {
            compoundForCalculation.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value = pH;
            double solFactorAtpH = compoundForCalculation.Parameter(CoreConstants.Parameters.SOLUBILITY_P_KA__P_H_FACTOR).Value;
            formula.AddPoint(pH, refSolubilityValue * solFactorRefpH / solFactorAtpH);
         }

         return formula;
      }

      private TableFormula initializeSolubiltyTableFormula(TableFormula tableFormula, IDimension xDimension, IDimension yDimension)
      {
         tableFormula.UseDerivedValues = false;

         return tableFormula
            .WithName(PKSimConstants.UI.Solubility)
            .InitializedWith(PKSimConstants.UI.pH, PKSimConstants.UI.Solubility, xDimension, yDimension);
      }

      private IEnumerable<IParameter> permeabilityParametersFor(Compound compound, string permeabilityParameterName)
      {
         //create a temp compound from the compound factory
         //retrieve the lipophilicity alternatives
         var lipophilicityGroup = compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         foreach (var alternative in lipophilicityGroup.AllAlternatives)
         {
            var tempCompound = _compoundFactory.Create();
            tempCompound.Parameter(Constants.Parameters.IS_SMALL_MOLECULE).Value = compound.Parameter(Constants.Parameters.IS_SMALL_MOLECULE).Value;
            tempCompound.Parameter(CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT).Value = compound.Parameter(CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT).Value;
            tempCompound.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value = alternative.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value;
            var permParameter = tempCompound.Parameter(permeabilityParameterName);
            permParameter.Editable = false;
            permParameter.Name = alternative.Name;
            yield return permParameter;
         }
      }
   }
}