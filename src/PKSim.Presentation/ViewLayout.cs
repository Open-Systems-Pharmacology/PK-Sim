using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   /// <summary>
   ///    Specifies how the layout in wizard based application should be created (only available in certain use case such as
   ///    Create Simulation)
   /// </summary>
   public enum ViewLayoutId
   {
      TabbedView,
      AccordionView
   }

   public static class ViewLayouts
   {
      private static readonly ICache<ViewLayoutId, ViewLayout> _allViewLayouts = new Cache<ViewLayoutId, ViewLayout>(x => x.ViewLayoutId);

      public static ViewLayout TabbedView = create(ViewLayoutId.TabbedView, PKSimConstants.UI.TabbedView);
      public static ViewLayout AccordionView = create(ViewLayoutId.AccordionView, PKSimConstants.UI.AccordionView);

      private static ViewLayout create(ViewLayoutId viewLayoutId, string displayName)
      {
         var viewLayout = new ViewLayout(viewLayoutId, displayName);
         _allViewLayouts.Add(viewLayout);
         return viewLayout;
      }

      public static ViewLayout ById(string viewLayoutId)
      {
         return ById(EnumHelper.ParseValue<ViewLayoutId>(viewLayoutId));
      }

      public static ViewLayout ById(ViewLayoutId viewLayoutId)
      {
         return _allViewLayouts[viewLayoutId];
      }

      public static IEnumerable<ViewLayout> All()
      {
         return _allViewLayouts;
      }
   }

   public class ViewLayout
   {
      public ViewLayoutId ViewLayoutId { get; private set; }
      public string DisplayName { get; private set; }

      internal ViewLayout(ViewLayoutId viewLayoutId, string displayName)
      {
         ViewLayoutId = viewLayoutId;
         DisplayName = displayName;
      }

      public override string ToString()
      {
         return DisplayName;
      }

      public string Id
      {
         get { return ViewLayoutId.ToString(); }
      }

   }
}