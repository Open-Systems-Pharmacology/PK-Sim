using System;
using System.Collections.Generic;
using System.Threading;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;

namespace PKSim.Presentation
{
   public abstract class concern_for_RandomPopulationSettingsPresenter : ContextSpecification<IRandomPopulationSettingsPresenter>
   {
      protected IRandomPopulationSettingsView _view;
      protected IRandomPopulationFactory _randomPopulationFactory;
      protected IPopulationSettingsDTOMapper _populationSettingsMapper;
      protected ILazyLoadTask _lazyLoadTask;

      protected override void Context()
      {
         _view = A.Fake<IRandomPopulationSettingsView>();
         _populationSettingsMapper = A.Fake<IPopulationSettingsDTOMapper>();
         _randomPopulationFactory = A.Fake<IRandomPopulationFactory>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         sut = new RandomPopulationSettingsPresenter(_view, _randomPopulationFactory, _populationSettingsMapper, _lazyLoadTask);
      }
   }

   public class When_the_population_settings_presenter_is_being_started_based_on_a_given_individual : concern_for_RandomPopulationSettingsPresenter
   {
      private Individual _baseIndividual;
      private PopulationSettingsDTO _populationSettingsDTO;

      protected override void Context()
      {
         base.Context();
         _baseIndividual = A.Fake<Individual>();
         _populationSettingsDTO = new PopulationSettingsDTO();
         A.CallTo(() => _populationSettingsMapper.MapFrom(_baseIndividual)).Returns(_populationSettingsDTO);
      }

      protected override void Because()
      {
         sut.PrepareForCreating(_baseIndividual);
      }

      [Observation]
      public void should_load_the_individual()
      {
         A.CallTo(() => _lazyLoadTask.Load(_baseIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_the_default_population_settings_based_on_the_individual_and_update_the_view()
      {
         A.CallTo(() => _view.BindTo(_populationSettingsDTO)).MustHaveHappened();
      }
   }

   public class When_the_population_settings_presenter_is_creating_a_population : concern_for_RandomPopulationSettingsPresenter
   {
      private RandomPopulationSettings _populationSettings;

      protected override void Context()
      {
         base.Context();
         _populationSettings = new RandomPopulationSettings();
         A.CallTo(_populationSettingsMapper).WithReturnType<RandomPopulationSettings>().Returns(_populationSettings);
         sut.PrepareForCreating(A.Fake<Individual>());
      }

      protected override void Because()
      {
         sut.CreatePopulation();
      }

      [Observation]
      public void should_leverage_the_population_factory_to_create_a_population_base_on_the_settings()
      {
         A.CallTo(() => _randomPopulationFactory.CreateFor(_populationSettings, A<CancellationToken>._, null, true)).MustHaveHappened();
      }

      [Observation]
      public void should_have_set_its_population_created_flag_to_true()
      {
         sut.PopulationCreated.ShouldBeTrue();
      }
   }

   public class When_the_user_stops_the_creation_of_the_population : concern_for_RandomPopulationSettingsPresenter
   {
      private RandomPopulationSettings _populationSettings;

      protected override void Context()
      {
         base.Context();
         _populationSettings = new RandomPopulationSettings();
         A.CallTo(_populationSettingsMapper).WithReturnType<RandomPopulationSettings>().Returns(_populationSettings);
         A.CallTo(() => _randomPopulationFactory.CreateFor(_populationSettings, A<CancellationToken>._, null, true)).Throws<OperationCanceledException>();
         sut.PrepareForCreating(A.Fake<Individual>());
         sut.CreatePopulation();
      }

      protected override void Because()
      {
         sut.Cancel();
      }

      [Observation]
      public void should_have_the_population_created_set_to_false()
      {
         sut.PopulationCreated.ShouldBeFalse();
      }
   }

   public class When_the_population_settings_presenter_is_being_notified_that_the_selected_individual_has_changed : concern_for_RandomPopulationSettingsPresenter
   {
      private PopulationSettingsDTO _populationSettingsDTO;
      private Individual _newIndividual;
      private Individual _oldIndividual;
      private PopulationSettingsDTO _newPopulationSettingsDTO;

      protected override void Context()
      {
         base.Context();
         _populationSettingsDTO = new PopulationSettingsDTO();
         _newPopulationSettingsDTO = new PopulationSettingsDTO();
         _oldIndividual = A.Fake<Individual>();
         A.CallTo(() => _populationSettingsMapper.MapFrom(_oldIndividual)).Returns(_populationSettingsDTO);
         A.CallTo(() => _populationSettingsMapper.MapFrom(_newIndividual)).Returns(_newPopulationSettingsDTO);
         _newIndividual = A.Fake<Individual>();
         A.CallTo(() => _newIndividual.AvailableGenders()).Returns(new List<Gender>());
         //first initialize with the old individual
         sut.PrepareForCreating(_oldIndividual);
      }

      protected override void Because()
      {
         sut.IndividualSelectionChanged(_newIndividual);
      }

      [Observation]
      public void should_require_the_creation_of_a_new_population()
      {
         sut.PopulationCreated.ShouldBeFalse();
      }

      [Observation]
      public void should_load_the_newly_selecetd_individual()
      {
         A.CallTo(() => _lazyLoadTask.Load(_newIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_display_default_population_settings_for_the_selected_individual()
      {
         A.CallTo(() => _populationSettingsMapper.MapFrom(_newIndividual)).MustHaveHappened();
         A.CallTo(() => _view.BindTo(_populationSettingsDTO)).MustHaveHappened();
      }
   }

   public class When_the_population_setting_presenter_is_being_notified_that_the_view_has_changed : concern_for_RandomPopulationSettingsPresenter
   {
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         sut.StatusChanged += (o, e) => _eventRaised = true;
      }

      protected override void Because()
      {
         sut.ViewChanged();
      }

      [Observation]
      public void should_set_the_population_created_flag_to_false()
      {
         sut.PopulationCreated.ShouldBeFalse();
      }

      [Observation]
      public void should_notify_the_status_change_event()
      {
         _eventRaised.ShouldBeTrue();
      }
   }

   public class When_the_population_setting_presenter_is_editing_a_population : concern_for_RandomPopulationSettingsPresenter
   {
      private PopulationSettingsDTO _populationSettingsDTO;
      private RandomPopulation _randomPopulation;

      protected override void Context()
      {
         base.Context();
         _randomPopulation = A.Fake<RandomPopulation>();
         _randomPopulation.Settings = new RandomPopulationSettings();
         _populationSettingsDTO = new PopulationSettingsDTO();
         A.CallTo(() => _populationSettingsMapper.MapFrom(_randomPopulation.Settings)).Returns(_populationSettingsDTO);
      }

      protected override void Because()
      {
         sut.LoadPopulation(_randomPopulation);
      }

      [Observation]
      public void should_load_the_settings_from_the_popualtion_and_display_them_in_the_view()
      {
         A.CallTo(() => _view.BindTo(_populationSettingsDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_prepare_the_view_for_the_edit_layout()
      {
         A.CallTo(() => _view.UpdateLayoutForEditing()).MustHaveHappened();
      }

      [Observation]
      public void should_return_that_the_population_was_already_created()
      {
         sut.PopulationCreated.ShouldBeTrue();
      }
   }
}