using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;



namespace PKSim.Presentation
{
   public abstract class concern_for_UserDefinedParametersPresenter : ContextSpecification<IUserDefinedParametersPresenter>
   {
      protected IMultiParameterEditView _view;
      protected IScaleParametersPresenter _scaleParameterPresenter;
      protected IParameterTask _parameterTask;
      protected IEditParameterPresenterTask _editParameterTask;
      protected IParameterToParameterDTOMapper _parameterDTOMapper;
      protected IParameterContextMenuFactory _parameterContextMenuFactory;

      protected override void Context()
      {
         _view = A.Fake<IMultiParameterEditView>();
         _scaleParameterPresenter = A.Fake<IScaleParametersPresenter>();
         _parameterTask = A.Fake<IParameterTask>();
         _editParameterTask = A.Fake<IEditParameterPresenterTask>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         _parameterContextMenuFactory = A.Fake<IParameterContextMenuFactory>();

         sut = new UserDefinedParametersPresenter(_view, _scaleParameterPresenter, _editParameterTask, _parameterTask, _parameterDTOMapper, _parameterContextMenuFactory);

         A.CallTo(() => _parameterDTOMapper.MapFrom(A<IParameter>._)).ReturnsLazily(x => new ParameterDTO(x.GetArgument<IParameter>(0)));
      }
   }

   public class When_editing_the_user_defined_parameter : concern_for_UserDefinedParametersPresenter
   {
      private List<IParameter> _parameters;
      private IParameter _parameter1;
      private IParameter _parameter2;
      private IParameter _parameter3;

      protected override void Context()
      {
         base.Context();
         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(20);
         _parameter3 = DomainHelperForSpecs.ConstantParameterWithValue(30);
         _parameters = new List<IParameter>(new[] {_parameter1, _parameter2, _parameter3});

         _parameter1.IsDefault = false;
         _parameter2.IsDefault = true;
         _parameter3.IsDefault = false;
      }

      protected override void Because()
      {
         sut.Edit(_parameters);
      }

      [Observation]
      public void should_return_that_a_refresh_is_always_required()
      {
         sut.AlwaysRefresh.ShouldBeTrue();
      }

      [Observation]
      public void should_only_show_those_parameters_that_are_not_default_parameter()
      {
         sut.EditedParameters.ShouldOnlyContain(_parameter1, _parameter3);
      }

      [Observation]
      public void should_always_show_the_parameter_names()
      {
         _view.ParameterNameVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_hide_distribution_column()
      {
         _view.DistributionVisible.ShouldBeFalse();
      }

      [Observation]
      public void should_hide_scaling()
      {
         _view.ScalingVisible.ShouldBeFalse();
      }
   }
}