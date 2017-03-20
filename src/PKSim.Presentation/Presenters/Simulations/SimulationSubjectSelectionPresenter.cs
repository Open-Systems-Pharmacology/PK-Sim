using System;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationSubjectSelectionPresenter : IPresenter<ISimulationSubjectSelectionView>, IDisposablePresenter
   {
      bool ChooseSimulationSubject();
      Type SimulationSubjetType { get; set; }
   }

   public class SimulationSubjectSelectionPresenter : AbstractDisposablePresenter<ISimulationSubjectSelectionView, ISimulationSubjectSelectionPresenter>, ISimulationSubjectSelectionPresenter
   {
      public Type SimulationSubjetType { get; set; }

      public SimulationSubjectSelectionPresenter(ISimulationSubjectSelectionView view) : base(view)
      {
         SimulationSubjetType = typeof (PKSim.Core.Model.Individual);
         _view.SimulationSubjectType = SimulationSubjetType;
      }

      public bool ChooseSimulationSubject()
      {
         _view.Display();
         return !_view.Canceled;
      }
   }
}