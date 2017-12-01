using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExtractIndividualsFromPopulationCommand : ActiveObjectUICommand<Population>
   {
      private readonly IPopulationTask _populationTask;

      public ExtractIndividualsFromPopulationCommand(IActiveSubjectRetriever activeSubjectRetriever, IPopulationTask populationTask) : base(activeSubjectRetriever)
      {
         _populationTask = populationTask;
      }

      protected override void PerformExecute()
      {
         _populationTask.ExtractIndividuals(Subject);
      }
   }
}