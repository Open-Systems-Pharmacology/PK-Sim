using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_TableParameterPresenter : ContextSpecification<ITableParameterPresenter>
   {
      protected ITableParameterView _view;
      protected IParameterTask _parameterTask;
      private IFormulaFactory _formulaFactory;
      protected IParameter _parameter;
      protected TableFormula _tableFormula;
      private ICloner _cloner;
      protected TableFormula _editedFormula;

      protected override void Context()
      {
         _view = A.Fake<ITableParameterView>();
         _parameterTask = A.Fake<IParameterTask>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         _cloner = A.Fake<ICloner>();
         _tableFormula = new TableFormula {Id = "1"};
         _tableFormula.XDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _tableFormula.XDisplayUnit = _tableFormula.XDimension.BaseUnit;
         _tableFormula.Dimension = DomainHelperForSpecs.LengthDimensionForSpecs();
         _tableFormula.YDisplayUnit = _tableFormula.Dimension.BaseUnit;
         _editedFormula = new TableFormula {Id = "2"};
         _editedFormula.XDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _editedFormula.XDisplayUnit = _editedFormula.XDimension.BaseUnit;
         _editedFormula.Dimension = DomainHelperForSpecs.LengthDimensionForSpecs();
         _editedFormula.YDisplayUnit = _editedFormula.Dimension.BaseUnit;

         A.CallTo(() => _cloner.Clone(_tableFormula)).Returns(_editedFormula);
         sut = new TableParametersForSpecs(_view, _parameterTask, _cloner, _formulaFactory);
         sut.InitializeWith(A.Fake<ICommandCollector>());
         _parameter = new PKSimParameter().WithFormula(_tableFormula);
      }

      private class TableParametersForSpecs : TableParameterPresenter<ITableParameterView>
      {
         public TableParametersForSpecs(ITableParameterView view, IParameterTask parameterTask, ICloner cloneManager, IFormulaFactory formulaFactory) :
            base(view, parameterTask, formulaFactory, cloneManager, () => new TableFormula {Id = "new"})

         {
         }
      }
   }

   public class When_editing_a_parameter_containg_a_table_formula : concern_for_TableParameterPresenter
   {
      private ValuePoint _p1;
      private ValuePoint _p2;

      protected override void Context()
      {
         base.Context();
         _p1 = new ValuePoint(1, 2);
         _p2 = new ValuePoint(3, 4);
         _tableFormula.AddPoint(_p1);
         _tableFormula.AddPoint(_p2);
         _editedFormula.AddPoint(_p1);
         _editedFormula.AddPoint(_p2);
      }

      protected override void Because()
      {
         sut.Edit(_parameter);
      }

      [Observation]
      public void should_allow_the_user_to_edit_the_points_defined_in_the_formula()
      {
         var allPoints = sut.AllPoints();
         A.CallTo(() => _view.BindTo(allPoints)).MustHaveHappened();
         allPoints.Count().ShouldBeEqualTo(2);
         allPoints.ElementAt(0).X.ShouldBeEqualTo(_p1.X);
         allPoints.ElementAt(0).Y.ShouldBeEqualTo(_p1.Y);
         allPoints.ElementAt(1).X.ShouldBeEqualTo(_p2.X);
         allPoints.ElementAt(1).Y.ShouldBeEqualTo(_p2.Y);
      }
   }

   public class When_removing_a_parameter_from_a_table_formula_and_saving_the_changes : concern_for_TableParameterPresenter
   {
      private ValuePoint _p1;
      private ValuePoint _p2;

      protected override void Context()
      {
         base.Context();
         _p1 = new ValuePoint(1, 2);
         _p2 = new ValuePoint(3, 4);
         _tableFormula.AddPoint(_p1);
         _editedFormula.AddPoint(_p1);
         _tableFormula.AddPoint(_p2);
         _editedFormula.AddPoint(_p2);
         sut.Edit(_parameter);
      }

      protected override void Because()
      {
         sut.RemovePoint(sut.AllPoints().Last());
         sut.Save();
      }

      [Observation]
      public void should_allow_the_user_to_edit_the_points_defined_in_the_formula()
      {
         _editedFormula.AllPoints().Count().ShouldBeEqualTo(1);
      }
   }

   public class When_adding_a_parameter_to_a_table_formula_and_saving_the_changes : concern_for_TableParameterPresenter
   {
      private ValuePoint _p1;
      private ValuePoint _p2;

      protected override void Context()
      {
         base.Context();
         _p1 = new ValuePoint(1, 2);
         _p2 = new ValuePoint(3, 4);
         _tableFormula.AddPoint(_p1);
         _editedFormula.AddPoint(_p1);
         _tableFormula.AddPoint(_p2);
         _editedFormula.AddPoint(_p2);
         sut.Edit(_parameter);
      }

      protected override void Because()
      {
         sut.AddPoint();
         sut.Save();
      }

      [Observation]
      public void should_allow_the_user_to_edit_the_points_defined_in_the_formula()
      {
         _editedFormula.AllPoints().Count().ShouldBeEqualTo(3);
      }
   }

   public class When_checking_if_an_edited_table_paramaeter_is_valid : concern_for_TableParameterPresenter
   {

      protected override void Context()
      {
         base.Context();
         sut.Edit(_parameter);
      }

      [Observation]
      public void should_return_false_if_the_edited_parameter_formula_has_no_points()
      {
         sut.CanClose.ShouldBeFalse();
      }
   }

   public class When_told_to_import_a_table : concern_for_TableParameterPresenter
   {
      private TableFormula _formula;

      protected override void Context()
      {
         base.Context();
         sut.Edit(_parameter);
         A.CallTo(() => _parameterTask.SetParameterFormula(A<IParameter>.Ignored, A<IFormula>.Ignored))
            .Invokes(x => _formula = x.GetArgument<TableFormula>(1));
      }

      protected override void Because()
      {
         sut.ImportTable();
         sut.Save();
      }

      [Observation]
      public void the_table_parameter_presenter_should_use_the_provided_function_to_import_the_table()
      {
         _formula.Id.ShouldBeEqualTo("new");
      }
   }

   public class When_editing_a_parameter_that_is_not_editable : concern_for_TableParameterPresenter
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Editable = false;
         sut.Edit(_parameter);
      }

      [Observation]
      public void should_set_the_view_as_readonly()
      {
         _view.Editable.ShouldBeFalse();
      }
   }
}