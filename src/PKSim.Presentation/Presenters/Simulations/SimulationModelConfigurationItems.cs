using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Simulations
{
   public static class SimulationModelConfigurationItems
   {
      public static SimulationModelConfigurationItem<ISimulationSubjectConfigurationPresenter> Subject = new SimulationModelConfigurationItem<ISimulationSubjectConfigurationPresenter>();
      public static SimulationModelConfigurationItem<ISimulationModelSelectionPresenter> ModelSelection = new SimulationModelConfigurationItem<ISimulationModelSelectionPresenter>();
      public static SimulationModelConfigurationItem<ISimulationCompoundsSelectionPresenter> CompoundsSelection = new SimulationModelConfigurationItem<ISimulationCompoundsSelectionPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { Subject, CompoundsSelection,ModelSelection };
   }

   public class SimulationModelConfigurationItem<TSimulationModelConfigurationItem> : SubPresenterItem<TSimulationModelConfigurationItem> where TSimulationModelConfigurationItem : ISimulationModelConfigurationItemPresenter
   {
   }
}