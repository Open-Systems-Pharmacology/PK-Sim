using OSPSuite.Core.Chart;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.Charts
{
   public class SimulationAnalysisPresenterFactory : OSPSuite.Presentation.Presenters.SimulationAnalysisPresenterFactory, ISimulationAnalysisPresenterFactory
   {
      public SimulationAnalysisPresenterFactory(IContainer container) : base(container)
      {
      }

      protected override ISimulationAnalysisPresenter PresenterFor(ISimulationAnalysis simulationAnalysis, IContainer container)
      {
         if (simulationAnalysis.IsAnImplementationOf<SimulationTimeProfileChart>())
            return container.Resolve<ISimulationTimeProfileChartPresenter>();

         if (simulationAnalysis.IsAnImplementationOf<SimulationPredictedVsObservedChart>())
            return container.Resolve<ISimulationPredictedVsObservedChartPresenter>();

         if (simulationAnalysis.IsAnImplementationOf<BoxWhiskerAnalysisChart>())
            return container.Resolve<IEditBoxWhiskerAnalysisChartPresenter>();

         if (simulationAnalysis.IsAnImplementationOf<ScatterAnalysisChart>())
            return container.Resolve<IEditScatterAnalysisChartPresenter>();

         if (simulationAnalysis.IsAnImplementationOf<RangeAnalysisChart>())
            return container.Resolve<IEditRangeAnalysisChartPresenter>();

         if (simulationAnalysis.IsAnImplementationOf<TimeProfileAnalysisChart>())
            return container.Resolve<IEditTimeProfileAnalysisChartPresenter>();

         return null;
      }
   }
}