using System;
using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.DiseaseStates;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using DiseaseState = PKSim.Core.Model.DiseaseState;
using Individual = PKSim.Core.Model.Individual;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualSettingsPresenter : ContextSpecification<IIndividualSettingsPresenter>
   {
      protected ISpeciesRepository _speciesRepository;
      protected IIndividualSettingsView _view;
      protected IndividualSettingsDTO _individualSettingsDTO;
      protected IIndividualPresenter _parentPresenter;
      protected IIndividualToIIndividualSettingsDTOMapper _individualSettingsDTOMapper;
      protected IIndividualDefaultValueUpdater _defaultValueUpdater;
      protected SpeciesPopulation _speciesPopulation;
      protected IList<Gender> _allGenders;
      protected Species _species;
      protected Gender _gender;
      protected IIndividualSettingsDTOToIndividualMapper _individualMapper;
      protected ObjectBaseDTO _individualPropertiesDTO;
      private ICalculationMethodCategoryRepository _calculationMethodRepository;
      private IEnumerable<CategoryParameterValueVersionDTO> _subPopulation;
      protected CalculationMethodCategory _cmCat1;
      protected CalculationMethodCategory _cmCat2;
      private IEditValueOriginPresenter _editValueOriginPresenter;
      protected IDefaultIndividualRetriever _defaultIndividualRetriever;
      protected Individual _defaultIndividual;
      protected IDiseaseStateRepository _diseaseStateRepository;
      protected IDiseaseStateSelectionPresenter _diseaseStateSelectionPresenter;
      private ICloner _cloner;
      private Individual _clonedIndividual;

      protected override void Context()
      {
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _view = A.Fake<IIndividualSettingsView>();
         _defaultValueUpdater = A.Fake<IIndividualDefaultValueUpdater>();
         _individualSettingsDTOMapper = A.Fake<IIndividualToIIndividualSettingsDTOMapper>();
         _individualMapper = A.Fake<IIndividualSettingsDTOToIndividualMapper>();
         _calculationMethodRepository = A.Fake<ICalculationMethodCategoryRepository>();
         _subPopulation = A.Fake<IEnumerable<CategoryParameterValueVersionDTO>>();
         _editValueOriginPresenter = A.Fake<IEditValueOriginPresenter>();
         _defaultIndividualRetriever = A.Fake<IDefaultIndividualRetriever>();
         _diseaseStateRepository = A.Fake<IDiseaseStateRepository>();
         _diseaseStateSelectionPresenter = A.Fake<IDiseaseStateSelectionPresenter>();
         _cloner= A.Fake<ICloner>();
         _individualSettingsDTO = new IndividualSettingsDTO();
         _individualPropertiesDTO = new ObjectBaseDTO();
         _defaultIndividual = new Individual();
         _clonedIndividual = new Individual();  
         _speciesPopulation = A.Fake<SpeciesPopulation>();
         _species = A.Fake<Species>();
         _gender = A.Fake<Gender>();
         _cmCat1 = new CalculationMethodCategory();
         _cmCat2 = new CalculationMethodCategory();
         _cmCat1.Add(new CalculationMethod());
         _cmCat2.Add(new CalculationMethod());
         _individualSettingsDTO.Species = _species;
         _individualSettingsDTO.Population = _speciesPopulation;
         _individualSettingsDTO.Gender = _gender;

         A.CallTo(() => _defaultIndividualRetriever.DefaultIndividual()).Returns(_defaultIndividual);
         A.CallTo(() => _cloner.Clone(_defaultIndividual)).Returns(_clonedIndividual);
         A.CallTo(() => _individualSettingsDTOMapper.MapFrom(_clonedIndividual)).Returns(_individualSettingsDTO);
         A.CallTo(() => _calculationMethodRepository.All()).Returns(new[] {_cmCat1, _cmCat2});
         _individualSettingsDTO.SubPopulation = _subPopulation;
         _parentPresenter = A.Fake<IIndividualPresenter>();

         sut = new IndividualSettingsPresenter(
            _view,
            _speciesRepository,
            _calculationMethodRepository,
            _defaultIndividualRetriever,
            _defaultValueUpdater,
            _individualSettingsDTOMapper,
            _individualMapper,
            _editValueOriginPresenter,
            _diseaseStateSelectionPresenter,
            _diseaseStateRepository,
            _cloner);
         sut.InitializeWith(_parentPresenter);
      }
   }

   public class When_the_individual_presenter_is_told_to_prepare_for_individual_creation : concern_for_IndividualSettingsPresenter
   {
      protected override void Because()
      {
         sut.PrepareForCreating();
      }

      [Observation]
      public void should_ask_the_view_to_bind_to_the_individual_dto_object()
      {
         A.CallTo(() => _view.BindToSettings(_individualSettingsDTO)).MustHaveHappened();
         A.CallTo(() => _view.BindToParameters(_individualSettingsDTO)).MustHaveHappened();
         A.CallTo(() => _view.BindToSubPopulation(_individualSettingsDTO.SubPopulation)).MustHaveHappened();
      }
   }

   public class When_retrieving_the_list_of_all_species : concern_for_IndividualSettingsPresenter
   {
      private IEnumerable<Species> _results;
      private Species _species1;
      private Species _species2;

      protected override void Context()
      {
         base.Context();
         _species1 = new Species();
         _species2 = new Species();
         A.CallTo(() => _speciesRepository.All()).Returns(new[] {_species1, _species2});
      }

      protected override void Because()
      {
         _results = sut.AllSpecies();
      }

      [Observation]
      public void should_leverage_the_individual_task_to_retrieve_all_available_species()
      {
         A.CallTo(() => _speciesRepository.All()).MustHaveHappened();
      }

      [Observation]
      public void should_return_the_available_species_ids()
      {
         _results.ShouldOnlyContainInOrder(_species1, _species2);
      }
   }

   public class When_retrieving_the_list_of_all_population_for_a_given_species : concern_for_IndividualSettingsPresenter
   {
      private SpeciesPopulation _pop1;
      private SpeciesPopulation _pop2;

      protected override void Context()
      {
         base.Context();
         _pop1 = A.Fake<SpeciesPopulation>();
         _pop2 = A.Fake<SpeciesPopulation>();
         _species = A.Fake<Species>();
         A.CallTo(() => _species.Populations).Returns(new[] {_pop1, _pop2});
      }

      [Observation]
      public void should_return_the_available_population()
      {
         sut.PopulationsFor(_species).ShouldOnlyContainInOrder(_pop1, _pop2);
      }
   }

   public class When_retrieving_the_list_of_all_gender_for_a_given_population : concern_for_IndividualSettingsPresenter
   {
      private SpeciesPopulation _pop;
      private Gender _gender1;
      private Gender _gender2;

      protected override void Context()
      {
         base.Context();
         _pop = A.Fake<SpeciesPopulation>();
         _gender1 = A.Fake<Gender>();
         _gender2 = A.Fake<Gender>();
         A.CallTo(() => _pop.Genders).Returns(new[] {_gender1, _gender2});
      }

      [Observation]
      public void should_return_the_available_genders()
      {
         sut.GenderFor(_pop).ShouldOnlyContainInOrder(_gender1, _gender2);
      }
   }

   public class When_retrieving_the_list_of_all_parameter_value_version_for_a_given_category : concern_for_IndividualSettingsPresenter
   {
      private ParameterValueVersion _pvv1;
      private ParameterValueVersion _pvv2;
      private ParameterValueVersionCategory _category;

      protected override void Context()
      {
         base.Context();
         _category = new ParameterValueVersionCategory();
         _pvv1 = new ParameterValueVersion();
         _pvv2 = new ParameterValueVersion();
         _category.Name = "toto";
         _category.Add(_pvv1);
         _category.Add(_pvv2);
         _individualSettingsDTO.Species = _species;
         A.CallTo(() => _species.PVVCategoryByName(_category.Name)).Returns(_category);
         sut.PrepareForCreating();
      }

      [Observation]
      public void should_return_all_parameter_value_version_for_this_category()
      {
         sut.AllParameterValueVersionsFor(_category.Name).ShouldOnlyContainInOrder(_pvv1, _pvv2);
      }
   }

   public abstract class When_notified_that_the_current_population_has_changed : concern_for_IndividualSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();

         sut.PrepareForCreating();
      }

      protected override void Because()
      {
         sut.PopulationChanged();
      }
   }

   public class When_the_underlying_view_is_dirty : concern_for_IndividualSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.HasError).Returns(true);
      }

      [Observation]
      public void should_not_be_able_to_close()
      {
         sut.CanClose.ShouldBeFalse();
      }
   }

   public class When_the_current_population_is_age_dependent : When_notified_that_the_current_population_has_changed
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.IsAgeDependent = true;
      }

      [Observation]
      public void should_tell_the_view_to_display_the_age_dependent_controls()
      {
         _view.AgeVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_tell_the_view_to_refresh_all_individual_lists()
      {
         A.CallTo(() => _view.RefreshAllIndividualList()).MustHaveHappened();
      }
   }

   public class When_the_current_population_is_age_independent : When_notified_that_the_current_population_has_changed
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.IsAgeDependent = false;
      }

      [Observation]
      public void should_tell_the_view_to_hide_the_age_dependent_controls()
      {
         _view.AgeVisible.ShouldBeFalse();
      }
   }

   public class When_the_current_population_is_height_dependent : When_notified_that_the_current_population_has_changed
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.IsHeightDependent = true;
      }

      [Observation]
      public void should_tell_the_view_to_display_the_height_dependent_controls()
      {
         _view.HeightAndBMIVisible.ShouldBeTrue();
      }
   }

   public class When_the_current_population_is_height_independent : When_notified_that_the_current_population_has_changed
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.IsHeightDependent = false;
      }

      [Observation]
      public void should_tell_the_view_to_hide_the_height_dependent_controls()
      {
         _view.HeightAndBMIVisible.ShouldBeFalse();
      }
   }

   public class When_editing_the_individual_settings_for_an_individual : concern_for_IndividualSettingsPresenter
   {
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _individualSettingsDTO.Species = _species;
         _individualSettingsDTO.Population = _speciesPopulation;
         _individualSettingsDTO.Gender = _gender;
         A.CallTo(() => _individualSettingsDTOMapper.MapFrom(_individual)).Returns(_individualSettingsDTO);
      }

      protected override void Because()
      {
         sut.EditIndividual(_individual);
      }

      [Observation]
      public void should_update_the_view_individual_settings()
      {
         A.CallTo(() => _view.BindToSettings(_individualSettingsDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_view_individual_parameters()
      {
         A.CallTo(() => _view.BindToParameters(_individualSettingsDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_view_as_read_only()
      {
         _view.IsReadOnly.ShouldBeTrue();
      }
   }

   public class When_being_notified_of_a_change_in_the_view : concern_for_IndividualSettingsPresenter
   {
      private bool _statusChangeWasRaised;

      protected override void Context()
      {
         base.Context();
         sut.StatusChanged += (o, e) => { _statusChangeWasRaised = true; };
         A.CallTo(() => _view.HasError).Returns(true);
      }

      protected override void Because()
      {
         sut.ViewChanged();
      }

      [Observation]
      public void should_notifiy_a_status_change()
      {
         _statusChangeWasRaised.ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_dirty_state_to_true()
      {
         sut.CanClose.ShouldBeFalse();
      }
   }

   public class When_the_user_changed_the_selected_species : concern_for_IndividualSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.PrepareForCreating();
      }

      protected override void Because()
      {
         sut.SpeciesChanged();
      }

      [Observation]
      public void should_refresh_all_individual_properties()
      {
         A.CallTo(() => _view.RefreshAllIndividualList()).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_default_population_and_gender_for_that_species()
      {
         A.CallTo(() => _defaultValueUpdater.UpdateSettingsAfterSpeciesChange(_individualSettingsDTO)).MustHaveHappened();
      }
   }

   public class When_the_user_changed_the_selected_gender : concern_for_IndividualSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.PrepareForCreating();
      }

      protected override void Because()
      {
         sut.GenderChanged();
      }

      [Observation]
      public void should_refresh_all_individual_properties()
      {
         A.CallTo(() => _view.RefreshAllIndividualList()).MustHaveHappened();
      }
   }

   public class When_the_view_has_a_dirty_state : concern_for_IndividualSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.HasError).Returns(true);
      }

      [Observation]
      public void should_not_be_able_to_close()
      {
         sut.CanClose.ShouldBeFalse();
      }
   }

   public class When_the_individual_setting_presenter_is_asked_for_the_its_view : concern_for_IndividualSettingsPresenter
   {
      [Observation]
      public void should_return_the_view()
      {
         sut.BaseView.ShouldBeEqualTo(_view);
      }
   }

   public class When_the_individual_presenter_is_told_to_prepare_for_individual_scaling : concern_for_IndividualSettingsPresenter
   {
      private Individual _individualToScale;

      protected override void Context()
      {
         base.Context();
         _individualToScale = A.Fake<Individual>();
         A.CallTo(() => _individualSettingsDTOMapper.MapFrom(_individualToScale)).Returns(_individualSettingsDTO);
         _individualSettingsDTO.Species = _species;
         _individualSettingsDTO.Population = _speciesPopulation;
         _individualSettingsDTO.Gender = _gender;
      }

      protected override void Because()
      {
         sut.PrepareForScaling(_individualToScale);
      }

      [Observation]
      public void should_tell_the_view_to_display_the_individual_properties()
      {
         A.CallTo(() => _view.BindToSettings(_individualSettingsDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_display_the_individual_parameters()
      {
         A.CallTo(() => _view.BindToParameters(_individualSettingsDTO)).MustHaveHappened();
      }
   }

   public class When_the_individual_setting_presenter_is_asked_if_a_category_should_be_displayed : concern_for_IndividualSettingsPresenter
   {
      private ParameterValueVersionCategory _categoryWithPvvs;
      private ParameterValueVersionCategory _categoryWithOnlyOnePvv;

      protected override void Context()
      {
         base.Context();
         sut.PrepareForCreating();
         _individualSettingsDTO.Species = _species;
         _categoryWithPvvs = new ParameterValueVersionCategory();
         _categoryWithOnlyOnePvv = new ParameterValueVersionCategory();
         _categoryWithPvvs.Name = "categoryWithPvv";
         _categoryWithOnlyOnePvv.Name = "categoryWith";
         _categoryWithPvvs.Add(new ParameterValueVersion());
         _categoryWithPvvs.Add(new ParameterValueVersion());
         _categoryWithOnlyOnePvv.Add(new ParameterValueVersion());
         A.CallTo(() => _species.PVVCategoryByName(_categoryWithPvvs.Name)).Returns(_categoryWithPvvs);
         A.CallTo(() => _species.PVVCategoryByName(_categoryWithOnlyOnePvv.Name)).Returns(_categoryWithOnlyOnePvv);
      }

      [Observation]
      public void should_return_true_if_the_category_contains_more_than_one_parameter_value_version()
      {
         sut.ShouldDisplayPvvCategory(_categoryWithPvvs.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_category_with_only_one_parameter_value_version()
      {
         sut.ShouldDisplayPvvCategory(_categoryWithOnlyOnePvv.Name).ShouldBeFalse();
      }
   }

   public class When_retrieving_the_list_of_all_disease_states_defined_for_a_population_with_disease_states : concern_for_IndividualSettingsPresenter
   {
      private SpeciesPopulation _population;
      private DiseaseState _diseaseState1;
      private DiseaseState _diseaseState2;
      private DiseaseState _healthyDiseaseState;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _population = new SpeciesPopulation();
         _diseaseState1 = new DiseaseState();
         _diseaseState2 = new DiseaseState();
         _healthyDiseaseState = new DiseaseState();
         A.CallTo(() => _diseaseStateRepository.AllFor(_population)).Returns(new[] {_diseaseState1, _diseaseState2});
         A.CallTo(() => _diseaseStateRepository.HealthyState).Returns(_healthyDiseaseState);

         _individual = A.Fake<Individual>();
         _individualSettingsDTO.Population = _population;
         A.CallTo(() => _individualSettingsDTOMapper.MapFrom(_individual)).Returns(_individualSettingsDTO);

         sut.EditIndividual(_individual);
      }

      [Observation]
      public void should_return_all_disease_states_defined_for_the_population_with_the_healthy_state_first()
      {
         _diseaseStateSelectionPresenter.AllDiseaseStates.ShouldOnlyContainInOrder(_healthyDiseaseState, _diseaseState1, _diseaseState2);
      }
   }

   public class When_retrieving_the_list_of_all_disease_states_defined_for_a_population_without_disease_states : concern_for_IndividualSettingsPresenter
   {
      private SpeciesPopulation _population;
      private DiseaseState _healthyDiseaseState;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _population = new SpeciesPopulation();
         _healthyDiseaseState = new DiseaseState();
         A.CallTo(() => _diseaseStateRepository.AllFor(_population)).Returns(Array.Empty<DiseaseState>());
         A.CallTo(() => _diseaseStateRepository.HealthyState).Returns(_healthyDiseaseState);
         _individual = A.Fake<Individual>();
         _individualSettingsDTO.Population = _population;
         A.CallTo(() => _individualSettingsDTOMapper.MapFrom(_individual)).Returns(_individualSettingsDTO);

         sut.EditIndividual(_individual);
      }

      [Observation]
      public void should_return_the_healthy_state()
      {
         _diseaseStateSelectionPresenter.AllDiseaseStates.ShouldOnlyContain(new []{_healthyDiseaseState});
      }
   }

   public class When_notified_that_the_selected_population_has_changed : concern_for_IndividualSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.PrepareForCreating();
      }

      protected override void Because()
      {
         sut.PopulationChanged();
      }

      [Observation]
      public void should_update_the_disease_state_settings_in_the_dto()
      {
         A.CallTo(() => _diseaseStateSelectionPresenter.ResetDiseaseState()).MustHaveHappened();
      }
   }
}