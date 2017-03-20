using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface IFormulationItemPresenter : ISubPresenter
   {
      void EditFormulation(Formulation formulation);
   }
}