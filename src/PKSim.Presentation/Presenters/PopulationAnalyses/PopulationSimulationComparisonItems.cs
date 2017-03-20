using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public static class PopulationSimulationComparisonItems
   {
      public static PopulationSimulationComparisonItem<IPopulationSimulationComparisonDistributionPresenter> Parameters = new PopulationSimulationComparisonItem<IPopulationSimulationComparisonDistributionPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Parameters};
   }

   public class PopulationSimulationComparisonItem<TPresenter> : SubPresenterItem<TPresenter> where TPresenter : IPopulationSimulationComparisonItemPresenter
   {
   }
}