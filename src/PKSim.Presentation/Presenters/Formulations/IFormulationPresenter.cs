using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface IFormulationPresenter : IContainerPresenter
   {
       PKSim.Core.Model.Formulation Formulation { get; }

   }
}