using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation.UICommands
{
   public abstract class StartSimulationPopulationAnalysisCommand : ActiveObjectUICommand<IPopulationDataCollector>
   {
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;
      private readonly PopulationAnalysisType _populationAnalysisType;

      protected StartSimulationPopulationAnalysisCommand(ISimulationAnalysisCreator simulationAnalysisCreator, IActiveSubjectRetriever activeSubjectRetriever,
         PopulationAnalysisType populationAnalysisType)
         : base(activeSubjectRetriever)
      {
         _simulationAnalysisCreator = simulationAnalysisCreator;
         _populationAnalysisType = populationAnalysisType;
      }

      protected override void PerformExecute()
      {
         _simulationAnalysisCreator.CreatePopulationAnalysisFor(Subject, _populationAnalysisType);
      }
   }

   public class StartBoxWhiskerAnalysisCommand : StartSimulationPopulationAnalysisCommand
   {
      public StartBoxWhiskerAnalysisCommand(ISimulationAnalysisCreator simulationAnalysisCreator, IActiveSubjectRetriever activeSubjectRetriever)
         : base(simulationAnalysisCreator, activeSubjectRetriever, PopulationAnalysisType.BoxWhisker)
      {
      }
   }

   public class StartScatterAnalysisCommand : StartSimulationPopulationAnalysisCommand
   {
      public StartScatterAnalysisCommand(ISimulationAnalysisCreator simulationAnalysisCreator, IActiveSubjectRetriever activeSubjectRetriever)
         : base(simulationAnalysisCreator, activeSubjectRetriever, PopulationAnalysisType.Scatter)
      {
      }
   }

   public class StartRangeAnalysisCommand : StartSimulationPopulationAnalysisCommand
   {
      public StartRangeAnalysisCommand(ISimulationAnalysisCreator simulationAnalysisCreator, IActiveSubjectRetriever activeSubjectRetriever)
         : base(simulationAnalysisCreator, activeSubjectRetriever, PopulationAnalysisType.Range)
      {
      }
   }

   public class StartTimeProfileAnalysisCommand : StartSimulationPopulationAnalysisCommand
   {
      public StartTimeProfileAnalysisCommand(ISimulationAnalysisCreator simulationAnalysisCreator, IActiveSubjectRetriever activeSubjectRetriever)
         : base(simulationAnalysisCreator, activeSubjectRetriever, PopulationAnalysisType.TimeProfile)
      {
      }
   }
}