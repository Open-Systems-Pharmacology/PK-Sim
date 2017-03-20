using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
  
   public class NewIndividualCommand : AddBuildingBlockUICommand<PKSim.Core.Model.Individual, IIndividualTask>
   {
      public NewIndividualCommand(IIndividualTask individualTask) : base(individualTask)
      {
      }
   }
}