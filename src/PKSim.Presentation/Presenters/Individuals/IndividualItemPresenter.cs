using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualItemPresenter : ISubPresenter
   {
      void EditIndividual(PKSim.Core.Model.Individual individualToEdit);
   }
}