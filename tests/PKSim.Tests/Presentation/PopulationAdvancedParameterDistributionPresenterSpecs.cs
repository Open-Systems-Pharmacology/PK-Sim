using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAdvancedParameterDistributionPresenter : ContextSpecification<IPopulationAdvancedParameterDistributionPresenter>
   {
      protected IAdvancedParameterDistributionView _view;
      protected Population _population;
      protected List<IParameter> _allParameters;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IEntityPathResolver _entityPathResolver;
      protected DistributionSettings _settings;
      protected IPopulationParameterGroupsPresenter _parametersPresenter;
      protected Gender _gender1;
      protected Gender _gender2;
      protected IPopulationDistributionPresenter _popParameterDistributionPresenter;
      private PathCache<IParameter> _allParametersCache;
      protected IProjectChangedNotifier _projectChangedNotifier;

      protected override void Context()
      {
         _view = A.Fake<IAdvancedParameterDistributionView>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parametersPresenter = A.Fake<IPopulationParameterGroupsPresenter>();
         _popParameterDistributionPresenter = A.Fake<IPopulationDistributionPresenter>();
         _population = A.Fake<Population>();
         _projectChangedNotifier = A.Fake<IProjectChangedNotifier>();

         _gender1 = A.Fake<Gender>().WithName("Gender1");
         _gender2 = A.Fake<Gender>().WithName("Gender2");
         A.CallTo(() => _population.AllGenders).Returns(new[] {_gender1, _gender2});
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_gender1)).Returns("Display1");
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_gender2)).Returns("Display2");


         _allParameters = new List<IParameter>();
         _allParametersCache = new PathCacheForSpecs<IParameter>();
         A.CallTo(() => _population.AllVectorialParameters(_entityPathResolver)).Returns(_allParameters);
         A.CallTo(() => _population.AllParameters(_entityPathResolver)).Returns(_allParametersCache);
         A.CallTo(() => _popParameterDistributionPresenter.Plot(_population, A<IParameter>._, A<DistributionSettings>._, A<IDimension>._, A<Unit>._))
            .Invokes(x => _settings = x.GetArgument<DistributionSettings>(2));


         sut = new PopulationAdvancedParameterDistributionPresenter(_view, _parametersPresenter,
            _representationInfoRepository, _entityPathResolver, _popParameterDistributionPresenter, _projectChangedNotifier);
      }
   }

   public class When_creating_the_population_parameter_distribution : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      [Observation]
      public void should_set_the_parameter_group_view_into_the_distribution_view()
      {
         A.CallTo(() => _view.UpdateParameterGroupView(_parametersPresenter.View)).MustHaveHappened();
      }
   }

   public class When_the_population_distribution_presenter_is_told_to_edit_a_population : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      private IParameter _p1;
      private IParameter _p2;
      private IParameter _p3;

      protected override void Context()
      {
         base.Context();
         _p1 = A.Fake<IParameter>();
         _p2 = A.Fake<IParameter>();
         _p3 = A.Fake<IParameter>();
         _allParameters.AddRange(new[] {_p1, _p2, _p3});
         A.CallTo(() => _population.AllGenders).Returns(new List<Gender>());
      }

      protected override void Because()
      {
         sut.EditPopulation(_population);
      }

      [Observation]
      public void should_retrieve_all_parameters_defined_in_that_simulation_and_initialize_the_group_display()
      {
         A.CallTo(() => _parametersPresenter.AddParameters(A<IEnumerable<IParameter>>._, true)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_plot_options_into_the_view()
      {
         A.CallTo(() => _view.BindToPlotOptions(A<DistributionSettings>._)).MustHaveHappened();
      }
   }

   public class When_adding_an_advanced_parameter_for_a_parameter_that_does_not_exist : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      private IParameter _p1;
      private AdvancedParameter _advancedParameterThatDoesNotExist;

      protected override void Context()
      {
         base.Context();
         _p1 = A.Fake<IParameter>();
         _allParameters.AddRange(new[] {_p1});
         sut.EditPopulation(_population);
         _advancedParameterThatDoesNotExist = new AdvancedParameter {ParameterPath = "DOES NOT EXIT"};
      }

      protected override void Because()
      {
         sut.AddAdvancedParameter(_advancedParameterThatDoesNotExist);
      }

      [Observation]
      public void should_not_add_the_parameter_to_the_underlying_parameter_presenter()
      {
         A.CallTo(() => _parametersPresenter.AddParameter(null)).MustNotHaveHappened();
      }
   }

   public class When_a_node_representing_a_parameter_is_being_selected : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      private ITreeNode _parameterNode;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new PKSimParameter().WithName("Tralal");
         _parameter.DisplayUnit = A.Fake<Unit>();
         _parameterNode = new ObjectWithIdAndNameNode<IParameter>(_parameter) {Text = "Tralala"};
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         _parametersPresenter.GroupNodeSelected += Raise.With(new NodeSelectedEventArgs(_parameterNode));
      }

      [Observation]
      public void should_retrieve_all_the_parameter_value_from_the_population_for_the_given_parameter()
      {
         A.CallTo(() => _popParameterDistributionPresenter.Plot(_population, _parameter, A<DistributionSettings>._, A<IDimension>._, A<Unit>._)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_plot_caption_with_the_text_defined_in_the_selected_node()
      {
         _settings.PlotCaption.ShouldBeEqualTo(_parameterNode.Text);
      }
   }

   public class When_a_node_that_is_not_a_parameter_node_is_being_selected : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      private ITreeNode _node;

      protected override void Context()
      {
         base.Context();
         _node = A.Fake<ITreeNode>();
      }

      protected override void Because()
      {
         _parametersPresenter.GroupNodeSelected += Raise.With(new NodeSelectedEventArgs(_node));
      }

      [Observation]
      public void should_reset_the_plots()
      {
         A.CallTo(() => _popParameterDistributionPresenter.ResetPlot()).MustHaveHappened();
      }
   }

   public class When_retrieving_all_genders_available_for_given_population_having_only_one_gender : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _population.AllGenders).Returns(new[] {_gender1});
         sut.EditPopulation(_population);
      }

      [Observation]
      public void should_only_return_the_available_gender()
      {
         sut.AllGenders().ShouldOnlyContain(_gender1.Name);
      }
   }

   public class When_retrieving_all_genders_available_for_given_population_having_many_genders : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditPopulation(_population);
      }

      [Observation]
      public void should_return_the_available_genders_and_have_added_the_generic_gender_to_the_genders()
      {
         sut.AllGenders().ShouldOnlyContainInOrder(CoreConstants.Population.AllGender, _gender1.Name, _gender2.Name);
      }
   }

   public class When_retrieving_the_display_name_for_a_given_gender : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditPopulation(_population);
      }

      [Observation]
      public void should_return_the_all_gender_name_if_the_selected_gender_is_the_all_gender()
      {
         sut.GenderDisplayFor(CoreConstants.Population.AllGender).ShouldBeEqualTo(PKSimConstants.UI.AllGender);
      }

      [Observation]
      public void should_return_the_display_name_of_the_gender_otherwise()
      {
         sut.GenderDisplayFor(_gender1.Name).ShouldBeEqualTo("Display1");
         sut.GenderDisplayFor(_gender2.Name).ShouldBeEqualTo("Display2");
      }
   }

   public class When_a_parameter_node_is_being_set_by_the_user_to_be_used_in_a_report : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      private IParameter _parameter;
      private ParameterDistributionSettingsCache _selectedDistribution;

      protected override void Context()
      {
         base.Context();
         _selectedDistribution = new ParameterDistributionSettingsCache();
         A.CallTo(() => _population.SelectedDistributions).Returns(_selectedDistribution);
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _parametersPresenter.SelectedParameter).Returns(_parameter);
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns("PATH");
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.UseSelectedParameterInReport(useInReport: true);
      }

      [Observation]
      public void should_have_added_the_current_settings_to_the_underlying_population()
      {
         _selectedDistribution.Contains("PATH").ShouldBeTrue();
      }

      [Observation]
      public void should_have_notified_a_building_block_changed()
      {
         A.CallTo(() => _projectChangedNotifier.NotifyChangedFor(_population)).MustHaveHappened();
      }
   }

   public class When_a_parameter_node_is_not_being_used_in_a_report_anymore : concern_for_PopulationAdvancedParameterDistributionPresenter
   {
      private IParameter _parameter;
      private ParameterDistributionSettingsCache _selectedDistribution;

      protected override void Context()
      {
         base.Context();
         _selectedDistribution = new ParameterDistributionSettingsCache
         {
            {"PATH", new ParameterDistributionSettings {Settings = new DistributionSettings()}}
         };
         A.CallTo(() => _population.SelectedDistributions).Returns(_selectedDistribution);
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _parametersPresenter.SelectedParameter).Returns(_parameter);
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns("PATH");
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.UseSelectedParameterInReport(useInReport: false);
      }

      [Observation]
      public void should_have_added_the_current_settings_to_the_underlying_population()
      {
         _selectedDistribution.Contains("PATH").ShouldBeFalse();
      }

      [Observation]
      public void should_have_notified_a_building_block_changed()
      {
         A.CallTo(() => _projectChangedNotifier.NotifyChangedFor(_population)).MustHaveHappened();
      }
   }
}