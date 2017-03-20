using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateIndividualPresenter : ContextSpecification<ICreateIndividualPresenter>
   {
      protected ICreateIndividualView _view;
      protected IIndividualSettingsPresenter _individualSettingsPresenter;
      protected IIndividualParametersPresenter _individualParameterPresenter;
      protected IIndividualMoleculesPresenter _individualMoleculesPresenter;
      protected ISubPresenterItemManager<IIndividualItemPresenter> _subPresenterManager;
      protected IBuildingBlockPropertiesMapper _propertiesMapper;
      private ObjectBaseDTO _individualPropertiesDTO;
      private IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _subPresenterManager = SubPresenterHelper.Create<IIndividualItemPresenter>();
         _propertiesMapper = A.Fake<IBuildingBlockPropertiesMapper>();
         _buildingBlockDTOFactory = A.Fake<IObjectBaseDTOFactory>();
         _individualSettingsPresenter = _subPresenterManager.CreateFake(IndividualItems.Settings);
         _individualParameterPresenter = _subPresenterManager.CreateFake(IndividualItems.Parameters);
         _individualMoleculesPresenter = _subPresenterManager.CreateFake(IndividualItems.Expression);
         _dialogCreator = A.Fake<IDialogCreator>();
         _individualPropertiesDTO = new ObjectBaseDTO();
         _view = A.Fake<ICreateIndividualView>();
         A.CallTo(() => _buildingBlockDTOFactory.CreateFor<Individual>()).Returns(_individualPropertiesDTO);
         sut = new CreateIndividualPresenter(_view, _subPresenterManager, _dialogCreator, _propertiesMapper, _buildingBlockDTOFactory);
         sut.Initialize();
      }
   }

   public class When_initializing_the_create_individual_presenter : concern_for_CreateIndividualPresenter
   {
      [Observation]
      public void should_tell_the_view_to_render_the_sub_presenter_views()
      {
         A.CallTo(() => _subPresenterManager.InitializeWith(sut,IndividualItems.All)).MustHaveHappened();
      }
   }

   public class When_starting_the_create_individual : concern_for_CreateIndividualPresenter
   {
      protected override void Because()
      {
         sut.Create();
      }

      [Observation]
      public void should_ask_the_view_to_render()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_enable_the_setting_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(IndividualItems.Settings, true)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_parameter_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(IndividualItems.Parameters, false)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_disable_the_expression_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(IndividualItems.Expression, false)).MustHaveHappened();
      }
   }

   public class When_one_sub_presenter_is_notifying_a_dirty_state : concern_for_CreateIndividualPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _individualSettingsPresenter.CanClose).Returns(false);
         A.CallTo(() => _individualParameterPresenter.CanClose).Returns(true);
      }

      protected override void Because()
      {
        // _individualSettingsPresenter.Raise(presenter => presenter.StatusChanged += null);
      }

      [Observation]
      public void the_ok_button_of_the_view_should_be_disabled()
      {
         _view.OkEnabled.ShouldBeFalse();
      }
   }

   public class When_asked_to_create_an_individual : concern_for_CreateIndividualPresenter
   {
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual();
         A.CallTo(() => _subPresenterManager.CanClose).Returns(true);
         A.CallTo(() => _individualSettingsPresenter.Individual).Returns(_individual);      }

      protected override void Because()
      {
         sut.CreateIndividual();
      }

      [Observation]
      public void should_return_an_individual()
      {
         A.CallTo(() => _individualSettingsPresenter.CreateIndividual()).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_individual_parameters_with_the_newly_created_individual()
      {
         A.CallTo(() => _individualParameterPresenter.EditIndividual(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_enzyme_expressions_with_the_newly_created_individual()
      {
         A.CallTo(() => _individualMoleculesPresenter.EditIndividual(_individual)).MustHaveHappened();
      }
   }

   public class When_creating_an_individual_and_the_create_algorithm_cannot_find_an_individual_for_the_given_data : concern_for_CreateIndividualPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _individualSettingsPresenter.Individual).Returns(null);
      }

      protected override void Because()
      {
         sut.CreateIndividual();
      }

      [Observation]
      public void should_create_the_individual()
      {
          A.CallTo(() => _individualSettingsPresenter.CreateIndividual()).MustHaveHappened();
      }

      [Observation]
      public void should_not_update_the_individual_parameters_with_the_newly_created_individual()
      {
        A.CallTo(() => _individualParameterPresenter.EditIndividual(A<Individual>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_update_the_enzyme_expressions_with_the_newly_created_individual()
      {
         A.CallTo(() => _individualMoleculesPresenter.EditIndividual(A<Individual>._)).MustNotHaveHappened();
      }
   }

   public class When_the_user_cancel_the_create_individual_action : concern_for_CreateIndividualPresenter
   {
      private IPKSimCommand _resultOfCreateIndividual;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true); 
      }

      protected override void Because()
      {
         _resultOfCreateIndividual = sut.Create();
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _resultOfCreateIndividual.ShouldBeAnInstanceOf<PKSimEmptyCommand>();
      }
   }

   public class When_user_actions_were_added_to_the_list_of_performed_command : concern_for_CreateIndividualPresenter
   {
      private IPKSimCommand _command1;
      private IPKSimCommand _command2;
      private IPKSimMacroCommand _resultOfCreateIndividual;

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
         _resultOfCreateIndividual = sut.Create() as IPKSimMacroCommand;
      }

      [Observation]
      public void should_return_a_macro_command_containing_all_sub_commands_executed_by_the_user()
      {
         _resultOfCreateIndividual.ShouldNotBeNull();
         _resultOfCreateIndividual.All().ShouldOnlyContainInOrder(_command1, _command2);
      }
   }

  
}