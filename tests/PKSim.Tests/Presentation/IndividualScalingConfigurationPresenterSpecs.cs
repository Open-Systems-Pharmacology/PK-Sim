using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;



using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualScalingConfigurationPresenter : ContextSpecification<IIndividualScalingConfigurationPresenter>
   {
      protected IIndividualScalingConfigurationView _view;
      protected IIndividualScalingTask _individualScalingTask;
      protected IParameterScalingToParameterScalingDTOMapper _mapper;
      protected IScalingMethodTask _scalingMethodTask;
      protected IIndividualPresenter _individualPresenter;
      protected IParameterToParameterDTOMapper _parameterDTOMapper;

      protected override void Context()
      {
         _view = A.Fake<IIndividualScalingConfigurationView>();
         _individualScalingTask = A.Fake<IIndividualScalingTask>();
         _mapper = A.Fake<IParameterScalingToParameterScalingDTOMapper>();
         _scalingMethodTask = A.Fake<IScalingMethodTask>();
         _individualPresenter = A.Fake<IIndividualPresenter>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();

         sut = new IndividualScalingConfigurationPresenter(_view, _individualScalingTask, _mapper, _scalingMethodTask,_parameterDTOMapper);
         sut.InitializeWith(_individualPresenter);
      }
   }

   
   

   
   public class When_asked_for_its_view : concern_for_IndividualScalingConfigurationPresenter
   {
      [Observation]
      public void should_return_the_view_it_was_initialized_with()
      {
         sut.BaseView.ShouldBeEqualTo(_view);
      }
   }

   
   public class When_editing_an_individuak : concern_for_IndividualScalingConfigurationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditIndividual(A.Fake<PKSim.Core.Model.Individual>());
      }

      [Observation]
      public void should_never_notify_a_state_that_cannot_be_closed()
      {
         sut.CanClose.ShouldBeTrue();
      }
   }

   
   public class When_configuring_the_scaling_from_one_individual_to_another : concern_for_IndividualScalingConfigurationPresenter
   {
      protected PKSim.Core.Model.Individual _targetIndividual;
      protected PKSim.Core.Model.Individual _sourceIndividual;
      protected ParameterScaling _parameterScaling1;
      protected ParameterScaling _parameterScaling2;
      protected ParameterScalingDTO _parameterScalingDTO1;
      protected ParameterScalingDTO _parameterScalingDTO2;

      protected override void Context()
      {
         base.Context();
         _targetIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _sourceIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _parameterScaling1 = A.Fake<ParameterScaling>();
         _parameterScaling2 = A.Fake<ParameterScaling>();
         _parameterScalingDTO1 = new ParameterScalingDTO(_parameterScaling1);
         _parameterScalingDTO2 = new ParameterScalingDTO(_parameterScaling2);
         A.CallTo(() => _individualScalingTask.AllParameterScalingsFrom(_sourceIndividual, _targetIndividual)).Returns(new[] {_parameterScaling1, _parameterScaling2});
         A.CallTo(() => _mapper.MapFrom(_parameterScaling1)).Returns(_parameterScalingDTO1);
         A.CallTo(() => _mapper.MapFrom(_parameterScaling2)).Returns(_parameterScalingDTO2);
      }

      protected override void Because()
      {
         sut.ConfigureScaling(_sourceIndividual, _targetIndividual);
      }

      [Observation]
      public void should_retrieve_the_scaling_parameters_options()
      {
         A.CallTo(() => _individualScalingTask.AllParameterScalingsFrom(_sourceIndividual, _targetIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_view_with_the_parameter_scaling_dto()
      {
         A.CallTo(() => _view.BindTo(A < IEnumerable<ParameterScalingDTO>>.Ignored)).MustHaveHappened();
      }

    
   }

 

  
   
   public class When_retrieving_the_list_of_scaling_methods_for_a_parameter_scaling_dto : concern_for_IndividualScalingConfigurationPresenter
   {
      private IEnumerable<ScalingMethod> _result;
      private IEnumerable<ScalingMethod> _methodLists;
      private ParameterScalingDTO _parameterScalingDTO;
      private ParameterScaling _parameterScaling;

      protected override void Context()
      {
         base.Context();
         _methodLists = A.Fake<IEnumerable<ScalingMethod>>();
         _parameterScaling = A.Fake<ParameterScaling>();
         _parameterScalingDTO = new ParameterScalingDTO(_parameterScaling);
         A.CallTo(() => _scalingMethodTask.AllMethodsFor(_parameterScaling)).Returns(_methodLists);
      }

      protected override void Because()
      {
         _result = sut.AllScalingMethodsFor(_parameterScalingDTO);
      }

      [Observation]
      public void should_leverage_the_scaling_method_task_to_retrieve_the_list_of_all_availble_method_for_the_parameter_scaling()
      {
         _result.ShouldBeEqualTo(_methodLists);
      }
   }

   
   public class When_notified_that_the_scaling_method_has_changed : concern_for_IndividualScalingConfigurationPresenter
   {
      private ParameterScalingDTO _parameterScalingDTO;
      private ScalingMethod _newScalingMethod;
      private ParameterScaling _parameterScaling;

      protected override void Context()
      {
         base.Context();
         _newScalingMethod = A.Fake<ScalingMethod>();
         _parameterScaling = A.Fake<ParameterScaling>();
         _parameterScalingDTO = new ParameterScalingDTO(_parameterScaling);
         A.CallTo(() => _parameterScaling.TargetScaledValueInDisplayUnit).Returns(40);
      }

      protected override void Because()
      {
         sut.ScalingMethodChanged(_parameterScalingDTO, _newScalingMethod);
      }

      [Observation]
      public void should_set_the_new_scaling_method_in_the_corresponding_parameter_scaling()
      {
         _parameterScaling.ScalingMethod.ShouldBeEqualTo(_newScalingMethod);
      }

      [Observation]
      public void should_update_the_value_of_the_new_value_of_the_parameter_scaling_dto_with_the_new_value_from_the_scaling_method()
      {
         _parameterScalingDTO.TargetScaledValue.ShouldBeEqualTo(_parameterScaling.TargetScaledValueInDisplayUnit);
      }
   }

   
   public class When_performing_the_actual_scaling_between_a_source_and_a_target_individual : concern_for_IndividualScalingConfigurationPresenter
   {
      private PKSim.Core.Model.Individual _targetIndividual;
      private PKSim.Core.Model.Individual _sourceIndividual;
      private List<IPKSimCommand> _scalingCommand;
      private IPKSimCommand _subCommand;

      protected override void Context()
      {
         base.Context();
         _targetIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _sourceIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _subCommand =A.Fake<IPKSimCommand>();
         _scalingCommand = new List<IPKSimCommand> {_subCommand};
         A.CallTo(() => _individualScalingTask.AllParameterScalingsFrom(_sourceIndividual, _targetIndividual)).Returns(new List<ParameterScaling>());
         A.CallTo(() => _view.HasError).Returns(false);
         A.CallTo(() => _individualScalingTask.PerformScaling(A<IEnumerable<ParameterScaling>>._)).Returns(_scalingCommand);
         sut.ConfigureScaling(_sourceIndividual, _targetIndividual);
      }

      protected override void Because()
      {
         sut.PerformScaling();
      }

      [Observation]
      public void should_leverage_the_individual_task_to_perform_the_scaling()
      {
         A.CallTo(() => _individualScalingTask.PerformScaling(A<IEnumerable<ParameterScaling>>._)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_resulting_commands_to_the_parent_individual_presenter()
      {
         A.CallTo(() => _individualPresenter.AddCommand(_subCommand)).MustHaveHappened();
      }

      [Observation]
      public void should_be_able_to_close()
      {
         sut.CanClose.ShouldBeTrue();
      }
   }

   public class When_performing_a_scaling_for_parameters_that_are_not_organ_volumes : concern_for_IndividualScalingConfigurationPresenter
   {
      protected PKSim.Core.Model.Individual _targetIndividual;
      protected PKSim.Core.Model.Individual _sourceIndividual;
      protected ParameterScaling _parameterScaling1;
      protected ParameterScaling _parameterScaling2;
      protected ParameterScalingDTO _parameterScalingDTO1;
      protected ParameterScalingDTO _parameterScalingDTO2;

      protected override void Context()
      {
         base.Context();
         _targetIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _sourceIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _parameterScaling1 = A.Fake<ParameterScaling>();
         _parameterScaling2 = A.Fake<ParameterScaling>();
         var p1 = A.Fake<IParameter>().WithName("P1").WithParentContainer(A.Fake<IContainer>());
         var p2 = A.Fake<IParameter>().WithName("P2").WithParentContainer(A.Fake<IContainer>());

         A.CallTo(() => _individualScalingTask.AllParameterScalingsFrom(_sourceIndividual, _targetIndividual)).Returns(new[] { _parameterScaling1, _parameterScaling2 });
         A.CallTo(() => _parameterScaling1.SourceParameter).Returns(p1);
         A.CallTo(() => _parameterScaling2.SourceParameter).Returns(p2);
      }

      protected override void Because()
      {
         sut.ConfigureScaling(_sourceIndividual, _targetIndividual);
      }

      [Observation]
      public void should_update_the_view_with_the_parameter_scaling_dto()
      {
         A.CallTo(() => _view.BindToWeight()).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_display_the_body_weight()
      {
         _view.WeightVisible.ShouldBeFalse();
      }
   }

   public class When_performing_a_scaling_for_parameters_that_contain_at_least_one_organ_volume: concern_for_IndividualScalingConfigurationPresenter
   {
      protected PKSim.Core.Model.Individual _targetIndividual;
      protected PKSim.Core.Model.Individual _sourceIndividual;
      protected ParameterScaling _parameterScaling1;
      protected ParameterScaling _parameterScaling2;
      protected ParameterScalingDTO _parameterScalingDTO1;
      protected ParameterScalingDTO _parameterScalingDTO2;

      protected override void Context()
      {
         base.Context();
         _targetIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _sourceIndividual = A.Fake<PKSim.Core.Model.Individual>();
         _parameterScaling1 = A.Fake<ParameterScaling>();
         _parameterScaling2 = A.Fake<ParameterScaling>();
         var volume =new PKSimParameter().WithName(CoreConstants.Parameter.VOLUME).WithParentContainer(new Organ());
         var p2 = new PKSimParameter().WithName("P2").WithParentContainer(new Container());

         A.CallTo(() => _individualScalingTask.AllParameterScalingsFrom(_sourceIndividual, _targetIndividual)).Returns(new[] { _parameterScaling1, _parameterScaling2 });
         A.CallTo(() => _parameterScaling1.SourceParameter).Returns(volume);
         A.CallTo(() => _parameterScaling2.SourceParameter).Returns(p2);
      }

      protected override void Because()
      {
         sut.ConfigureScaling(_sourceIndividual, _targetIndividual);
      }

      [Observation]
      public void should_update_the_view_with_the_parameter_scaling_dto()
      {
         A.CallTo(() => _view.BindToWeight()).MustHaveHappened();
      }

      [Observation]
      public void should_not_display_the_body_weight()
      {
         _view.WeightVisible.ShouldBeTrue();
      }
   }
}