using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Observers
{
   public class ObserverItems
   {
      public static readonly ObserverItem<IImportObserversPresenter> ImportSettings = new ObserverItem<IImportObserversPresenter> {Index = 0};
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {ImportSettings};
   }

   public class ObserverItem<TObserverItemPresenter> : SubPresenterItem<TObserverItemPresenter> where TObserverItemPresenter : IObserverItemPresenter
   {
   }
}