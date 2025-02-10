using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Parameters;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using IEditTableParameterPresenter = PKSim.Presentation.Presenters.Parameters.IEditTableParameterPresenter;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditParameterPresenterTask : ContextSpecification<IEditParameterPresenterTask>
   {
      protected IParameterTask _parameterTask;
      protected IParameter _parameter;
      protected ParameterDTO _parameterDTO;
      protected IEditParameterPresenter _presenter;
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _parameter = new PKSimParameter();
         _parameterTask = A.Fake<IParameterTask>();
         _parameterDTO = new ParameterDTO(_parameter);
         _presenter = A.Fake<IEditParameterPresenter>();
         _applicationController = A.Fake<IApplicationController>();
         sut = new EditParameterPresenterTask(_parameterTask, _applicationController);
      }
   }

   public class When_setting_a_percentile_for_a_parameter_dto : concern_for_EditParameterPresenterTask
   {
      private double _percentileInPercent;
      private IPKSimCommand _parameterPercentileSetCommand;
      private double _percentileInFraction;

      protected override void Context()
      {
         base.Context();
         _percentileInPercent = 50;
         _percentileInFraction = _percentileInPercent / 100;
         _parameterPercentileSetCommand = A.Fake<IPKSimCommand>();
         A.CallTo(() => _parameterTask.SetParameterPercentile(_parameter, _percentileInFraction)).Returns(_parameterPercentileSetCommand);
      }

      protected override void Because()
      {
         sut.SetParameterPercentile(_presenter, _parameterDTO, _percentileInPercent);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_set_the_percentile_into_the_parameter()
      {
         A.CallTo(() => _parameterTask.SetParameterPercentile(_parameter, _percentileInFraction)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_resulting_command_into_the_command_register()
      {
         A.CallTo(() => _presenter.AddCommand(_parameterPercentileSetCommand)).MustHaveHappened();
      }
   }

   public class When_editing_the_table_for_a_parameter_using_a_table_formula : concern_for_EditParameterPresenterTask
   {
      private ICommand _editCommand;
      private TableFormula _tableFormula;
      private IEditTableParameterPresenter _editTableFormulaPresenter;

      protected override void Context()
      {
         base.Context();
         _editCommand= A.Fake<ICommand>();
         _editTableFormulaPresenter= A.Fake<IEditTableParameterPresenter>();
         _tableFormula = new TableFormula();
         A.CallTo(() => _applicationController.Start<IEditTableParameterPresenter>()).Returns(_editTableFormulaPresenter);
         A.CallTo(() => _parameterTask.UpdateTableFormula(_parameter, _tableFormula)).Returns(_editCommand);

         A.CallTo(() => _editTableFormulaPresenter.Edit(_parameter)).Returns(true);
         A.CallTo(() => _editTableFormulaPresenter.EditedFormula).Returns(_tableFormula);
      }
      protected override void Because()
      {
         sut.EditTableFor(_presenter, _parameterDTO);
      }

      [Observation]
      public void should_start_the_edit_table_formula_presenter_and_edit_the_formula_of_the_underlying_parameter()
      {
         A.CallTo(() => _editTableFormulaPresenter.Edit(_parameter)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_edited_parameter_with_the_table_formula_and_add_the_command_into_the_register()
      {
         A.CallTo(() => _presenter.AddCommand(_editCommand)).MustHaveHappened();
      }
   }

   public class When_editing_the_table_for_a_parameter_using_a_table_formula_and_the_user_cancels_the_edit : concern_for_EditParameterPresenterTask
   {
      private IEditTableParameterPresenter _editTableFormulaPresenter;

      protected override void Context()
      {
         base.Context();
         _editTableFormulaPresenter = A.Fake<IEditTableParameterPresenter>();
         A.CallTo(() => _applicationController.Start<IEditTableParameterPresenter>()).Returns(_editTableFormulaPresenter);

         A.CallTo(() => _editTableFormulaPresenter.Edit(_parameter)).Returns(false);
      }
      protected override void Because()
      {
         sut.EditTableFor(_presenter, _parameterDTO);
      }

      [Observation]
      public void should_start_the_edit_table_formula_presenter_and_edit_the_formula_of_the_underlying_parameter()
      {
         A.CallTo(() => _editTableFormulaPresenter.Edit(_parameter)).MustHaveHappened();
      }

      [Observation]
      public void should_not_update_the_parameter_formula()
      {
         A.CallTo(() => _parameterTask.UpdateTableFormula(_parameter, A<TableFormula>._)).MustNotHaveHappened();
      }
   }

}