using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Simulations
{
   public static class SimulationItems
   {
      public static SimulationItem<ISimulationModelConfigurationPresenter> Model = new SimulationItem<ISimulationModelConfigurationPresenter>();
      public static SimulationItem<ISimulationCompoundConfigurationCollectorPresenter> Compounds = new SimulationItem<ISimulationCompoundConfigurationCollectorPresenter>();
      public static SimulationItem<ISimulationCompoundProcessSummaryCollectorPresenter> CompoundsProcesses = new SimulationItem<ISimulationCompoundProcessSummaryCollectorPresenter>();
      public static SimulationItem<ISimulationCompoundProtocolCollectorPresenter> CompoundProtocols = new SimulationItem<ISimulationCompoundProtocolCollectorPresenter>();
      public static SimulationItem<ISimulationEventsConfigurationPresenter> Events = new SimulationItem<ISimulationEventsConfigurationPresenter>();
      public static SimulationItem<ISimulationObserversConfigurationPresenter> Observers = new SimulationItem<ISimulationObserversConfigurationPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Model, Compounds, CompoundsProcesses, CompoundProtocols, Events, Observers };
   }

   public class SimulationItem<TSimulationItemPresenter> : SubPresenterItem<TSimulationItemPresenter> where TSimulationItemPresenter : ISimulationItemPresenter
   {
   }
}