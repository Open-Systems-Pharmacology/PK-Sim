using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_MultiParameterEditPresenter : ContextSpecification<IMultiParameterEditPresenter>
   {
      protected IParameterTask _parameterTask;
      protected IMultiParameterEditView _view;
      protected IParameterToParameterDTOMapper _mapper;
      protected IParameter _parameter;
      protected ParameterDTO _parameterDTO;
      protected ICommandCollector _commandRegister;
      protected bool _eventWasRaised;
      protected IScaleParametersPresenter _scaleParameterPresenter;
      protected IParameterContextMenuFactory _contextMenuFactory;
      protected IList<IParameter> _parameterList;
      protected IEditParameterPresenterTask _editParameterPresenterTask;

      protected override void Context()
      {
         _parameterTask = A.Fake<IParameterTask>();
         _view = A.Fake<IMultiParameterEditView>();
         _scaleParameterPresenter = A.Fake<IScaleParametersPresenter>();
         _mapper = A.Fake<IParameterToParameterDTOMapper>();
         _editParameterPresenterTask = A.Fake<IEditParameterPresenterTask>();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         _parameterDTO = new ParameterDTO(_parameter);
         _commandRegister = A.Fake<ICommandCollector>();
         _contextMenuFactory = A.Fake<IParameterContextMenuFactory>();
         A.CallTo(() => _scaleParameterPresenter.View).Returns(A.Fake<IScaleParametersView>());
         A.CallTo(() => _mapper.MapFrom(_parameter)).Returns(_parameterDTO);
         sut = new MultiParameterEditPresenter(_view, _scaleParameterPresenter, _editParameterPresenterTask, _parameterTask, _mapper, _contextMenuFactory);
         sut.StatusChanged += (o, e) => { _eventWasRaised = true; };
         sut.InitializeWith(_commandRegister);
         _parameterList = new List<IParameter>();
      }
   }

   public class When_initializing_the_multi_parameter_edit : concern_for_MultiParameterEditPresenter
   {
      [Observation]
      public void should_register_itself_to_the_scale_parameters_presenter()
      {
         A.CallTo(() => _scaleParameterPresenter.InitializeWith(sut)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_scale_parameter_view_into_the_parameter_edit_view()
      {
         A.CallTo(() => _view.SetScaleParameterView(_scaleParameterPresenter.View)).MustHaveHappened();
      }
   }

   public class When_setting_a_value_for_a_parameter : concern_for_MultiParameterEditPresenter
   {
      private double _value;

      protected override void Context()
      {
         base.Context();
         _value = 5;
      }

      protected override void Because()
      {
         sut.SetParameterValue(_parameterDTO, _value);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_set_the_value_into_the_parameter()
      {
         A.CallTo(() => _editParameterPresenterTask.SetParameterValue(sut, _parameterDTO, _value)).MustHaveHappened();
      }
   }

   public class When_registering_a_command : concern_for_MultiParameterEditPresenter
   {
      protected override void Because()
      {
         sut.AddCommand(A.Fake<IPKSimCommand>());
      }

      [Observation]
      public void should_notify_that_the_state_of_the_view_might_have_changed()
      {
         _eventWasRaised.ShouldBeTrue();
      }
   }

   public class When_setting_a_percentile_for_a_parameter : concern_for_MultiParameterEditPresenter
   {
      private double _percentileInPercent;

      protected override void Context()
      {
         base.Context();
         _percentileInPercent = 50;
      }

      protected override void Because()
      {
         sut.SetParameterPercentile(_parameterDTO, _percentileInPercent);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_set_the_percentile_into_the_parameter()
      {
         A.CallTo(() => _editParameterPresenterTask.SetParameterPercentile(sut, _parameterDTO, _percentileInPercent)).MustHaveHappened();
      }
   }

   public class When_setting_a_new_gui_unit_for_a_parameter : concern_for_MultiParameterEditPresenter
   {
      private Unit _newGuiUnit;

      protected override void Context()
      {
         base.Context();
         //Create volume dimension
         _newGuiUnit = A.Fake<Unit>();
      }

      protected override void Because()
      {
         //change unit from liter to ml
         sut.SetParameterUnit(_parameterDTO, _newGuiUnit);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_set_the_new_unit_in_the_parametr()
      {
         A.CallTo(() => _editParameterPresenterTask.SetParameterUnit(sut, _parameterDTO, _newGuiUnit)).MustHaveHappened();
      }
   }

   public class When_the_multi_parameter_presenter_is_asked_for_its_view : concern_for_MultiParameterEditPresenter
   {
      [Observation]
      public void should_return_the_view()
      {
         sut.View.ShouldBeEqualTo(_view);
      }
   }

   public class When_asked_if_a_parameter_dto_is_distributed : concern_for_MultiParameterEditPresenter
   {
      private ParameterDTO _distribuedParamterDTO;
      private ParameterDTO _nonDistibuedParameterDTO;
      private ParameterDTO _disrecteDistributedParameterDTO;

      protected override void Context()
      {
         base.Context();
         _distribuedParamterDTO = A.Fake<ParameterDTO>();
         A.CallTo(() => _distribuedParamterDTO.Parameter).Returns(new PKSimDistributedParameter().WithFormula(new LogNormalDistributionFormula()));
         _nonDistibuedParameterDTO = A.Fake<ParameterDTO>();
         A.CallTo(() => _nonDistibuedParameterDTO.Parameter).Returns(A.Fake<IParameter>());
         _disrecteDistributedParameterDTO = A.Fake<ParameterDTO>();
         var discretedDistributedParameter = new PKSimDistributedParameter().WithFormula(new DiscreteDistributionFormula());
         A.CallTo(() => _disrecteDistributedParameterDTO.Parameter).Returns(discretedDistributedParameter);
      }

      [Observation]
      public void should_return_true_for_a_distributed_parameter()
      {
         sut.ParameterIsDistributed(_distribuedParamterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_non_distributed_parameter()
      {
         sut.ParameterIsDistributed(_nonDistibuedParameterDTO).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_for_a_discreted_distributed_parameter()
      {
         sut.ParameterIsDistributed(_disrecteDistributedParameterDTO).ShouldBeFalse();
      }
   }

   public class When_updating_the_container_visibility_container : concern_for_MultiParameterEditPresenter
   {
      [Observation]
      public void should_show_the_container_if_visible()
      {
         sut.ContainerVisible = true;
         A.CallTo(() => _view.SetVisibility(PathElement.Container, true)).MustHaveHappened();
      }

      [Observation]
      public void should_hide_the_container_if_unvisible()
      {
         sut.ContainerVisible = false;
         A.CallTo(() => _view.SetVisibility(PathElement.Container, false)).MustHaveHappened();
      }
   }

   public class When_updating_the_parameter_scaling_visibility : concern_for_MultiParameterEditPresenter
   {
      [Observation]
      public void should_show_the_parameter_scale_control_if_visible()
      {
         sut.ScalingVisible = true;
         _view.ScalingVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_hide_the_parameter_scale_control_if_unvisible()
      {
         sut.ScalingVisible = false;
         _view.ScalingVisible.ShouldBeFalse();
      }
   }

   public class When_updating_the_distribution_visibility : concern_for_MultiParameterEditPresenter
   {
      [Observation]
      public void should_show_the_distribution_if_visible()
      {
         sut.DistributionVisible = true;
         _view.DistributionVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_hide_the_distribution_if_unvisible()
      {
         sut.DistributionVisible = false;
         _view.DistributionVisible.ShouldBeFalse();
      }
   }

   public class When_editing_a_set_of_parameters : concern_for_MultiParameterEditPresenter
   {
      private IParameter _visibleParameter;
      private IParameter _hiddenUnchangedParameter;
      private IParameter _hiddenIsFixedParameter;
      private IParameter _hiddenIsNotDefaultParameter;
      private List<ParameterDTO> _allParameterDTO;
      private List<IParameter> _displayedParameters;

      protected override void Context()
      {
         base.Context();
         _visibleParameter = new PKSimParameter {Visible = true};
         _hiddenUnchangedParameter = new PKSimParameter {Visible = false, IsDefault = true};
         _hiddenIsFixedParameter = new PKSimParameter {Visible = false, IsFixedValue = true};
         _hiddenIsNotDefaultParameter = new PKSimParameter {Visible = false, IsDefault = false};

         _parameterList.Add(_visibleParameter);
         _parameterList.Add(_hiddenUnchangedParameter);
         _parameterList.Add(_hiddenIsFixedParameter);
         _parameterList.Add(_hiddenIsNotDefaultParameter);

         A.CallTo(() => _mapper.MapFrom(A<IParameter>._)).ReturnsLazily(x => new ParameterDTO(x.GetArgument<IParameter>(0)));

         A.CallTo(() => _view.BindTo(A<IEnumerable<ParameterDTO>>._))
            .Invokes(x => _allParameterDTO = x.GetArgument<IEnumerable<ParameterDTO>>(0).ToList());
      }

      protected override void Because()
      {
         sut.Edit(_parameterList);
         _displayedParameters = _allParameterDTO.Select(x => x.Parameter).ToList();
      }

      [Observation]
      public void should_show_all_visible_parameters()
      {
         _displayedParameters.ShouldContain(_visibleParameter);
      }

      [Observation]
      public void should_show_all_hidden_parameter_that_were_changed_by_the_user()
      {
         _displayedParameters.ShouldContain(_hiddenIsFixedParameter, _hiddenIsNotDefaultParameter);
      }

      [Observation]
      public void should_not_show_hidden_parameters_that_were_not_changed_by_the_user()
      {
         _displayedParameters.ShouldNotContain(_hiddenUnchangedParameter);
      }
   }

   public class When_asked_to_reset_all_the_parameters_to_their_original_values : concern_for_MultiParameterEditPresenter
   {
      private IPKSimCommand _resetParameterCommand;
      private IParameter _para1;
      private IParameter _para2;
      private IEnumerable<IParameter> _parameters;

      protected override void Context()
      {
         base.Context();
         _para1 = new PKSimParameter {Visible = true};
         _para2 = new PKSimParameter {Visible = false, IsDefault = true};
         _resetParameterCommand = A.Fake<IPKSimCommand>();
         _parameterList.Add(_para1);
         _parameterList.Add(_para2);
         var parameterDTO = new ParameterDTO(_para1);

         A.CallTo(() => _mapper.MapFrom(_para1)).Returns(parameterDTO);
         A.CallTo(() => _view.SelectedParameters).Returns(new[] {parameterDTO});

         A.CallTo(() => _parameterTask.ResetParameters(A<IEnumerable<IParameter>>._))
            .Invokes(x => _parameters = x.GetArgument<IEnumerable<IParameter>>(0))
            .Returns(_resetParameterCommand);

         sut.Edit(_parameterList);
      }

      protected override void Because()
      {
         sut.ResetParameters();
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_reset_all_the_displayed_parameters()
      {
         _parameters.ShouldOnlyContain(_para1);
      }

      [Observation]
      public void should_add_the_resulting_command_into_the_command_register()
      {
         A.CallTo(() => _commandRegister.AddCommand(_resetParameterCommand)).MustHaveHappened();
      }
   }

   public class When_asked_to_scale_all_parameters_values_with_a_given_factor : concern_for_MultiParameterEditPresenter
   {
      private IPKSimCommand _scaleParameterCommand;
      private IParameter _para1;
      private IEnumerable<IParameter> _parameters;

      protected override void Context()
      {
         base.Context();
         _para1 = new PKSimParameter {Visible = true};
         _scaleParameterCommand = A.Fake<IPKSimCommand>();
         _parameterList.Add(_para1);
         _parameterList.Add(new PKSimParameter {Visible = false, IsDefault = true});
         var parameterDTO = new ParameterDTO(_para1);
         A.CallTo(() => _mapper.MapFrom(_para1)).Returns(parameterDTO);
         A.CallTo(() => _view.SelectedParameters).Returns(new[] {parameterDTO});

         A.CallTo(() => _parameterTask.ScaleParameters(A<IEnumerable<IParameter>>._, 10))
            .Invokes(x => _parameters = x.GetArgument<IEnumerable<IParameter>>(0))
            .Returns(_scaleParameterCommand);

         sut.Edit(_parameterList);
      }

      protected override void Because()
      {
         sut.ScaleParametersWith(10);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_reset_all_the_displayed_parameters()
      {
         _parameters.ShouldOnlyContain(_para1);
      }

      [Observation]
      public void should_add_the_resulting_command_into_the_command_register()
      {
         A.CallTo(() => _commandRegister.AddCommand(_scaleParameterCommand)).MustHaveHappened();
      }
   }

   public class When_a_parameter_is_being_added_to_the_favorites : concern_for_MultiParameterEditPresenter
   {
      protected override void Context()
      {
         base.Context();
         _parameter = new PKSimParameter();
         _parameterDTO = new ParameterDTO(_parameter);
      }

      protected override void Because()
      {
         sut.SetFavorite(_parameterDTO, true);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_add_the_parameter_to_the_favorites()
      {
         A.CallTo(() => _editParameterPresenterTask.SetParameterFavorite(_parameterDTO, true)).MustHaveHappened();
      }
   }

   public class When_a_parameter_is_being_removed_from_the_favorites : concern_for_MultiParameterEditPresenter
   {
      protected override void Context()
      {
         base.Context();
         _parameter = new PKSimParameter();
         _parameterDTO = new ParameterDTO(_parameter);
      }

      protected override void Because()
      {
         sut.SetFavorite(_parameterDTO, false);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_remove_the_parameter_from_the_favorites()
      {
         A.CallTo(() => _editParameterPresenterTask.SetParameterFavorite(_parameterDTO, false)).MustHaveHappened();
      }
   }

   public class When_the_multi_parameter_edit_presenter_is_being_released : concern_for_MultiParameterEditPresenter
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Visible = true;
         _parameterList.Add(_parameter);
         _parameterDTO = A.Fake<ParameterDTO>();
         A.CallTo(() => _mapper.MapFrom(_parameter)).Returns(_parameterDTO);
         sut.Edit(_parameterList);
      }

      protected override void Because()
      {
         var eventPublisher = A.Fake<IEventPublisher>();
         sut.ReleaseFrom(eventPublisher);
      }

      [Observation]
      public void should_also_release_all_parameter_dto_explicitely()
      {
         A.CallTo(() => _parameterDTO.Release()).MustHaveHappened();
      }
   }
}