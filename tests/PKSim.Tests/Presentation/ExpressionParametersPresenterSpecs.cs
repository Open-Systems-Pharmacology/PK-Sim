using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionParametersPresenter : ContextSpecification<IExpressionParametersPresenter>
   {
      protected IExpressionParametersView<ExpressionParameterDTO> _view;
      protected IEditParameterPresenterTask _editParameterTask;
      protected ExpressionParameterDTO _initialConcentration;
      protected ExpressionParameterDTO _relativeExpression;
      protected ExpressionParameterDTO _relativeExpression2;
      protected ExpressionParameterDTO _fraction_exp_bc;

      protected override void Context()
      {
         _view= A.Fake<IExpressionParametersView<ExpressionParameterDTO>>();
         _editParameterTask= A.Fake<IEditParameterPresenterTask>();  
         sut = new ExpressionParametersPresenter(_view,_editParameterTask);


         _initialConcentration = createParameter(CoreConstants.Parameters.INITIAL_CONCENTRATION);
         _relativeExpression = createParameter(CoreConstants.Parameters.REL_EXP);
         _relativeExpression2 = createParameter(CoreConstants.Parameters.REL_EXP);
         _fraction_exp_bc = createParameter(CoreConstants.Parameters.FRACTION_EXPRESSED_BLOOD_CELLS);
     
      }

      private ExpressionParameterDTO createParameter(string parameterName)
      {
         return new ExpressionParameterDTO
            { Parameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(parameterName)) };
      }
   }

   public class When_setting_the_value_of_an_expression_parameter : concern_for_ExpressionParametersPresenter
   {
      protected override void Context()
      {
         base.Context();

         _relativeExpression.Parameter.Parameter.Value = 5;
         _relativeExpression2.Parameter.Parameter.Value = 10;

         sut.Edit(new []{_fraction_exp_bc, _initialConcentration, _relativeExpression, _relativeExpression2, });

         //Make sure that we in fact update the value of the parameter as the command will not be executed
         A.CallTo(() => _editParameterTask.SetParameterValue(sut, _relativeExpression.Parameter, 20))
            .Invokes(x => _relativeExpression.Parameter.Parameter.Value = 20);
      }

      protected override void Because()
      {
         sut.SetExpressionParameterValue(_relativeExpression.Parameter, 20);
      }

      [Observation]
      public void should_also_calculate_the_normalized_value()
      {
         _relativeExpression.NormalizedExpressionPercent.ShouldBeEqualTo(100);
         _relativeExpression2.NormalizedExpressionPercent.ShouldBeEqualTo(50);
      }
   }


   public class When_switching_the_visibility_of_initial_concentration_parameters_off : concern_for_ExpressionParametersPresenter
   {
      private List<ExpressionParameterDTO> _allExpressionParameterDTO;

      protected override void Context()
      {
         base.Context();
         sut.Edit(new[] { _fraction_exp_bc, _initialConcentration, _relativeExpression, _relativeExpression2, });

         A.CallTo(() => _view.BindTo(A<IEnumerable<ExpressionParameterDTO>>._))
            .Invokes(x => _allExpressionParameterDTO = x.GetArgument<IEnumerable<ExpressionParameterDTO>>(0).ToList());

      }

      protected override void Because()
      {
         sut.ShowInitialConcentration = false;
      }

      [Observation]
      public void should_hide_concentration_parameters()
      {
         _allExpressionParameterDTO.Contains(_initialConcentration).ShouldBeFalse();
      }
   }

   public class When_switching_the_visibility_of_initial_concentration_parameters_on : concern_for_ExpressionParametersPresenter
   {
      private List<ExpressionParameterDTO> _allExpressionParameterDTO;
      protected override void Context()
      {
         base.Context();
         sut.Edit(new[] { _fraction_exp_bc, _initialConcentration, _relativeExpression, _relativeExpression2, });

         A.CallTo(() => _view.BindTo(A<IEnumerable<ExpressionParameterDTO>>._))
            .Invokes(x => _allExpressionParameterDTO = x.GetArgument<IEnumerable<ExpressionParameterDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.ShowInitialConcentration = true;
      }

      [Observation]
      public void should_show_concentration_parameters()
      {
         _allExpressionParameterDTO.Contains(_initialConcentration).ShouldBeTrue();
      }
   }

}