using System.Collections.Generic;
using System.Linq;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;


using PKSim.Presentation.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterSetPresenter : ContextSpecification<IParameterSetPresenter>
   {
      private IView<IParameterSetPresenter> _view;
      private IEditParameterPresenterTask _task;
      private IParameterTask _parameterTask;

      protected override void Context()
      {
         _view  = A.Fake<IView<IParameterSetPresenter>>();
         _task  = A.Fake<IEditParameterPresenterTask>();
         _parameterTask = A.Fake<IParameterTask>();
         sut = new ParameterSetPresenterForSpecs(_view,_task,_parameterTask);
      }

      protected class ParameterSetPresenterForSpecs : ParameterSetPresenter<IView<IParameterSetPresenter>, IParameterSetPresenter>
      {
         public ParameterSetPresenterForSpecs(IView<IParameterSetPresenter> view, IEditParameterPresenterTask editParameterTask, IParameterTask parameterTask) : base(view, editParameterTask, parameterTask)
         {
         }

         public override bool ShowFavorites
         {
            set { ; }
         }

         protected override IEnumerable<IParameterDTO> AllVisibleParameterDTOs => Enumerable.Empty<IParameterDTO>();

         public override void Edit(IEnumerable<IParameter> parameters)
         {
           
         }
      }
   }

   
   public class When_checking_if_a_parameter_was_set_by_the_user : concern_for_ParameterSetPresenter
   {
      private IParameter _formulaParameter;
      private IParameter _constantParameter;
      private IParameter _distributedParameter;

      protected override void Context()
      {
         base.Context();
         _formulaParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _formulaParameter.Formula = new ExplicitFormula("10");
         _constantParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _distributedParameter = DomainHelperForSpecs.NormalDistributedParameter();

      }
      [Observation]
      public void should_return_true_if_the_parameter_is_a_formula_parameter_whose_value_was_fixed()
      {
         _formulaParameter.Value = 5;
         var parameterDTO = new ParameterDTO(_formulaParameter);
         sut.IsSetByUser(parameterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_a_formula_parameter_whose_value_was_not_fixed()
      {
         var parameterDTO = new ParameterDTO(_formulaParameter);
         _formulaParameter.IsFixedValue = false;
         sut.IsSetByUser(parameterDTO).ShouldBeFalse();
      }


      [Observation]
      public void should_return_true_if_the_parameter_is_a_constant_parameter_whose_value_differs_from_the_default_value()
      {
         var parameterDTO = new ParameterDTO(_constantParameter);
         _constantParameter.DefaultValue = _constantParameter.Value - 1;
         sut.IsSetByUser(parameterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_a_constant_parameter_whose_default_value_was_not_set()
      {
         var parameterDTO = new ParameterDTO(_constantParameter);
          _constantParameter.DefaultValue = null;
         sut.IsSetByUser(parameterDTO).ShouldBeFalse();
      }


      [Observation]
      public void should_return_false_if_the_parameter_is_a_constant_parameter_whose_default_value_is_equal_to_the_current_value()
      {
         var parameterDTO = new ParameterDTO(_constantParameter);
         _constantParameter.DefaultValue = _constantParameter.Value;
         sut.IsSetByUser(parameterDTO).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_parameter_is_a_distributed_parameter_whose_value_differs_from_the_default_value()
      {
         var parameterDTO = new ParameterDTO(_distributedParameter);
         _distributedParameter.IsFixedValue = true;
         sut.IsSetByUser(parameterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_a_distributed_parameter_whose_default_value_was_not_set()
      {
         var parameterDTO = new ParameterDTO(_distributedParameter);
          _distributedParameter.DefaultValue = null;
         sut.IsSetByUser(parameterDTO).ShouldBeFalse();
      }


      [Observation]
      public void should_return_false_if_the_parameter_is_a_distributed_parameter_whose_default_value_is_equal_to_the_current_value()
      {
         var parameterDTO = new ParameterDTO(_distributedParameter);
         _distributedParameter.DefaultValue = _distributedParameter.Value;
         sut.IsSetByUser(parameterDTO).ShouldBeFalse();
      }
   }

   
   public class When_checking_if_a_parameter_is_a_formula_parameter_not_fixed : concern_for_ParameterSetPresenter
   {
      private IParameter _formulaParameter;
      private IParameter _constantParameter;
      private IParameter _distributedParameter;

      protected override void Context()
      {
         base.Context();
         _formulaParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _formulaParameter.Formula = new ExplicitFormula("10");
         _constantParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _distributedParameter = DomainHelperForSpecs.NormalDistributedParameter();

      }

      [Observation]
      public void should_return_false_if_the_parameter_is_a_formula_parameter_whose_value_was_fixed()
      {
         _formulaParameter.Value = 5;
         var parameterDTO = new ParameterDTO(_formulaParameter);
         parameterDTO.FormulaType = FormulaType.Rate;
         sut.IsFormulaNotFixed(parameterDTO).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_parameter_is_a_formula_parameter_whose_value_was_not_fixed()
      {
         var parameterDTO = new ParameterDTO(_formulaParameter);
         _formulaParameter.IsFixedValue = false;
          parameterDTO.FormulaType = FormulaType.Rate;
         sut.IsFormulaNotFixed(parameterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_not_a_formula_parameter()
      {
         var parameterDTO = new ParameterDTO(_constantParameter);
         parameterDTO.FormulaType = FormulaType.Constant;
         sut.IsFormulaNotFixed(parameterDTO).ShouldBeFalse();
      }
   }
}	