using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualPresenter : IContainerPresenter
   {
      PKSim.Core.Model.Individual Individual { get; }
   }
}