using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Core;

using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualPresenterSettings : ContextSpecification<IIndividualSettingsPresenter>
   {
      protected ISpeciesRepository _speciesRepository;
      protected IIndividualSettingsView _view;
      protected IndividualSettingsDTO _individualSettingsDTO;
      protected IIndividualPresenter _parentPresenter;
      protected IIndividualToIIndividualSettingsDTOMapper _individualSettingsDTOMapper;
      protected IIndividualDefaultValueRetriever _defaultValueRetriever;
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

      protected override void Context()
      {
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _view = A.Fake<IIndividualSettingsView>();
         _defaultValueRetriever = A.Fake<IIndividualDefaultValueRetriever>();
         _individualSettingsDTOMapper = A.Fake<IIndividualToIIndividualSettingsDTOMapper>();
         _individualMapper = A.Fake<IIndividualSettingsDTOToIndividualMapper>();
         _calculationMethodRepository = A.Fake<ICalculationMethodCategoryRepository>();
         _subPopulation = A.Fake<IEnumerable<CategoryParameterValueVersionDTO>>();
         _editValueOriginPresenter= A.Fake<IEditValueOriginPresenter>();
         _individualSettingsDTO = new IndividualSettingsDTO();
         _individualPropertiesDTO = new ObjectBaseDTO();

         _speciesPopulation = A.Fake<SpeciesPopulation>();
         _species = A.Fake<Species>();
         _gender = A.Fake<Gender>();
         _cmCat1 = new CalculationMethodCategory();
         _cmCat2 = new CalculationMethodCategory();
         _cmCat1.Add(new CalculationMethod());
         _cmCat2.Add(new CalculationMethod());
         _individualSettingsDTO.Species = _species;
         _individualSettingsDTO.SpeciesPopulation = _speciesPopulation;
         _individualSettingsDTO.Gender = _gender;

         A.CallTo(() => _defaultValueRetriever.DefaultSettings()).Returns(_individualSettingsDTO);

         A.CallTo(() => _calculationMethodRepository.All()).Returns(new[] {_cmCat1, _cmCat2});
         _individualSettingsDTO.SubPopulation = _subPopulation;
         _parentPresenter = A.Fake<IIndividualPresenter>();
         sut = new IndividualSettingsPresenter(_view, _speciesRepository, _calculationMethodRepository, _defaultValueRetriever,
                                               _individualSettingsDTOMapper, _individualMapper, _editValueOriginPresenter);
         sut.InitializeWith(_parentPresenter);
      }
   }

   public class When_the_individual_presenter_is_told_to_prepare_for_individual_creation : concern_for_IndividualPresenterSettings
   {
      protected override void Because()
      {
         sut.PrepareForCreating();
      }

      [Observation]
      public void should_ask_the_view_to_bind_to_the_individual_dto_object()
      {
         A.CallTo(() => _view.BindToSettings(_individualSettingsDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_the_default_values()
      {
         A.CallTo(() => _defaultValueRetriever.DefaultSettings()).MustHaveHappened();
      }
   }

   public class When_retrieving_the_list_of_all_species : concern_for_IndividualPresenterSettings
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

   public class When_retrieving_the_list_of_all_population_for_a_given_species : concern_for_IndividualPresenterSettings
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

   public class When_retrieving_the_list_of_all_gender_for_a_given_population : concern_for_IndividualPresenterSettings
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

   public class When_retrieving_the_list_of_all_parameter_value_version_for_a_given_category : concern_for_IndividualPresenterSettings
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

   public abstract class When_notified_that_the_current_population_has_changed : concern_for_IndividualPresenterSettings
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

   public class When_the_underlying_view_is_dirty : concern_for_IndividualPresenterSettings
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

   public class When_editing_the_individual_settings_for_an_individual : concern_for_IndividualPresenterSettings
   {
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _individualSettingsDTO.Species = _species;
         _individualSettingsDTO.SpeciesPopulation = _speciesPopulation;
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

   public class When_being_notified_of_a_change_in_the_view : concern_for_IndividualPresenterSettings
   {
      private bool _statusChangeWasRaised;

      protected override void Context()
      {
         base.Context();
         sut.StatusChanged += (o,e) => { _statusChangeWasRaised = true; };
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

   public class When_the_user_changed_the_selected_species : concern_for_IndividualPresenterSettings
   {
      protected override void Context()
      {
         base.Context();
         sut.PrepareForCreating();
         A.CallTo(() => _defaultValueRetriever.DefaultPopulationFor(_individualSettingsDTO.Species)).Returns(_speciesPopulation);
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
      public void should_updatte_the_default_population_and_gender_for_that_species()
      {
         A.CallTo(() => _defaultValueRetriever.UpdateSettingsAfterSpeciesChange(_individualSettingsDTO)).MustHaveHappened();
      }
   }

   public class When_the_user_changed_the_selected_gender : concern_for_IndividualPresenterSettings
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

   public class When_the_view_has_a_dirty_state : concern_for_IndividualPresenterSettings
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

   public class When_the_individual_setting_presenter_is_asked_for_the_its_view : concern_for_IndividualPresenterSettings
   {
      [Observation]
      public void should_return_the_view()
      {
         sut.BaseView.ShouldBeEqualTo(_view);
      }
   }

   public class When_the_individual_presenter_is_told_to_prepare_for_individual_scaling : concern_for_IndividualPresenterSettings
   {
      private Individual _individualToScale;

      protected override void Context()
      {
         base.Context();
         _individualToScale = A.Fake<Individual>();
         A.CallTo(() => _individualSettingsDTOMapper.MapFrom(_individualToScale)).Returns(_individualSettingsDTO);
         _individualSettingsDTO.Species = _species;
         _individualSettingsDTO.SpeciesPopulation = _speciesPopulation;
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

   public class When_the_individual_setting_presetner_is_asked_if_a_category_should_be_displayed : concern_for_IndividualPresenterSettings
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
}