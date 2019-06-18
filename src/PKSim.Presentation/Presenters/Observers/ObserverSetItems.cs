using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Observers
{
   public class ObserverSetItems
   {
      public static readonly ObserverSetItem<IImportObserverSetPresenter> ImportSettings = new ObserverSetItem<IImportObserverSetPresenter> {Index = 0};
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {ImportSettings};
   }

   public class ObserverSetItem<TObserverSetItemPresenter> : SubPresenterItem<TObserverSetItemPresenter> where TObserverSetItemPresenter : IObserverSetItemPresenter
   {
   }
}