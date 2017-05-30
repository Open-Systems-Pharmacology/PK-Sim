using PKSim.Core.Model;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ScaleIndividualCommand : ObjectUICommand<Individual>
   {
      private readonly IIndividualTask _individualTask;

      public ScaleIndividualCommand(IIndividualTask individualTask)
      {
         _individualTask = individualTask;
      }

      protected override void PerformExecute()
      {
         _individualTask.ScaleIndividual(Subject);
      }
   }
}