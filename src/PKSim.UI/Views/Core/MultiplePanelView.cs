using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Core
{
   public partial class MultiplePanelView : BaseContainerUserControl, IMultiplePanelView
   {
      public MultiplePanelView()
      {
         InitializeComponent();
      }

      public void ActivateView(IView view)
      {
         AddViewTo(layoutControlGroup,layoutControl,  view);
      }


      public void HideView(IView view)
      {
         var controlGroup = layoutControlGroupContainingView(view);
         if (controlGroup == null) return;
         controlGroup.Visibility = LayoutVisibility.Never;
      }

      private LayoutControlGroup layoutControlGroupContainingView(IView view)
      {
         return allLayoutControlGroups().FirstOrDefault(group => firstGroupContainingView(view, group) != null);
      }

      private static LayoutControlItem firstGroupContainingView(IView view, LayoutControlGroup group)
      {
         return group.Items.OfType<LayoutControlItem>().FirstOrDefault(x => Equals(x.Control, view));
      }

      private IEnumerable<LayoutControlGroup> allLayoutControlGroups()
      {
         return layoutControlGroup.Items.OfType<LayoutControlGroup>();
      }

      public void AddEmptyPlaceHolder()
      {
         AddEmptyPlaceHolder(layoutControlGroup);
      }

      public void AttachPresenter(IMultiplePanelPresenter presenter)
      {
      }

   }
}
