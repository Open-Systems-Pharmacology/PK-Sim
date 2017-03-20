using System;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationSubjectSelectionView : IModalView<ISimulationSubjectSelectionPresenter>
   {
      Type SimulationSubjectType { set; }
   }
}