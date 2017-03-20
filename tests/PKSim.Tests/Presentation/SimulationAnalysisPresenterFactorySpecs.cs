using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.Charts;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using IContainer = OSPSuite.Utility.Container.IContainer;
using SimulationAnalysisPresenterFactory = PKSim.Presentation.Presenters.Charts.SimulationAnalysisPresenterFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationAnalysisPresenterFactory : ContextSpecification<ISimulationAnalysisPresenterFactory>
   {
      private IContainer _container;
      protected ISimulationTimeProfileChartPresenter _timeProfileChartPresenter;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         _timeProfileChartPresenter = A.Fake<ISimulationTimeProfileChartPresenter>();
         A.CallTo(() => _container.Resolve<ISimulationTimeProfileChartPresenter>()).Returns(_timeProfileChartPresenter);
         sut = new SimulationAnalysisPresenterFactory(_container);
      }
   }

   public class When_the_plot_presenter_factory_is_asked_to_retrieve_a_plot_presenter_for_a_concentraion_plot : concern_for_SimulationAnalysisPresenterFactory
   {
      private SimulationTimeProfileChart _chart;
      private ISimulationAnalysisPresenter _presenter;

      protected override void Context()
      {
         base.Context();
         _chart = new SimulationTimeProfileChart();
      }

      protected override void Because()
      {
         _presenter = sut.PresenterFor(_chart);
      }

      [Observation]
      public void should_return_a_plot_presenter_matching_the_plot()
      {
         _presenter.ShouldBeAnInstanceOf<ISimulationTimeProfileChartPresenter>();
      }
   }

   public class When_the_plot_presenter_factory_is_asked_to_retrieve_a_plot_presenter_for_an_unknown_plot_type : concern_for_SimulationAnalysisPresenterFactory
   {
      private ISimulationAnalysis _chart;

      protected override void Context()
      {
         base.Context();
         _chart = A.Fake<ISimulationAnalysis>();
      }

      [Observation]
      public void should_return_a_plot_presenter_matching_the_plot()
      {
         The.Action(() => sut.PresenterFor(_chart)).ShouldThrowAn<ArgumentException>();
      }
   }
}