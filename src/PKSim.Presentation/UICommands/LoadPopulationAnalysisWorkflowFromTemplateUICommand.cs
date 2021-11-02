using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadPopulationAnalysisWorkflowFromTemplateUICommand : ActiveObjectUICommand<PopulationSimulation>
   {
      private readonly IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;

      public LoadPopulationAnalysisWorkflowFromTemplateUICommand(IActiveSubjectRetriever activeSubjectRetriever, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask) : base(activeSubjectRetriever)
      {
         _populationAnalysisTemplateTask = populationAnalysisTemplateTask;
      }

      protected override async void PerformExecute()
      {
         await _populationAnalysisTemplateTask.SecureAwait(x => x.LoadPopulationAnalysisWorkflowIntoAsync(Subject));
      }
   }
}