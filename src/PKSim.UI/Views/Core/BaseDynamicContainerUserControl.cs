using OSPSuite.Utility.Extensions;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraLayout;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Core
{
   /// <summary>
   ///    Defines a base class for a container that is adding an arbitrary number of sub containers
   /// </summary>
   public partial class BaseDynamicContainerUserControl : BaseContainerUserControl
   {
      private readonly UserLookAndFeel _lookAndFeel;

      //for designer only
      public BaseDynamicContainerUserControl()
      {
         InitializeComponent();
      }

      public BaseDynamicContainerUserControl(UserLookAndFeel lookAndFeel) : this()
      {
         _lookAndFeel = lookAndFeel;
      }

      protected LayoutControlItem AddViewToLayout(IView view)
      {
         LayoutControlItem layoutControlItem;
         try
         {
            layoutControl.BeginUpdate();
            layoutControlItem = new LayoutControlItem(mainLayoutGroup) {TextLocation = Locations.Top};
            layoutControlItem.InitializeAsHeader(_lookAndFeel, view.Caption);
            mainLayoutGroup.AddItem(layoutControlItem);

            AddViewTo(layoutControlItem, view);

            if (!view.Caption.IsNullOrEmpty())
               mainLayoutGroup.AddItem(new EmptySpaceItem());
         }
         finally
         {
            layoutControl.EndUpdate();
         }
         return layoutControlItem;
      }
   }
}