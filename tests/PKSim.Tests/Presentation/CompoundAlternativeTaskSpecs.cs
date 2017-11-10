using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundAlternativeTask : ContextSpecification<ICompoundAlternativeTask>
   {
      protected IParameterAlternativeFactory _parameterAlternativeFactory;
      protected IApplicationController _applicationController;
      private IExecutionContext _executionContext;
      protected ICompoundFactory _compoundFactory;
      protected IEntityTask _entityTask;
      protected IFormulaFactory _formulaFactory;
      protected IParameterTask _parameterTask;
      protected IBuildingBlockRepository _buildingBlockRepository;

      protected override void Context()
      {
         _parameterAlternativeFactory = A.Fake<IParameterAlternativeFactory>();
         _applicationController = A.Fake<IApplicationController>();
         _executionContext = A.Fake<IExecutionContext>();
         _entityTask = A.Fake<IEntityTask>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         _parameterTask = A.Fake<IParameterTask>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _compoundFactory = new CompoundFactoryForSpecs();
         sut = new CompoundAlternativeTask(_parameterAlternativeFactory, _applicationController,
            _executionContext, _compoundFactory, _entityTask, _formulaFactory, _parameterTask, _buildingBlockRepository);
      }

      protected class CompoundFactoryForSpecs : ICompoundFactory
      {
         public Compound Create()
         {
            var compound = new Compound();
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(CoreConstants.Parameter.EFFECTIVE_MOLECULAR_WEIGHT));
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(CoreConstants.Parameter.LIPOPHILICITY));
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(8).WithName(CoreConstants.Parameter.Permeability));
            compound.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.IS_SMALL_MOLECULE));
            return compound;
         }
      }
   }

   public class When_the_compound_alternative_task_is_adding_an_alternative_to_a_group : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternativeGroup _group;
      private IParameterAlternativeNamePresenter _parameterAlternativePresenter;
      private ParameterAlternative _newAlternative;

      protected override void Context()
      {
         base.Context();
         _group = new ParameterAlternativeGroup();
         _newAlternative = new ParameterAlternative();
         A.CallTo(() => _parameterAlternativeFactory.CreateAlternativeFor(_group)).Returns(_newAlternative);
         _parameterAlternativePresenter = A.Fake<IParameterAlternativeNamePresenter>();
         A.CallTo(() => _parameterAlternativePresenter.Edit(_group)).Returns(true);
         A.CallTo(() => _parameterAlternativePresenter.Name).Returns("new name");
         A.CallTo(() => _parameterAlternativePresenter.Description).Returns("new description");
         A.CallTo(() => _applicationController.Start<IParameterAlternativeNamePresenter>()).Returns(_parameterAlternativePresenter);
      }

      protected override void Because()
      {
         sut.AddParameterGroupAlternativeTo(_group);
      }

      [Observation]
      public void should_set_the_name_and_the_description_of_the_new_alternative_according_to_the_value_entered_by_the_user()
      {
         _newAlternative.Name.ShouldBeEqualTo("new name");
         _newAlternative.Description.ShouldBeEqualTo("new description");
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
         _alternative1.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.LIPOPHILICITY));
         lipoGroup.AddAlternative(_alternative1);

         _alternative2 = new ParameterAlternative().WithName("ALT2");
         _alternative2.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(CoreConstants.Parameter.LIPOPHILICITY));
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

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         var solDim = new Dimension(new BaseDimensionRepresentation(), "Solubility", "m");
         solDim.AddUnit(new Unit("cm", 0.01, 0));
         _refPhValue = 7;
         _solValue = 100;
         _gainPerChargeValue = 1000;
         A.CallTo(() => _formulaFactory.CreateTableFormula()).Returns(new TableFormula());
         var refPh = DomainHelperForSpecs.ConstantParameterWithValue(_refPhValue).WithName(CoreConstants.Parameter.REFERENCE_PH).WithDimension(DomainHelperForSpecs.NoDimension());
         var solubilty = DomainHelperForSpecs.ConstantParameterWithValue(_solValue).WithName(CoreConstants.Parameter.SOLUBILITY_AT_REFERENCE_PH).WithDimension(solDim);
         var gainPerCharge = DomainHelperForSpecs.ConstantParameterWithValue(_gainPerChargeValue).WithName(CoreConstants.Parameter.SolubilityGainPerCharge).WithDimension(DomainHelperForSpecs.NoDimension());
         _solubility_pKa_pH_Factor = new PKSimParameter().WithName(CoreConstants.Parameter.SOLUBILITY_P_KA__P_H_FACTOR);
         _solubility_pKa_pH_Factor.Formula = new ExplicitFormula("10 * (pH +1)");
         _solubility_pKa_pH_Factor.Formula.AddObjectPath(new FormulaUsablePath(new[] {ObjectPath.PARENT_CONTAINER, CoreConstants.Parameter.REFERENCE_PH}).WithAlias("pH"));
         _compound.Add(refPh);
         _compound.Add(solubilty);
         _compound.Add(gainPerCharge);
         _compound.Add(_solubility_pKa_pH_Factor);
         _solubilityAlternative = new ParameterAlternative();
         _solubilityAlternative.Add(refPh);
         _solubilityAlternative.Add(solubilty);
         _solubilityAlternative.Add(gainPerCharge);

         var solubilityAlternativeGroup = new ParameterAlternativeGroup();
         solubilityAlternativeGroup.AddAlternative(_solubilityAlternative);

         _compound.AddParameterAlternativeGroup(solubilityAlternativeGroup);
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
      public void should_leverage_the_formula_factory_to_create_a_new_table_formula()
      {
         A.CallTo(() => _formulaFactory.CreateTableFormula()).MustHaveHappened();
      }

      [Observation]
      public void the_value_at_ref_ph_should_be_equal_to_the_ref_sol_value()
      {
         _tableFormula.ValueAt(_refPhValue).ShouldBeEqualTo(_solValue);
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

   public class When_setting_an_alternative_parameter_value_for_an_alternative_that_is_not_used_anywhere_in_a_simulation : concern_for_CompoundAlternativeTask
   {
      private IParameter _parameterInAlternative;
      private ParameterAlternative _alternative;

      protected override void Context()
      {
         base.Context();
         var lipoGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         _alternative = new ParameterAlternative().WithName("ALT1");
         _parameterInAlternative = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.LIPOPHILICITY);
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
         _parameterInAlternative = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.LIPOPHILICITY);
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

   public class When_renaming_a_parameter_alternative : concern_for_CompoundAlternativeTask
   {
      private ParameterAlternative _alternative;

      protected override void Context()
      {
         base.Context();
         _alternative = A.Fake<ParameterAlternative>();
      }

      protected override void Because()
      {
         sut.RenameParameterAlternative(_alternative);
      }

      [Observation]
      public void should_induce_a_structurel_change()
      {
         A.CallTo(() => _entityTask.StructuralRename(_alternative)).MustHaveHappened();
      }
   }
}