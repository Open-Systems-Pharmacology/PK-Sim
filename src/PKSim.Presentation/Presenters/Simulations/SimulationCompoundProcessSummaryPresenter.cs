using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundProcessSummaryPresenter : IEditSimulationCompoundPresenter
   {
      /// <summary>
      /// True if any processes are defined in the subject of this presenter
      /// </summary>
      bool HasProcessesDefined { get; }
   }

   public class SimulationCompoundProcessSummaryPresenter : AbstractSubPresenter<ISimulationCompoundProcessSummaryView, ISimulationCompoundProcessSummaryPresenter>,
      ISimulationCompoundProcessSummaryPresenter
   {
      public SimulationCompoundProcessSummaryPresenter(ISimulationCompoundProcessSummaryView view,
         ISimulationCompoundEnzymaticProcessPresenter simulationEnzymaticProcessPresenter,
         ISimulationCompoundTransportAndExcretionPresenter simulationTransportAndExcretionPresenter,
         ISimulationCompoundSpecificBindingPresenter simulationSpecificBindingPresenter)
         : base(view)
      {

         AddSubPresenters(simulationEnzymaticProcessPresenter, simulationTransportAndExcretionPresenter, simulationSpecificBindingPresenter);
         _view.AddProcessView(simulationEnzymaticProcessPresenter.View);
         _view.AddProcessView(simulationTransportAndExcretionPresenter.View);
         _view.AddProcessView(simulationSpecificBindingPresenter.View);
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         var compoundProperties = simulation.CompoundPropertiesFor(compound);
         AllSimulationProcessPresenters.Each(p => p.EditProcessesIn(simulation, compoundProperties));
      }

      public void SaveConfiguration()
      {
         AllSimulationProcessPresenters.Each(p => p.SaveConfiguration());
      }

      protected IEnumerable<ISimulationCompoundProcessPresenter> AllSimulationProcessPresenters => base.AllSubPresenters.OfType<ISimulationCompoundProcessPresenter>();

      public bool HasProcessesDefined
      {
         get { return AllSimulationProcessPresenters.Any(x => x.HasProcessesDefined); }
      }
   }
}