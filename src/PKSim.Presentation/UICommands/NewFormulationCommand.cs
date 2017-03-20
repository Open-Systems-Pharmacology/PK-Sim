using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewFormulationCommand : AddBuildingBlockUICommand< PKSim.Core.Model.Formulation, IFormulationTask>
   {
      public NewFormulationCommand(IFormulationTask formulationTask) : base(formulationTask)
      {
      }
   }
}