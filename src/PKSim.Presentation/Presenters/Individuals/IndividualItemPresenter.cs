using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualItemPresenter : ISubPresenter
   {
      void EditIndividual(Individual individualToEdit);
   }
}