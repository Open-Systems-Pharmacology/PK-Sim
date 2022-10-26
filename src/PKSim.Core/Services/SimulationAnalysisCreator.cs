using OSPSuite.Core.Chart.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Chart;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationAnalysisCreator : OSPSuite.Core.Domain.Services.ISimulationAnalysisCreator
   {
      ISimulationAnalysis CreateAnalysisFor(Simulation simulation);
      ISimulationAnalysis CreatePredictedVsObservedAnalysisFor(IndividualSimulation simulation);
      ISimulationAnalysis CreateResidualsVsTimeAnalysisFor(IndividualSimulation simulation);
      ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector);

      ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector,
         PopulationAnalysisType populationAnalysisType);
   }

   public class SimulationAnalysisCreator : OSPSuite.Core.Domain.Services.SimulationAnalysisCreator, ISimulationAnalysisCreator,
      IStrictVisitor,
      IVisitor<IndividualSimulation>,
      IVisitor<PopulationSimulation>

   {
      private readonly IPopulationSimulationAnalysisStarter _populationSimulationAnalysisStarter;
      private readonly IPKSimChartFactory _chartFactory;
      private readonly ICoreUserSettings _userSettings;
      private readonly ICloner _cloner;
      private ISimulationAnalysis _simulationAnalysis;

      public SimulationAnalysisCreator(IPopulationSimulationAnalysisStarter populationSimulationAnalysisStarter,
         IExecutionContext executionContext, IContainerTask containerTask, IPKSimChartFactory chartFactory,
         ICoreUserSettings userSettings, ICloner cloner) : base(containerTask, executionContext)
      {
         _populationSimulationAnalysisStarter = populationSimulationAnalysisStarter;
         _chartFactory = chartFactory;
         _userSettings = userSettings;
         _cloner = cloner;
      }

      public ISimulationAnalysis CreateAnalysisFor(Simulation simulation)
      {
         try
         {
            if (simulation != null)
               this.Visit(simulation);

            return _simulationAnalysis;
         }
         finally
         {
            _simulationAnalysis = null;
         }
      }

      public ISimulationAnalysis CreatePredictedVsObservedAnalysisFor(IndividualSimulation simulation)
      {
         _simulationAnalysis = _chartFactory.CreateChartFor<SimulationPredictedVsObservedChart>(simulation);
         AddSimulationAnalysisTo(simulation, _simulationAnalysis);
         return _simulationAnalysis;
      }

      public ISimulationAnalysis CreateResidualsVsTimeAnalysisFor(IndividualSimulation simulation)
      {
         _simulationAnalysis = _chartFactory.CreateChartFor<SimulationResidualVsTimeChart>(simulation);
         AddSimulationAnalysisTo(simulation, _simulationAnalysis);
         return _simulationAnalysis;
      }

      public ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector)
      {
         return CreatePopulationAnalysisFor(populationDataCollector, _userSettings.DefaultPopulationAnalysis);
      }

      public ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector,
         PopulationAnalysisType populationAnalysisType)
      {
         var populationSimulationAnalysis =
            _populationSimulationAnalysisStarter.CreateAnalysisForPopulationSimulation(populationDataCollector, populationAnalysisType);
         AddSimulationAnalysisTo(populationDataCollector, populationSimulationAnalysis);
         return populationSimulationAnalysis;
      }

      public void Visit(IndividualSimulation simulation)
      {
         _simulationAnalysis = _chartFactory.CreateChartFor<SimulationTimeProfileChart>(simulation);
         AddSimulationAnalysisTo(simulation, _simulationAnalysis);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         _simulationAnalysis = CreatePopulationAnalysisFor(populationSimulation.DowncastTo<IPopulationDataCollector>());
      }

      public override ISimulationAnalysis CreateAnalysisBasedOn(ISimulationAnalysis sourceAnalysis)
      {
         return _cloner.CloneObject(sourceAnalysis);
      }
   }
}