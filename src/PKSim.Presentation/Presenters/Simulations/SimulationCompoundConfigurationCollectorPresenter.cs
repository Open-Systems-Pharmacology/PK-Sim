using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundConfigurationCollectorPresenter : ISimulationItemPresenter
   {
   }

   public class SimulationCompoundConfigurationCollectorPresenter : SimulationCompoundCollectorPresenterBase<ISimulationCompoundCollectorView, ISimulationCompoundConfigurationPresenter>, ISimulationCompoundConfigurationCollectorPresenter
   {
      public SimulationCompoundConfigurationCollectorPresenter(
         ISimulationCompoundCollectorView view,
         IApplicationController applicationController,
         IConfigurableLayoutPresenter configurableLayoutPresenter,
         IEventPublisher eventPublisher) : base(view, applicationController, configurableLayoutPresenter, eventPublisher)
      {
         view.Caption = PKSimConstants.UI.SimulationCompoundsConfiguration;
         view.ApplicationIcon = ApplicationIcons.Compound;
      }
   }
}