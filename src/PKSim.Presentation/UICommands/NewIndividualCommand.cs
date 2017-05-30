using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewIndividualCommand : AddBuildingBlockUICommand<Individual, IIndividualTask>
   {
      public NewIndividualCommand(IIndividualTask individualTask) : base(individualTask)
      {
      }
   }
}