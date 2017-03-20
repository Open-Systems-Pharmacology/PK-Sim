using PKSim.Assets;
using OSPSuite.Utility.Events;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundConfigurationCollectorPresenter : ISimulationItemPresenter
   {
   }

   public class SimulationCompoundConfigurationCollectorPresenter : SimulationCompoundCollectorPresenterBase<ISimulationCompoundCollectorView,ISimulationCompoundConfigurationPresenter>, ISimulationCompoundConfigurationCollectorPresenter
   {
      public SimulationCompoundConfigurationCollectorPresenter(ISimulationCompoundCollectorView view, IApplicationController applicationController, IConfigurableLayoutPresenter configurableLayoutPresenter, IEventPublisher eventPubliser) : base(view, applicationController, configurableLayoutPresenter, eventPubliser)
      {
         view.Caption = PKSimConstants.UI.SimulationCompoundsConfiguration;
         view.ApplicationIcon = ApplicationIcons.Compound;
      }
   }
}