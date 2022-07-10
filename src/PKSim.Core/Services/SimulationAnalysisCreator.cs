using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface ISimulationAnalysisCreator : OSPSuite.Core.Domain.Services.ISimulationAnalysisCreator
   {
      ISimulationAnalysis CreateAnalysisFor(Simulation simulation); //probably here we should add the simulationType, when we figure out what to do with the visitor
      ISimulationAnalysis CreatePredictedVsObservedAnalysisFor(IndividualSimulation simulation);
      ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector);
      ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector, PopulationAnalysisType populationAnalysisType);
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
         try
         {
            if (simulation != null)
            {
               _simulationAnalysis = _chartFactory.CreatePredictedVsObservedChartFor(simulation);
               AddSimulationAnalysisTo(simulation, _simulationAnalysis);
            }

            return _simulationAnalysis;
         }
         finally
         {
            _simulationAnalysis = null;
         }
      }

      public ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector)
      {
         return CreatePopulationAnalysisFor(populationDataCollector, _userSettings.DefaultPopulationAnalysis);
      }

      public ISimulationAnalysis CreatePopulationAnalysisFor(IPopulationDataCollector populationDataCollector, PopulationAnalysisType populationAnalysisType)
      {
         var populationSimulationAnalysis = _populationSimulationAnalysisStarter.CreateAnalysisForPopulationSimulation(populationDataCollector, populationAnalysisType);
         AddSimulationAnalysisTo(populationDataCollector, populationSimulationAnalysis);
         return populationSimulationAnalysis;
      }

      public void Visit(IndividualSimulation simulation)//OK, so now I am afraid we are going to break the visitor when adding the new analysis
      {
         _simulationAnalysis = _chartFactory.CreateChartFor(simulation);
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