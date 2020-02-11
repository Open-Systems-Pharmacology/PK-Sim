using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundAlternativePresentationTask : ContextSpecification<ICompoundAlternativePresentationTask>
   {
      protected ICompoundAlternativeTask _compoundAlternativeTask;
      protected IApplicationController _applicationController;
      protected IEntityTask _entityTask;

      protected override void Context()
      {
         _compoundAlternativeTask = A.Fake<ICompoundAlternativeTask>();
         _applicationController = A.Fake<IApplicationController>();
         _entityTask = A.Fake<IEntityTask>();
         sut = new CompoundAlternativePresentationTask(_compoundAlternativeTask, _applicationController, _entityTask);
      }
   }

   public class When_renaming_a_parameter_alternative : concern_for_CompoundAlternativePresentationTask
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
      public void should_induce_a_structural_change()
      {
         A.CallTo(() => _entityTask.StructuralRename(_alternative)).MustHaveHappened();
      }
   }

   public class When_the_compound_alternative_task_is_adding_an_alternative_to_a_group : concern_for_CompoundAlternativePresentationTask
   {
      private ParameterAlternativeGroup _group;
      private IParameterAlternativeNamePresenter _parameterAlternativePresenter;
      private ParameterAlternative _newAlternative;

      protected override void Context()
      {
         base.Context();
         _group = new ParameterAlternativeGroup();
         _newAlternative = new ParameterAlternative();
         A.CallTo(() => _compoundAlternativeTask.CreateAlternative(_group, "new name")).Returns(_newAlternative);
         _parameterAlternativePresenter = A.Fake<IParameterAlternativeNamePresenter>();
         A.CallTo(() => _parameterAlternativePresenter.Edit(_group)).Returns(true);
         A.CallTo(() => _parameterAlternativePresenter.Name).Returns("new name");
         A.CallTo(() => _applicationController.Start<IParameterAlternativeNamePresenter>()).Returns(_parameterAlternativePresenter);
      }

      protected override void Because()
      {
         sut.AddParameterGroupAlternativeTo(_group);
      }

      [Observation]
      public void should_add_the_alternative_with_the_name_to_the_group()
      {
         A.CallTo(() => _compoundAlternativeTask.AddParameterGroupAlternativeTo(_group, _newAlternative)).MustHaveHappened();
      }
   }

   public class When_an_alternative_for_solubility_is_being_added_and_the_user_decides_to_create_a_normal_solubility : concern_for_CompoundAlternativePresentationTask
   {
      private ParameterAlternativeGroup _solubilityGroup;
      private ISolubilityAlternativeNamePresenter _solubilityAlternativeNameParameter;
      private ParameterAlternative _newAlternative;

      protected override void Context()
      {
         base.Context();
         _solubilityAlternativeNameParameter = A.Fake<ISolubilityAlternativeNamePresenter>();
         _solubilityGroup = new ParameterAlternativeGroup {Name = CoreConstants.Groups.COMPOUND_SOLUBILITY};
         A.CallTo(() => _applicationController.Start<ISolubilityAlternativeNamePresenter>()).Returns(_solubilityAlternativeNameParameter);
         A.CallTo(() => _solubilityAlternativeNameParameter.Edit(_solubilityGroup)).Returns(true);
         A.CallTo(() => _solubilityAlternativeNameParameter.Name).Returns("new name");
         _solubilityAlternativeNameParameter.CreateAsTable = false;
         _newAlternative = new ParameterAlternative();
         A.CallTo(() => _compoundAlternativeTask.CreateAlternative(_solubilityGroup, "new name")).Returns(_newAlternative);
      }

      protected override void Because()
      {
         sut.AddParameterGroupAlternativeTo(_solubilityGroup);
      }

      [Observation]
      public void the_user_should_be_presented_with_the_option_to_add_a_table_alternative()
      {
         A.CallTo(() => _applicationController.Start<ISolubilityAlternativeNamePresenter>()).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_new_alternative_to_the_compound()
      {
         A.CallTo(() => _compoundAlternativeTask.CreateAlternative(_solubilityGroup, "new name")).MustHaveHappened();
      }
   }

   public class When_an_alternative_for_solubility_is_being_added_and_the_user_decides_to_create_a_table_solubility : concern_for_CompoundAlternativePresentationTask
   {
      private ParameterAlternativeGroup _solubilityGroup;
      private ISolubilityAlternativeNamePresenter _solubilityAlternativeNameParameter;
      private ParameterAlternative _newTableAlternative;
      private TableFormula _solubilityTableFormula;

      protected override void Context()
      {
         base.Context();
         _solubilityGroup = new ParameterAlternativeGroup {Name = CoreConstants.Groups.COMPOUND_SOLUBILITY};

         var compound = new Compound();
         _solubilityAlternativeNameParameter = A.Fake<ISolubilityAlternativeNamePresenter>();
         A.CallTo(() => _applicationController.Start<ISolubilityAlternativeNamePresenter>()).Returns(_solubilityAlternativeNameParameter);
         A.CallTo(() => _solubilityAlternativeNameParameter.Edit(_solubilityGroup)).Returns(true);
         A.CallTo(() => _solubilityAlternativeNameParameter.Name).Returns("new name");
         _solubilityAlternativeNameParameter.CreateAsTable = true;

         _newTableAlternative = new ParameterAlternative();

         A.CallTo(() => _compoundAlternativeTask.CreateSolubilityTableAlternativeFor(_solubilityGroup, "new name")).Returns(_newTableAlternative);

         compound.Add(_solubilityGroup);

         _solubilityTableFormula = new TableFormula();
      }

      protected override void Because()
      {
         sut.AddParameterGroupAlternativeTo(_solubilityGroup);
      }

      [Observation]
      public void the_user_should_be_presented_with_the_option_to_add_a_table_alternative()
      {
         A.CallTo(() => _applicationController.Start<ISolubilityAlternativeNamePresenter>()).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_alternative_to_the_solubility()
      {
         A.CallTo(() => _compoundAlternativeTask.AddParameterGroupAlternativeTo(_solubilityGroup, _newTableAlternative)).MustHaveHappened();
      }
   }

   public class When_editing_the_solubility_table_for_a_solubility_parameter_for_an_alternative_that_is_not_used_anywhere_in_a_simulation : concern_for_CompoundAlternativePresentationTask
   {
      private IParameter _parameter;
      private ICommand _result;
      private IEditTableSolubilityParameterPresenter _editSolubilityParameterPresenter;
      private TableFormula _editedTableFormula;
      private ICommand _updateTableFormulaCommand;

      protected override void Context()
      {
         base.Context();
         _updateTableFormulaCommand = A.Fake<ICommand>();
         _editedTableFormula = new TableFormula();
         _editSolubilityParameterPresenter = A.Fake<IEditTableSolubilityParameterPresenter>();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue();
         A.CallTo(() => _applicationController.Start<IEditTableSolubilityParameterPresenter>()).Returns(_editSolubilityParameterPresenter);

         A.CallTo(() => _editSolubilityParameterPresenter.Edit(_parameter)).Returns(true);
         A.CallTo(() => _editSolubilityParameterPresenter.EditedFormula).Returns(_editedTableFormula);

         A.CallTo(() => _compoundAlternativeTask.SetAlternativeParameterTable(_parameter, _editedTableFormula)).Returns(_updateTableFormulaCommand);
      }

      protected override void Because()
      {
         _result = sut.EditSolubilityTableFor(_parameter);
      }

      [Observation]
      public void should_retrieve_the_edit_solubility_parameter_table()
      {
         A.CallTo(() => _editSolubilityParameterPresenter.Edit(_parameter)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_table_formula_and_return_the_edited_command()
      {
         _result.ShouldBeEqualTo(_updateTableFormulaCommand);
      }
   }

   public class When_editing_the_solubility_table_for_a_solubility_parameter_for_an_alternative_that_is_used_in_a_simulation : concern_for_CompoundAlternativePresentationTask
   {
      private IParameter _parameter;
      private ICommand _result;
      private IEditTableSolubilityParameterPresenter _editSolubilityParameterPresenter;
      private TableFormula _editedTableFormula;
      private ICommand _updateTableFormulaCommand;

      protected override void Context()
      {
         base.Context();
         _updateTableFormulaCommand = A.Fake<ICommand>();
         _editedTableFormula = new TableFormula();
         _editSolubilityParameterPresenter = A.Fake<IEditTableSolubilityParameterPresenter>();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue().WithName("SOL");
         A.CallTo(() => _applicationController.Start<IEditTableSolubilityParameterPresenter>()).Returns(_editSolubilityParameterPresenter);

         A.CallTo(() => _editSolubilityParameterPresenter.Edit(_parameter)).Returns(true);
         A.CallTo(() => _editSolubilityParameterPresenter.EditedFormula).Returns(_editedTableFormula);

         A.CallTo(() => _compoundAlternativeTask.SetAlternativeParameterTable(_parameter, _editedTableFormula)).Returns(_updateTableFormulaCommand);
      }

      protected override void Because()
      {
         _result = sut.EditSolubilityTableFor(_parameter);
      }

      [Observation]
      public void should_retrieve_the_edit_solubility_parameter_table()
      {
         A.CallTo(() => _editSolubilityParameterPresenter.Edit(_parameter)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_table_formula_and_return_the_edited_command()
      {
         _result.ShouldBeEqualTo(_updateTableFormulaCommand);
      }
   }
}