using OSPSuite.Utility.Extensions;

using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;

using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_ScaleParametersPresenter : ContextSpecification<IScaleParametersPresenter>
   {
      protected IScaleParametersView _view;
      protected IParameterSetPresenter _parameterSetPresenter;
      protected ParameterScaleWithFactorDTO _parameterScaleWithFactorDTO;

      protected override void Context()
      {
         _view = A.Fake<IScaleParametersView>();
         _parameterSetPresenter = A.Fake<IParameterSetPresenter>();
         A.CallTo(() => _view.BindTo(A<ParameterScaleWithFactorDTO>._))
          .Invokes(x => _parameterScaleWithFactorDTO = x.GetArgument<ParameterScaleWithFactorDTO>(0));

         sut = new ScaleParametersPresenter(_view);
         sut.InitializeWith(_parameterSetPresenter);

      }
   }

   
   public class When_initializing_the_parameter_scale_presenter : concern_for_ScaleParametersPresenter
   {
     
      [Observation]
      public void should_bind_a_parameter_scale_dto_to_the_view()
      {
         A.CallTo(() => _view.BindTo(_parameterScaleWithFactorDTO)).MustHaveHappened();
      }
   }

   
   public class When_the_user_activates_the_scaling_of_a_parameter_with_a_factor : concern_for_ScaleParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         _parameterScaleWithFactorDTO.Factor = 15;
      }

      protected override void Because()
      {
         sut.Scale();
      }

      [Observation]
      public void should_notify_the_parameter_set_presenter_to_scale_the_parameters_with_the_factor_defined_by_the_user()
      {
         A.CallTo(() => _parameterSetPresenter.ScaleParametersWith(_parameterScaleWithFactorDTO.Factor)).MustHaveHappened();
      }
   }

   
   public class When_the_user_decides_to_reset_the_value_of_all_parameters_to_their_default : concern_for_ScaleParametersPresenter
   {
 
      protected override void Because()
      {
         sut.Reset();
      }

      [Observation]
      public void should_notify_the_parameter_set_presenter_to_reset_the_parameters_to_their_original_values()
      {
         A.CallTo(() => _parameterSetPresenter.ResetParameters()).MustHaveHappened();
      }
   }
}	