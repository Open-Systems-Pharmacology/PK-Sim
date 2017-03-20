using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Protocols
{
   public static class ProtocolItems
   {
      public static readonly ProtocolItem<ISimpleProtocolPresenter> Simple = new ProtocolItem<ISimpleProtocolPresenter>();
      public static readonly ProtocolItem<IAdvancedProtocolPresenter> Advanced = new ProtocolItem<IAdvancedProtocolPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { Simple, Advanced };
   }

   public class ProtocolItem<TProtocolItemPresenter> : SubPresenterItem<TProtocolItemPresenter> where TProtocolItemPresenter : IProtocolItemPresenter
   {
   }
}