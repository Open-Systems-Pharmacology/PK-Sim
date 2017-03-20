using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Mappers
{
   public interface IAnalysableToSimulationAnalysisWorkflowMapper : IMapper<IAnalysable, SimulationAnalysisWorkflow>
   {
   }

   public class AnalysableToSimulationAnalysisWorkflowMapper : IAnalysableToSimulationAnalysisWorkflowMapper
   {
      public SimulationAnalysisWorkflow MapFrom(IAnalysable populationDataCollector)
      {
         var populationAnalysisWorkflow = new SimulationAnalysisWorkflow();
         populationDataCollector.Analyses.Each(populationAnalysisWorkflow.Add);
         var populationSimulation = populationDataCollector as PopulationSimulation;
         if (populationSimulation != null)
            populationAnalysisWorkflow.OutputSelections = populationSimulation.OutputSelections;

         return populationAnalysisWorkflow;
      }
   }
}