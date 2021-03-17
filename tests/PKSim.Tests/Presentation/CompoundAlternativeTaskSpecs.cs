using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using Dimension = OSPSuite.Core.Domain.UnitSystem.Dimension;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using Unit = OSPSuite.Core.Domain.UnitSystem.Unit;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundAlternativeTask : ContextSpecification<ICompoundAlternativeTask>
   {
      protected IParameterAlternativeFactory _parameterAlternativeFactory;
      protected IExecutionContext _executionContext;
      protected ICompoundFactory _compoundFactory;
      protected IFormulaFactory _formulaFactory;
      protected IParameterTask _parameterTask;
      protected IBuildingBlockRepository _buildingBlockRepository;
      private IDimensionRepository _dimensionRepository;
      protected IDataImporter _dataImporter;

      protected override void Context()
      {
         _parameterAlternativeFactory = A.Fake<IParameterAlternativeFactory>();
         _executionContext = A.Fake<IExecutionContext>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         _parameterTask = A.Fake<IParameterTask>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _dataImporter = A.Fake<IDataImporter>();
         _compoundFactory = new CompoundFactoryForSpecs();
         sut = new CompoundAlternativeTask(_parameterAlternativeFactory,
            _executionContext, _compoundFactory, _formulaFactory, _parameterTask,
            _buildingBlockRepository, _dimensionRepository, _dataImporter);
      }

      protected class CompoundFactoryForSpecs : ICompoundFactory
      {
         public Compound Create()
         {
            var compound = new Compound();
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT));
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(CoreConstants.Parameters.LIPOPHILICITY));
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(8).WithName(CoreConstants.Parameters.PERMEABILITY));
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.Parameters.IS_SMALL_MOLECULE));
            return compound;
         }
      }
   }

   public class When_the_compound_task_is_removing_a_parameter_alternative_marked_as_default_alternative : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternative _alternative;
      private ParameterAlternativeGroup _groupAlternative;

      protected override void Context()
      {
         base.Context();
         _alternative = new ParameterAlternative().WithName("Alt1");
         _alternative.IsDefault = true;
         _groupAlternative = new ParameterAlternativeGroup();
         _groupAlternative.AddAlternative(_alternative);
         _groupAlternative.AddAlternative(new ParameterAlternative().WithName("Alt2"));
      }

      [Observation]
      public void should_throw_an_exception_stipulating_that_the_default_alternative_cannot_be_deleted()
      {
         The.Action(() => sut.RemoveParameterGroupAlternative(_groupAlternative, _alternative)).ShouldThrowAn<CannotDeleteDefaultParameterAlternativeException>();
      }
   }

   public class When_the_compound_task_is_removing_the_only_parameter_alternative_defined_in_a_parameter_group : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternative _alternative;
      private ParameterAlternativeGroup _groupAlternative;

      protected override void Context()
      {
         base.Context();
         _alternative = new ParameterAlternative();
         _groupAlternative = new ParameterAlternativeGroup();
         _groupAlternative.AddAlternative(_alternative);
      }

      [Observation]
      public void should_throw_an_exception_stipulating_that_at_least_one_exception_needs_to_be_defined()
      {
         The.Action(() => sut.RemoveParameterGroupAlternative(_groupAlternative, _alternative)).ShouldThrowAn<CannotDeleteParameterAlternativeException>();
      }
   }

   public class When_calculating_the_possible_values_for_the_permeability_based_on_the_lipophilicty_values : concern_for_CompoundAlternativeTask
   {
      private Compound _compound;
      private IEnumerable<IParameter> _results;
      private ParameterAlternative _alternative1;
      private ParameterAlternative _alternative2;

      protected override void Context()
      {
         base.Context();
         //A compound with 2 lipo alternatives and MolWeight Eff
         _compound = new CompoundFactoryForSpecs().Create();
         var lipoGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         _alternative1 = new ParameterAlternative().WithName("ALT1");
         _alternative1.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.LIPOPHILICITY));
         lipoGroup.AddAlternative(_alternative1);

         _alternative2 = new ParameterAlternative().WithName("ALT2");
         _alternative2.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(CoreConstants.Parameters.LIPOPHILICITY));
         lipoGroup.AddAlternative(_alternative2);
         _compound.AddParameterAlternativeGroup(lipoGroup);
      }

      protected override void Because()
      {
         _results = sut.PermeabilityValuesFor(_compound);
      }

      [Observation]
      public void should_return_one_parameter_for_each_defined_lipophilcity_alternative()
      {
         _results.Count().ShouldBeEqualTo(2);
      }
   }

   public class When_retrieving_the_table_formula_for_a_solubility_alternative : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternative _solubilityAlternative;
      private Compound _compound;
      private TableFormula _tableFormula;
      private IParameter _solubility_pKa_pH_Factor;
      private double _refPhValue;
      private double _solValue;
      private double _gainPerChargeValue;
      private Compound _compoundForCalculation;
      private IParameter _originalRefPh;

      protected override void Context()
      {
         base.Context();
         var solDim = new Dimension(new BaseDimensionRepresentation(), "Solubility", "m");
         solDim.AddUnit(new Unit("cm", 0.01, 0));
         _refPhValue = 7;
         _solValue = 100;
         _gainPerChargeValue = 1000;
         A.CallTo(_formulaFactory).WithReturnType<TableFormula>().Returns(new TableFormula());
         var solubilityTable = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.SOLUBILITY_TABLE).WithDimension(solDim);
         var refPh = DomainHelperForSpecs.ConstantParameterWithValue(_refPhValue).WithName(CoreConstants.Parameters.REFERENCE_PH).WithDimension(DomainHelperForSpecs.NoDimension());
         var solubilty = DomainHelperForSpecs.ConstantParameterWithValue(_solValue).WithName(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH).WithDimension(solDim);
         var gainPerCharge = DomainHelperForSpecs.ConstantParameterWithValue(_gainPerChargeValue).WithName(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE).WithDimension(DomainHelperForSpecs.NoDimension());
         _solubility_pKa_pH_Factor = new PKSimParameter().WithName(CoreConstants.Parameters.SOLUBILITY_P_KA__P_H_FACTOR);
         _solubility_pKa_pH_Factor.Formula = new ExplicitFormula("10 * (pH +1)");
         _solubility_pKa_pH_Factor.Formula.AddObjectPath(new FormulaUsablePath(new[] {ObjectPath.PARENT_CONTAINER, CoreConstants.Parameters.REFERENCE_PH}).WithAlias("pH"));

         _compoundForCalculation = new Compound {refPh, solubilty, gainPerCharge, _solubility_pKa_pH_Factor};
         _solubilityAlternative = new ParameterAlternative {refPh, solubilty, gainPerCharge, solubilityTable};

         var solubilityAlternativeGroup = new ParameterAlternativeGroup();
         solubilityAlternativeGroup.AddAlternative(_solubilityAlternative);


         _originalRefPh = DomainHelperForSpecs.ConstantParameterWithValue(_refPhValue).WithName(CoreConstants.Parameters.REFERENCE_PH).WithDimension(DomainHelperForSpecs.NoDimension());
         _originalRefPh.Value = 4;
         _compound = new Compound {_originalRefPh};
         _compound.AddParameterAlternativeGroup(solubilityAlternativeGroup);

         A.CallTo(() => _executionContext.Clone(_compound)).Returns(_compoundForCalculation);
      }

      protected override void Because()
      {
         _tableFormula = sut.SolubilityTableForPh(_solubilityAlternative, _compound);
      }

      [Observation]
      public void should_return_a_table_formula_containing_a_solubility_for_each_half_ph()
      {
         _tableFormula.AllPoints().Count().ShouldBeEqualTo(14 * 2 + 1);
      }

      [Observation]
      public void the_value_at_ref_ph_should_be_equal_to_the_ref_sol_value()
      {
         _tableFormula.ValueAt(_refPhValue).ShouldBeEqualTo(_solValue);
      }

      [Observation]
      public void should_have_used_a_clone_of_the_given_compound_to_perform_solubility_calculation()
      {
         //ensure that value was not updated during calculation
         _originalRefPh.Value.ShouldBeEqualTo(4);
      }

      [Observation]
      [TestCase(0)]
      [TestCase(1)]
      [TestCase(1.5)]
      [TestCase(10)]
      [TestCase(12)]
      public void should_calculate_the_expected_values_according_to_the_sol_formula(double currentPh)
      {
         //formula is Sol(pH) = ref_Solubility * Solubility_Factor (ref_pH) / Solubility_Factor(pH) 
         _tableFormula.ValueAt(currentPh).ShouldBeEqualTo(_solValue * factorFor(_refPhValue) / factorFor(currentPh), 1e-7);
      }

      private double factorFor(double pH)
      {
         return 10 * (pH + 1);
      }
   }

   public class When_retrieving_the_table_formula_for_a_solubility_alternative_that_is_using_a_table_already : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternative _solubilityAlternative;
      private Compound _compound;
      private TableFormula _tableFormula;
      private IFormula _solubilityTableFormula;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         var solDim = new Dimension(new BaseDimensionRepresentation(), "Solubility", "m");
         solDim.AddUnit(new Unit("cm", 0.01, 0));
         _solubilityTableFormula = new TableFormula();
         var solubilityTable = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.SOLUBILITY_TABLE).WithDimension(solDim);
         solubilityTable.Formula = _solubilityTableFormula;
         _solubilityAlternative = new ParameterAlternative {solubilityTable};

         var solubilityAlternativeGroup = new ParameterAlternativeGroup();
         solubilityAlternativeGroup.AddAlternative(_solubilityAlternative);

         _compound.AddParameterAlternativeGroup(solubilityAlternativeGroup);
      }

      protected override void Because()
      {
         _tableFormula = sut.SolubilityTableForPh(_solubilityAlternative, _compound);
      }

      [Observation]
      public void should_return_the_table_formula_from_the_table_solubility()
      {
         _tableFormula.ShouldBeEqualTo(_solubilityTableFormula);
      }
   }

   public class When_setting_an_alternative_parameter_value_for_an_alternative_that_is_not_used_anywhere_in_a_simulation : concern_for_CompoundAlternativeTask
   {
      private IParameter _parameterInAlternative;
      private ParameterAlternative _alternative;

      protected override void Context()
      {
         base.Context();
         var lipoGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         _alternative = new ParameterAlternative().WithName("ALT1");
         _parameterInAlternative = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.LIPOPHILICITY);
         _alternative.Add(_parameterInAlternative);
         lipoGroup.AddAlternative(_alternative);
         var simulation = A.Fake<Simulation>();
         A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new[] {simulation});
      }

      protected override void Because()
      {
         sut.SetAlternativeParameterValue(_parameterInAlternative, 5);
      }

      [Observation]
      public void should_not_update_the_version_of_the_building_block()
      {
         A.CallTo(() => _parameterTask.SetParameterDisplayValueWithoutBuildingBlockChange(_parameterInAlternative, 5)).MustHaveHappened();
      }
   }

   public class When_setting_an_alternative_parameter_value_for_an_alternative_that_is_in_a_simulation : concern_for_CompoundAlternativeTask
   {
      private IParameter _parameterInAlternative;
      private ParameterAlternative _alternative;

      protected override void Context()
      {
         base.Context();
         var lipoGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         _alternative = new ParameterAlternative().WithName("ALT1");
         _parameterInAlternative = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.LIPOPHILICITY);
         _alternative.Add(_parameterInAlternative);
         lipoGroup.AddAlternative(_alternative);
         var simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new[] {simulation});

         var compoundProperties = new CompoundProperties();
         simulation.Properties.AddCompoundProperties(compoundProperties);
         compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = _alternative.Name, GroupName = lipoGroup.Name});
      }

      protected override void Because()
      {
         sut.SetAlternativeParameterValue(_parameterInAlternative, 5);
      }

      [Observation]
      public void should_not_update_the_version_of_the_building_block()
      {
         A.CallTo(() => _parameterTask.SetParameterDisplayValue(_parameterInAlternative, 5)).MustHaveHappened();
      }
   }

   public class When_updating_the_value_origin_of_a_compound_parameter_group_alternative_that_is_used_in_a_simulation : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternative _parameterAlternative;
      private ValueOrigin _newValueOrigin;
      private IParameter _parameter1;
      private IParameter _parameter2;
      private IParameter _parameter3;
      private PKSimMacroCommand _command;

      protected override void Context()
      {
         base.Context();

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("P1");
         _parameter1.IsDefault = false;
         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("P2");
         _parameter2.IsDefault = true;
         _parameter3 = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("P3");
         _parameter3.IsDefault = false;

         var alternativeGroup = new ParameterAlternativeGroup().WithName("Gr");
         _parameterAlternative = new ParameterAlternative {_parameter1, _parameter2, _parameter3};
         _parameterAlternative.Name = "Alt";
         alternativeGroup.AddAlternative(_parameterAlternative);

         _newValueOrigin = new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.InVivo,
            Source = ValueOriginSources.Database
         };

         var simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new[] {simulation});

         var compoundProperties = new CompoundProperties();
         simulation.Properties.AddCompoundProperties(compoundProperties);
         compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = _parameterAlternative.Name, GroupName = alternativeGroup.Name});
      }

      protected override void Because()
      {
         _command = sut.UpdateValueOrigin(_parameterAlternative, _newValueOrigin) as PKSimMacroCommand;
      }

      [Observation]
      public void should_update_the_value_origin_of_all_non_default_parameters_defined_in_the_alternative()
      {
         _parameter1.ValueOrigin.ShouldBeEqualTo(_newValueOrigin);
         _parameter2.ValueOrigin.ShouldNotBeEqualTo(_newValueOrigin);
         _parameter3.ValueOrigin.ShouldBeEqualTo(_newValueOrigin);
      }

      [Observation]
      public void should_update_the_building_block_version()
      {
         _command.All().OfType<BuildingBlockChangeCommand>().Each(x => x.ShouldChangeVersion.ShouldBeTrue());
      }
   }

   public class When_updating_the_value_origin_of_a_compound_parameter_group_alternative_that_is_not_used_in_a_simulation : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternative _parameterAlternative;
      private ValueOrigin _newValueOrigin;
      private IParameter _parameter1;
      private PKSimMacroCommand _command;

      protected override void Context()
      {
         base.Context();

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("P1");
         _parameter1.IsDefault = false;

         var alternativeGroup = new ParameterAlternativeGroup().WithName("Gr");
         _parameterAlternative = new ParameterAlternative {_parameter1};
         _parameterAlternative.Name = "Alt";
         alternativeGroup.AddAlternative(_parameterAlternative);

         _newValueOrigin = new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.InVivo,
            Source = ValueOriginSources.Database
         };

         var simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new[] {simulation});
      }

      protected override void Because()
      {
         _command = sut.UpdateValueOrigin(_parameterAlternative, _newValueOrigin) as PKSimMacroCommand;
      }

      [Observation]
      public void should_update_the_value_origin_of_all_non_default_parameters_defined_in_the_alternative()
      {
         _parameter1.ValueOrigin.ShouldBeEqualTo(_newValueOrigin);
      }

      [Observation]
      public void should_not_update_the_building_block_version()
      {
         _command.All().OfType<BuildingBlockChangeCommand>().Each(x => x.ShouldChangeVersion.ShouldBeFalse());
      }
   }

   public class When_importing_a_solubility_table_from_file : concern_for_CompoundAlternativeTask
   {
      private TableFormula _tableFormula;
      private DataRepository _solubilityDataRepository;

      protected override void Context()
      {
         base.Context();
         _solubilityDataRepository = DomainHelperForSpecs.ObservedData("SolubilityTable");
         A.CallTo(() => _formulaFactory.CreateTableFormula(A<bool>._)).Returns(new TableFormula {UseDerivedValues = true});
         A.CallTo(() => _dataImporter.ImportDataSets(A<IReadOnlyList<MetaDataCategory>>.Ignored, A<IReadOnlyList<ColumnInfo>>.Ignored, A<DataImporterSettings>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns((new List<DataRepository>() { _solubilityDataRepository }, null));
      }

      protected override void Because()
      {
         _tableFormula = sut.ImportSolubilityTableFormula();
      }

      [Observation]
      public void should_return_a_solubility_table_that_is_set_to_not_use_derived_values()
      {
         _tableFormula.UseDerivedValues.ShouldBeFalse();
      }
   }

   public class When_setting_the_parameter_alternative_table_for_a_solubility_parameter_used_in_a_simmulation : concern_for_CompoundAlternativeTask
   {
      private IParameter _parameter;
      private ICommand _result;
      private TableFormula _editedTableFormula;
      private ICommand _updateTableFormulaCommand;
      private ParameterAlternative _alternative;

      protected override void Context()
      {
         base.Context();
         _updateTableFormulaCommand = A.Fake<ICommand>();
         _editedTableFormula = new TableFormula();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue().WithName("SOL");


         A.CallTo(() => _parameterTask.UpdateTableFormula(_parameter, _editedTableFormula)).Returns(_updateTableFormulaCommand);


         var solGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_SOLUBILITY);
         _alternative = new ParameterAlternative().WithName("ALT1");
         _alternative.Add(_parameter);
         solGroup.AddAlternative(_alternative);
         var simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new[] {simulation});

         var compoundProperties = new CompoundProperties();
         simulation.Properties.AddCompoundProperties(compoundProperties);
         compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = _alternative.Name, GroupName = solGroup.Name});
      }

      protected override void Because()
      {
         _result = sut.SetAlternativeParameterTable(_parameter, _editedTableFormula);
      }

      [Observation]
      public void should_update_the_table_formula_and_return_the_edited_command()
      {
         _result.ShouldBeEqualTo(_updateTableFormulaCommand);
      }
   }

   public class When_an_alternative_for_solubility_is_being_created_as_table : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternativeGroup _solubilityGroup;
      private ParameterAlternative _newTableAlternative;
      private IParameter _solubilityRefPh;
      private IParameter _solubilityTable;
      private IParameter _refPhParameter;
      private TableFormula _solubilityTableFormula;
      private IParameter _solubilityGainParameter;

      protected override void Context()
      {
         base.Context();
         _solubilityGroup = new ParameterAlternativeGroup {Name = CoreConstants.Groups.COMPOUND_SOLUBILITY};
         _refPhParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.REFERENCE_PH);
         _solubilityRefPh = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH);
         _solubilityTable = DomainHelperForSpecs.ConstantParameterWithValue(20).WithName(CoreConstants.Parameters.SOLUBILITY_TABLE);
         _solubilityGainParameter = DomainHelperForSpecs.ConstantParameterWithValue(20).WithName(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE);

         var compound = new Compound();

         _newTableAlternative = new ParameterAlternative {_solubilityRefPh, _solubilityTable, _refPhParameter, _solubilityGainParameter};
         A.CallTo(() => _parameterAlternativeFactory.CreateTableAlternativeFor(_solubilityGroup, CoreConstants.Parameters.SOLUBILITY_TABLE)).Returns(_newTableAlternative);

         compound.Add(_solubilityGroup);

         _solubilityTableFormula = new TableFormula();
         _solubilityTable.Formula = _solubilityTableFormula;
      }

      protected override void Because()
      {
         sut.CreateSolubilityTableAlternativeFor(_solubilityGroup, "New Alternative");
      }


      [Observation]
      public void should_set_the_value_of_the_solubility_parameter_to_zero()
      {
         _solubilityRefPh.Value.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_hide_the_solubility_parameters()
      {
         _solubilityRefPh.Visible.ShouldBeFalse();
         _refPhParameter.Visible.ShouldBeFalse();
         _solubilityGainParameter.Visible.ShouldBeFalse();
      }

      [Observation]
      public void should_show_the_solubility_table_parameter()
      {
         _solubilityTable.Visible.ShouldBeTrue();
      }

      [Observation]
      public void should_have_initialized_the_solubility_table_formula()
      {
         _solubilityTableFormula.Name.ShouldBeEqualTo(PKSimConstants.UI.Solubility);
         _solubilityTableFormula.XName.ShouldBeEqualTo(PKSimConstants.UI.pH);
         _solubilityTableFormula.YName.ShouldBeEqualTo(PKSimConstants.UI.Solubility);
      }
   }
}