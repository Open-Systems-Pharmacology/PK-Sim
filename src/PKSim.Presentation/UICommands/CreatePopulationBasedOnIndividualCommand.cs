using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class CreatePopulationBasedOnIndividualCommand : ObjectUICommand<Individual>
   {
      private readonly IPopulationTask _populationTask;

      public CreatePopulationBasedOnIndividualCommand(IPopulationTask populationTask)
      {
         _populationTask = populationTask;
      }

      protected override void PerformExecute()
      {
         _populationTask.AddToProjectBasedOn(Subject);
      }
   }
}