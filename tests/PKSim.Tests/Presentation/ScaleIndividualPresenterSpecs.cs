using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using PKSim.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_ScaleIndividualPresenter : ContextSpecification<IScaleIndividualPresenter>
   {
      protected IScaleIndividualView _view;
      protected ICoreWorkspace _workspace;
      protected IIndividualSettingsPresenter _settingPresenter;
      protected IIndividualParametersPresenter _parameterPresenter;
      protected Individual _sourceIndividual;
      protected IndividualSettingsDTO _individualSettingsDTO;
      protected IIndividualScalingConfigurationPresenter _scalingConfigurationPresenter;
      protected Individual _scaledIndividual;
      protected Individual _cloneIndividual;
      protected IIndividualMoleculesPresenter _moleculePresenter;
      protected IIndividualExpressionsUpdater _individualExpressionsUpdater;
      protected ISubPresenterItemManager<IIndividualItemPresenter> _subPresenterManager;
      protected IObjectBaseDTOFactory _objectBaseDTOFactory;
      protected IBuildingBlockPropertiesMapper _propertiesMapper;
      protected ObjectBaseDTO _scaleIndividualPropertiesDTO;
      private IDialogCreator _dialogCreator;
      protected ICloner _cloner;

      protected override void Context()
      {
         _subPresenterManager = SubPresenterHelper.Create<IIndividualItemPresenter>();
         _view = A.Fake<IScaleIndividualView>();
         _propertiesMapper = A.Fake<IBuildingBlockPropertiesMapper>();
         _workspace = A.Fake<ICoreWorkspace>();
         _individualExpressionsUpdater = A.Fake<IIndividualExpressionsUpdater>();
         _cloner = A.Fake<ICloner>();
         _sourceIndividual = A.Fake<Individual>();
         _cloneIndividual = A.Fake<Individual>();
         _scaledIndividual = A.Fake<Individual>();
         _objectBaseDTOFactory = A.Fake<IObjectBaseDTOFactory>();
         _settingPresenter = _subPresenterManager.CreateFake(ScaleIndividualItems.Settings);
         _parameterPresenter = _subPresenterManager.CreateFake(ScaleIndividualItems.Parameters);
         _scalingConfigurationPresenter = _subPresenterManager.CreateFake(ScaleIndividualItems.Scaling);
         _moleculePresenter = _subPresenterManager.CreateFake(ScaleIndividualItems.Expressions);
         ScaleIndividualItems.Expressions.Index = 3;
         A.CallTo(() => _cloner.Clone(_sourceIndividual)).Returns(_cloneIndividual);
         _individualSettingsDTO = new IndividualSettingsDTO();

         _dialogCreator = A.Fake<IDialogCreator>();
         A.CallTo(() => _settingPresenter.Individual).Returns(_scaledIndividual);
         _scaleIndividualPropertiesDTO = new ObjectBaseDTO();
         A.CallTo(() => _objectBaseDTOFactory.CreateFor<Individual>()).Returns(_scaleIndividualPropertiesDTO);

         sut = new ScaleIndividualPresenter(_view, _subPresenterManager, _dialogCreator, _individualExpressionsUpdater,
            _objectBaseDTOFactory, _propertiesMapper, _cloner);
         sut.Initialize();
      }
   }

   public class When_one_sub_presenter_is_notifying_the_scale_presenter_of_a_dirty_state : concern_for_ScaleIndividualPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.ScaleIndividual(_sourceIndividual);
         A.CallTo(() => _settingPresenter.CanClose).Returns(false);
      }

      protected override void Because()
      {
         _settingPresenter.StatusChanged += Raise.WithEmpty();
      }

      [Observation]
      public void the_ok_button_of_the_view_should_be_disabled()
      {
         _view.OkEnabled.ShouldBeFalse();
      }

      [Observation]
      public void the_next_button_of_the_view_should_be_disabled()
      {
         _view.NextEnabled.ShouldBeFalse();
      }
   }

   public class When_starting_the_scale_individual : concern_for_ScaleIndividualPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _settingPresenter.CanClose).Returns(true);
      }

      protected override void Because()
      {
         sut.ScaleIndividual(_sourceIndividual);
      }

      [Observation]
      public void should_ask_the_view_to_render()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }

      [Observation]
      public void should_clone_the_individual_to_scale()
      {
         A.CallTo(() => _cloner.Clone(_sourceIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_enable_the_setting_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(ScaleIndividualItems.Settings, true)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_parameter_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(ScaleIndividualItems.Parameters, false)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_expression_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(ScaleIndividualItems.Expressions, false)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_scale_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(ScaleIndividualItems.Scaling, false)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_ok_button()
      {
         _view.OkEnabled.ShouldBeFalse();
      }

      [Observation]
      public void should_tell_the_view_to_enable_the_next_button()
      {
         _view.NextEnabled.ShouldBeTrue();
      }

      [Observation]
      public void should_update_the_individual_with_the_name_and_description_defined_by_the_user()
      {
         A.CallTo(() => _propertiesMapper.MapProperties(_scaleIndividualPropertiesDTO, _scaledIndividual)).MustHaveHappened();
      }
   }

   public class When_the_user_cancel_the_scale_individual_action : concern_for_ScaleIndividualPresenter
   {
      private IPKSimCommand _resultOfScaleIndividual;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _resultOfScaleIndividual = sut.ScaleIndividual(_sourceIndividual);
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _resultOfScaleIndividual.ShouldBeAnInstanceOf<PKSimEmptyCommand>();
      }
   }

   public class When_user_actions_were_added_to_the_list_of_performed_command_while_scaling : concern_for_ScaleIndividualPresenter
   {
      private IPKSimCommand _command1;
      private IPKSimCommand _command2;
      private IPKSimMacroCommand _resultOfScaleIndividual;

      protected override void Context()
      {
         base.Context();
         _command1 = A.Fake<IPKSimCommand>();
         _command2 = A.Fake<IPKSimCommand>();
         sut.AddCommand(_command1);
         sut.AddCommand(_command2);
      }

      protected override void Because()
      {
         _resultOfScaleIndividual = sut.ScaleIndividual(_sourceIndividual) as IPKSimMacroCommand;
      }

      [Observation]
      public void should_return_a_macro_command_containing_all_sub_commands_executed_by_the_user()
      {
         _resultOfScaleIndividual.ShouldNotBeNull();
         _resultOfScaleIndividual.All().ShouldOnlyContainInOrder(_command1, _command2);
      }
   }

   public class When_notify_that_the_settings_for_the_scaling_are_completed : concern_for_ScaleIndividualPresenter
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         sut.ScaleIndividual(_sourceIndividual);
      }

      protected override void Because()
      {
         _result = sut.CreateIndividual();
      }

      [Observation]
      public void the_result_of_the_create_individual_operation_should_be_true()
      {
         _result.ShouldBeTrue();
      }

      [Observation]
      public void should_create_a_new_individual_based_on_the_source_individual_properties()
      {
         A.CallTo(() => _settingPresenter.CreateIndividual()).MustHaveHappened();
      }

      [Observation]
      public void should_tell_scaling_configuration_presenter_to_update_the_configuration()
      {
         A.CallTo(() => _scalingConfigurationPresenter.ConfigureScaling(_cloneIndividual, _scaledIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_not_tell_the_parameter_presenter_to_update_itself_from_the_individual()
      {
         A.CallTo(() => _parameterPresenter.EditIndividual(_scaledIndividual)).MustNotHaveHappened();
      }

      [Observation]
      public void should_update_the_enzyme_expression_from_the_source_individual_to_the_target_individual()
      {
         A.CallTo(() => _individualExpressionsUpdater.Update(_cloneIndividual, _scaledIndividual)).MustHaveHappened();
      }
   }

   public class When_notify_that_the_settings_for_the_scaling_are_completed_but_the_creation_of_the_scaled_individual_was_not_successful : concern_for_ScaleIndividualPresenter
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         sut.ScaleIndividual(_sourceIndividual);
         A.CallTo(() => _settingPresenter.Individual).Returns(null);
      }

      protected override void Because()
      {
         _result = sut.CreateIndividual();
      }

      [Observation]
      public void the_result_of_the_create_individual_operation_should_be_false()
      {
         _result.ShouldBeFalse();
      }

      [Observation]
      public void should_create_a_new_individual_based_on_the_source_individual_properties()
      {
         A.CallTo(() => _settingPresenter.CreateIndividual()).MustHaveHappened();
      }

      [Observation]
      public void should_not_tell_scaling_configuration_presenter_to_update_the_configuration()
      {
         A.CallTo(() => _scalingConfigurationPresenter.ConfigureScaling(_cloneIndividual, A<Individual>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_update_the_enzyme_expression_from_the_source_individual_to_the_target_individual()
      {
         A.CallTo(() => _individualExpressionsUpdater.Update(_cloneIndividual, _scaledIndividual)).MustNotHaveHappened();
      }
   }

   public class When_the_scale_individual_presenter_is_performing_the_individual_scaling : concern_for_ScaleIndividualPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.ScaleIndividual(_sourceIndividual);
      }

      protected override void Because()
      {
         sut.PerformScaling();
      }

      [Observation]
      public void should_trigger_the_scaling_from_the_scaling_presenter()
      {
         A.CallTo(() => _scalingConfigurationPresenter.PerformScaling()).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_parameter_view_with_the_parameters_from_the_scaled_individual()
      {
         A.CallTo(() => _parameterPresenter.EditIndividual(_scaledIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_enzyme_view_with_the_expressions_from_the_scaled_individual()
      {
         A.CallTo(() => _moleculePresenter.EditIndividual(_scaledIndividual)).MustHaveHappened();
      }
   }
}