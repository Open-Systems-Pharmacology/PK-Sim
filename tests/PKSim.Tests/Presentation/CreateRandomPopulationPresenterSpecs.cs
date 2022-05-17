using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateRandomPopulationPresenter : ContextSpecificationAsync<ICreateRandomPopulationPresenter>
   {
      protected IObjectBaseDTOFactory _buildingBlockDTOFactory;
      protected ISubPresenterItemManager<IPopulationItemPresenter> _subPresenterManager;
      protected IBuildingBlockPropertiesMapper _propertiesMapper;
      protected ICreateRandomPopulationView _view;
      protected IRandomPopulationSettingsPresenter _popSettingsPresenter;
      protected IPopulationAdvancedParametersPresenter _popAdvancedParameterPresenter;
      protected IPopulationAdvancedParameterDistributionPresenter _popDistributionPresenter;
      private IDialogCreator _dialogCreator;
      private IBuildingBlockRepository _buildingBlockRepository;
      private List<Individual> _allIndividuals;

      protected override Task Context()
      {
         _buildingBlockDTOFactory = A.Fake<IObjectBaseDTOFactory>();
         _subPresenterManager = SubPresenterHelper.Create<IPopulationItemPresenter>();
         _propertiesMapper = A.Fake<IBuildingBlockPropertiesMapper>();
         _view = A.Fake<ICreateRandomPopulationView>();
         _popSettingsPresenter = _subPresenterManager.CreateFake(RandomPopulationItems.Settings);
         _popAdvancedParameterPresenter = _subPresenterManager.CreateFake(RandomPopulationItems.AdvancedParameters);
         _popDistributionPresenter = _subPresenterManager.CreateFake(RandomPopulationItems.ParameterDistribution);
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _allIndividuals = new List<Individual>();
         A.CallTo(() => _buildingBlockRepository.All<Individual>()).Returns(_allIndividuals);
         sut = new CreateRandomPopulationPresenter(_view, _subPresenterManager, _dialogCreator, _propertiesMapper, _buildingBlockDTOFactory, _buildingBlockRepository);
         sut.Initialize();

         return _completed;
      }
   }

   public class When_the_create_population_presenter_is_told_to_create_a_population_that_was_already_created : concern_for_CreateRandomPopulationPresenter
   {
      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _popSettingsPresenter.PopulationCreated).Returns(true);
      }

      protected override Task Because()
      {
         sut.WizardNext(RandomPopulationItems.Settings.Index);
         return _completed;
      }

      [Observation]
      public void should_not_do_anything_since_the_settings_were_not_changed_by_the_user()
      {
         A.CallTo(() => _popSettingsPresenter.CreatePopulation()).MustNotHaveHappened();
      }
   }

   public class When_the_create_population_presenter_is_told_to_create_a_population_after_the_settings_changed : concern_for_CreateRandomPopulationPresenter
   {
      private RandomPopulation _population;

      protected override async Task Context()
      {
         await base.Context();
         _population = A.Fake<RandomPopulation>();
         A.CallTo(() => _popSettingsPresenter.PopulationCreated).Returns(false);
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_population);
      }

      protected override async Task Because()
      {
         await sut.CreatePopulation();
      }

      [Observation]
      public void should_create_a_population()
      {
         A.CallTo(() => _popSettingsPresenter.CreatePopulation()).MustHaveHappened();
      }

      [Observation]
      public void should_disable_the_cancel_button()
      {
         _view.CancelEnabled.ShouldBeEqualTo(false);
      }

      [Observation]
      public void should_disable_the_next_button()
      {
         _view.NextEnabled.ShouldBeEqualTo(false);
      }
   }

   public class When_the_presenter_is_being_notified_that_the_population_was_created_successfuly : concern_for_CreateRandomPopulationPresenter
   {
      private RandomPopulation _population;

      protected override async Task Context()
      {
         await base.Context();
         _population = A.Fake<RandomPopulation>();
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_population);
         _popSettingsPresenter.PopulationCreationFinished += Raise.With(new PopulationCreationEventArgs(true, true));
      }

      [Observation]
      public void should_edit_the_expert_parameters_tab()
      {
         A.CallTo(() => _popAdvancedParameterPresenter.EditPopulation(_population)).MustHaveHappened();
      }

      [Observation]
      public void should_enable_the_cancel_button()
      {
         _view.CancelEnabled.ShouldBeEqualTo(true);
      }

      [Observation]
      public void should_enable_the_next_button()
      {
         _view.NextEnabled.ShouldBeEqualTo(true);
      }
   }

   public class When_creating_a_random_population_based_on_a_given_individual : concern_for_CreateRandomPopulationPresenter
   {
      private Individual _baseIndividual;
      private ObjectBaseDTO _populationDTO;

      protected override async Task Context()
      {
         await base.Context();
         _baseIndividual = A.Fake<Individual>();
         _populationDTO = new ObjectBaseDTO();
         A.CallTo(() => _buildingBlockDTOFactory.CreateFor<Population>()).Returns(_populationDTO);
      }

      protected override Task Because()
      {
         sut.CreatePopulation(_baseIndividual);
         return _completed;
      }

      [Observation]
      public void should_initialize_the_view_with_the_properties_derived_from_the_base_individual()
      {
         A.CallTo(() => _view.BindToProperties(_populationDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_population_settings_based_on_the_individual()
      {
         A.CallTo(() => _popSettingsPresenter.PrepareForCreating(_baseIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_only_enable_the_setting_view()
      {
         A.CallTo(() => _view.SetControlEnabled(RandomPopulationItems.Settings, true)).MustHaveHappened();
         A.CallTo(() => _view.SetControlEnabled(RandomPopulationItems.AdvancedParameters, true)).MustNotHaveHappened();
         A.CallTo(() => _view.SetControlEnabled(RandomPopulationItems.ParameterDistribution, true)).MustNotHaveHappened();
      }
   }

   public class When_the_user_cancels_the_create_population_action : concern_for_CreateRandomPopulationPresenter
   {
      private IPKSimCommand _result;

      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override Task Because()
      {
         _result = sut.Create();
         return _completed;
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _result.IsEmpty().ShouldBeTrue();
      }
   }

   public class When_the_confirms_the_creation_of_a_population : concern_for_CreateRandomPopulationPresenter
   {
      private IPKSimCommand _result;
      private RandomPopulation _randomPopulation;
      private ObjectBaseDTO _buildingBlockDTO;

      protected override async Task Context()
      {
         await base.Context();
         _buildingBlockDTO = new ObjectBaseDTO();
         _randomPopulation = A.Fake<RandomPopulation>();
         A.CallTo(() => _view.Canceled).Returns(false);
         A.CallTo(() => _buildingBlockDTOFactory.CreateFor<Population>()).Returns(_buildingBlockDTO);
         A.CallTo(() => _popSettingsPresenter.Population).Returns(_randomPopulation);
      }

      protected override Task Because()
      {
         _result = sut.Create();
         return _completed;
      }

      [Observation]
      public void should_not_return_an_empty_command()
      {
         _result.IsEmpty().ShouldBeFalse();
      }

      [Observation]
      public void should_set_the_population_properties_into_the_created_population()
      {
         A.CallTo(() => _propertiesMapper.MapProperties(_buildingBlockDTO, _randomPopulation)).MustHaveHappened();
      }
   }

   public class When_the_user_selects_the_next_button_and_moves_from_the_setting_tabs_to_the_advanced_parameter_tab : concern_for_CreateRandomPopulationPresenter
   {
      protected override Task Because()
      {
         sut.WizardNext(RandomPopulationItems.Settings.Index);
         return _completed;
      }

      [Observation]
      public void should_create_the_population()
      {
         A.CallTo(() => _popSettingsPresenter.CreatePopulation()).MustHaveHappened();
      }
   }

   public class When_one_sub_presenter_of_the_create_random_population_presenter_notifies_a_status_change_and_no_error_is_found_in_any_sub_presenter : concern_for_CreateRandomPopulationPresenter
   {
      protected override async Task Context()
      {
         await base.Context();

         //pop was created and all presenter have no error
         A.CallTo(() => _popSettingsPresenter.CanClose).Returns(true);
         A.CallTo(() => _subPresenterManager.CanClose).Returns(true);
         A.CallTo(() => _popSettingsPresenter.PopulationCreated).Returns(true);
         A.CallTo(() => _view.HasError).Returns(false);
      }

      protected override Task Because()
      {
         _popSettingsPresenter.StatusChanged += Raise.WithEmpty();
         return _completed;
      }

      [Observation]
      public void should_enable_the_next_button()
      {
         _view.NextEnabled.ShouldBeTrue();
      }

      [Observation]
      public void should_enable_the_ok_button()
      {
         _view.OkEnabled.ShouldBeTrue();
      }
   }

   public class When_one_sub_presenter_of_the_create_random_population_presenter_notifies_a_status_change_and_one_error_is_found_in_at_least_one_sub_presenter : concern_for_CreateRandomPopulationPresenter
   {
      protected override async Task Context()
      {
         await base.Context();

         A.CallTo(() => _subPresenterManager.CanClose).Returns(false);
         A.CallTo(() => _view.HasError).Returns(false);
      }

      protected override Task Because()
      {
         _popSettingsPresenter.StatusChanged += Raise.WithEmpty();
         return _completed;
      }

      [Observation]
      public void should_disable_the_next_button()
      {
         _view.NextEnabled.ShouldBeFalse();
      }

      [Observation]
      public void should_disable_the_ok_button()
      {
         _view.OkEnabled.ShouldBeFalse();
      }
   }

   public class When_the_create_population_presenter_is_being_notified_a_status_changed_and_a_population_was_created : concern_for_CreateRandomPopulationPresenter
   {
      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _popSettingsPresenter.PopulationCreated).Returns(true);
      }

      protected override Task Because()
      {
         _popSettingsPresenter.StatusChanged += Raise.WithEmpty();
         return _completed;
      }

      [Observation]
      public void should_enable_the_advanced_parameter_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(RandomPopulationItems.AdvancedParameters, true)).MustHaveHappened();
      }

      [Observation]
      public void should_enable_the_distribution_parameter_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(RandomPopulationItems.ParameterDistribution, true)).MustHaveHappened();
      }
   }

   public class When_the_create_population_presenter_is_being_notified_a_status_changed_and_no_population_was_created_yet : concern_for_CreateRandomPopulationPresenter
   {
      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _popSettingsPresenter.PopulationCreated).Returns(false);
      }

      protected override Task Because()
      {
         _popSettingsPresenter.StatusChanged += Raise.WithEmpty();
         return _completed;
      }

      [Observation]
      public void should_disable_the_advanced_parameter_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(RandomPopulationItems.AdvancedParameters, false)).MustHaveHappened();
      }

      [Observation]
      public void should_disable_the_distribution_parameter_tab()
      {
         A.CallTo(() => _view.SetControlEnabled(RandomPopulationItems.ParameterDistribution, false)).MustHaveHappened();
      }
   }
}