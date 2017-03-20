using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class LoadPopulationAnalysisWorkflowFromTemplateUICommand : ActiveObjectUICommand<PopulationSimulation>
   {
      private readonly IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;

      public LoadPopulationAnalysisWorkflowFromTemplateUICommand(IActiveSubjectRetriever activeSubjectRetriever,IPopulationAnalysisTemplateTask populationAnalysisTemplateTask) : base(activeSubjectRetriever)
      {
         _populationAnalysisTemplateTask = populationAnalysisTemplateTask;
      }

      protected override void PerformExecute()
      {
         _populationAnalysisTemplateTask.LoadPopulationAnalysisWorkflowInto(Subject);
      }
   }
}