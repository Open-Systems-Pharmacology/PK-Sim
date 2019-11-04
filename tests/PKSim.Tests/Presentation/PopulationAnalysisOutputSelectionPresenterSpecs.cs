using System;
using System.Linq;
using OSPSuite.Utility.Events;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisOutputSelectionPresenter : ContextSpecification<IPopulationAnalysisOutputSelectionPresenter>
   {
      protected IPopulationAnalysisOutputSelectionView _view;
      protected IQuantityListPresenter _allOutputsPresenter;
      protected IPopulationAnalysisOutputFieldsPresenter _selectedOutputsPresenter;
      protected IPopulationAnalysisStatisticsSelectionPresenter _statisticSelectionPresenter;
      protected IEntitiesInContainerRetriever _outputsRetriever;
      protected IEventPublisher _eventPublisher;
      private IDimensionRepository _dimensionRepository;
      protected IDimension _timeDimension;

      protected override void Context()
      {
         _view = A.Fake<IPopulationAnalysisOutputSelectionView>();
         _allOutputsPresenter = A.Fake<IQuantityListPresenter>();
         _selectedOutputsPresenter = A.Fake<IPopulationAnalysisOutputFieldsPresenter>();
         _statisticSelectionPresenter = A.Fake<IPopulationAnalysisStatisticsSelectionPresenter>();
         _outputsRetriever = A.Fake<IEntitiesInContainerRetriever>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _dimensionRepository= A.Fake<IDimensionRepository>();
         _timeDimension= A.Fake<IDimension>();  
         A.CallTo(() => _dimensionRepository.Time).Returns(_timeDimension);
         sut = new PopulationAnalysisOutputSelectionPresenter(_view,_allOutputsPresenter,_selectedOutputsPresenter,_statisticSelectionPresenter,_outputsRetriever,_eventPublisher,_dimensionRepository);
      }
   }

   public class When_starting_the_output_selection_presenter : concern_for_PopulationAnalysisOutputSelectionPresenter
   {
      private PopulationStatisticalAnalysis _populationAnalysis;
      private IPopulationDataCollector _populationDataCollector;
      private PathCache<IQuantity> _allOutputs;

      protected override void Context()
      {
         base.Context();
         _populationAnalysis= A.Fake<PopulationStatisticalAnalysis>();
         _populationDataCollector= A.Fake<IPopulationDataCollector>();
         _allOutputs=new PathCacheForSpecs<IQuantity>();
         A.CallTo(() => _outputsRetriever.OutputsFrom(_populationDataCollector)).Returns(_allOutputs);
      }

      protected override void Because()
      {
         sut.StartAnalysis(_populationDataCollector,_populationAnalysis);
      }

      [Observation]
      public void should_add_all_available_outputs_from_the_data_collector_into_the_output_selection_view()
      {
         A.CallTo(() => _allOutputsPresenter.Edit(_allOutputs)).MustHaveHappened();
      }

      [Observation]
      public void should_start_the_analysis_for_all_sub_presenters()
      {
         A.CallTo(() => _selectedOutputsPresenter.StartAnalysis(_populationDataCollector, _populationAnalysis)).MustHaveHappened();
         A.CallTo(() => _statisticSelectionPresenter.StartAnalysis(_populationDataCollector, _populationAnalysis)).MustHaveHappened();
      }

      [Observation]
      public void should_ensure_that_output_groups_are_expanded()
      {
         _allOutputsPresenter.ExpandAllGroups.ShouldBeTrue();
      }
   }

   public class When_removing_an_output : concern_for_PopulationAnalysisOutputSelectionPresenter
   {
      protected override void Because()
      {
         sut.RemoveOutput();
      }

      [Observation]
      public void should_remove_the_selected_output_from_the_current_selection()
      {
         A.CallTo(() => _selectedOutputsPresenter.RemoveSelection()).MustHaveHappened();
      }
   }

   public class When_notify_that_the_statistic_selection_was_changed : concern_for_PopulationAnalysisOutputSelectionPresenter
   {
      protected override void Because()
      {
         _statisticSelectionPresenter.SelectionChanged += Raise.With(new EventArgs());
      }

      [Observation]
      public void should_raise_an_event_notifying_that_the_chart_settings_have_changed_and_needs_to_be_updated()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A < PopulationAnalysisChartSettingsChangedEvent>._)).MustHaveHappened();
      }
   }

   public class When_asked_for_the_available_time_units : concern_for_PopulationAnalysisOutputSelectionPresenter
   {
      [Observation]
      public void the_presenter_should_return_all_unit_defined_for_time()
      {
         sut.AllTimeUnits.ShouldBeEqualTo(_timeDimension.Units);
      }
   }
   public class When_starting_an_analysis_for_a_population_containing_only_one_output : concern_for_PopulationAnalysisOutputSelectionPresenter
   {
      private PopulationStatisticalAnalysis _populationAnalysis;
      private IPopulationDataCollector _populationDataCollector;
      private PathCache<IQuantity> _allOutputs;
      private IQuantity _oneOutput;

      protected override void Context()
      {
         base.Context();
         _populationAnalysis = A.Fake<PopulationStatisticalAnalysis>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _allOutputs = new PathCacheForSpecs<IQuantity>();
         _oneOutput = new Observer {Name = "obs"};
         _allOutputs.Add(_oneOutput);
         A.CallTo(() => _outputsRetriever.OutputsFrom(_populationDataCollector)).Returns(_allOutputs);
         var dto=new QuantitySelectionDTO{Quantity = _oneOutput};
         dto.PathElements.Add(PathElementId.TopContainer, new PathElement{DisplayName = "TOTO"});
         A.CallTo(() => _allOutputsPresenter.QuantityDTOByPath(_allOutputs.Keys.First())).Returns(dto);
      }

      protected override void Because()
      {
         sut.StartAnalysis(_populationDataCollector, _populationAnalysis);
      }

      [Observation]
      public void should_automatically_select_the_output()
      {
         A.CallTo(() => _selectedOutputsPresenter.AddOutput(_oneOutput, "TOTO")).MustHaveHappened();
      }
   }

   public class When_starting_an_analysis_for_a_population_containing_more_that_one_output : concern_for_PopulationAnalysisOutputSelectionPresenter
   {
      private PopulationStatisticalAnalysis _populationAnalysis;
      private IPopulationDataCollector _populationDataCollector;
      private PathCache<IQuantity> _allOutputs;

      protected override void Context()
      {
         base.Context();
         _populationAnalysis = A.Fake<PopulationStatisticalAnalysis>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _allOutputs = new PathCacheForSpecs<IQuantity>();
         _allOutputs.Add(new Observer { Name = "obs" });
         _allOutputs.Add(new Observer { Name = "obs2" });
         A.CallTo(() => _outputsRetriever.OutputsFrom(_populationDataCollector)).Returns(_allOutputs);
      }

      protected override void Because()
      {
         sut.StartAnalysis(_populationDataCollector, _populationAnalysis);
      }

      [Observation]
      public void should_not_select_any_output()
      {
         A.CallTo(() => _selectedOutputsPresenter.AddOutput(A<IQuantity>._,A<string>._)).MustNotHaveHappened();
      }
   }
}	