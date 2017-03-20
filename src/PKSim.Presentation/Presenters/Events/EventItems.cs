using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Events
{
   public class EventItems
   {
      public static readonly EventItem<IEventSettingsPresenter> Settings = new EventItem<IEventSettingsPresenter> {Index = 0};
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Settings};
   }

   public class EventItem<TEventItemPresenter> : SubPresenterItem<TEventItemPresenter> where TEventItemPresenter : IEventItemPresenter
   {
   }
}