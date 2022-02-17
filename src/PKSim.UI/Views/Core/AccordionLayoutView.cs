using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Views;

namespace PKSim.UI.Views.Core
{
   public partial class AccordionLayoutView : BaseUserControl, IAccordionLayoutView
   {
      public AccordionLayoutView()
      {
         InitializeComponent();
      }

      public void StartAddingViews()
      {
         layoutControl.SuspendLayout();
      }

      protected LayoutControlItem AddViewTo(LayoutControlGroup layoutControlGroup, IView view)
      {
         var group = layoutControlGroup.AddGroup();
         return AddViewToGroup(group, view);
      }

      protected LayoutControlItem AddViewToGroup(LayoutControlGroup group, IView view)
      {
         var layoutControlItem = group.AddItem();
         group.Text = view.Caption;
         if (view.ApplicationIcon != null)
            group.CaptionImageIndex = ApplicationIcons.IconIndex(view.ApplicationIcon);

         AddViewTo(layoutControlItem, view);

         layoutControlItem.TextVisible = false;

         return layoutControlItem;
      }

      protected void AddViewTo(LayoutControlItem layoutControlItem, IView view)
      {
         if (layoutControlItem.Control != null)
            layoutControlItem.Control.FillWith(view);
         else
            layoutControlItem.Control = view.DowncastTo<Control>();

         var resizableView = view as IResizableView;
         if (resizableView == null) return;

         layoutControlItem.SizeConstraintsType = SizeConstraintsType.Custom;
         resizableView.HeightChanged += (o, e) => { AdjustLayoutItemSize(layoutControlItem, resizableView, e.Height); };
      }

      protected virtual void AdjustLayoutItemSize(LayoutControlItem layoutControlItem, IResizableView view, int height)
      {
         if (layoutControlItem == null || layoutControlItem.Parent == null)
            return;

         try
         {
            layoutControlItem.Owner.BeginUpdate();
            layoutControlItem.AdjustControlHeight(height);
            var group = layoutControlItem.Parent;
            //only one item in the group and the view should be hidden
            group.Visibility = LayoutVisibilityConvertor.FromBoolean(height > 0 || group.Items.Count > 1);
            view.Repaint();
         }
         finally
         {
            layoutControlItem.Owner.EndUpdate();
         }
      }

      public void AddView(IView view)
      {
         var group = layoutControl.Root.AddGroup();
         var layoutItem = AddViewToGroup(group, view);
         var resizeView = view as IResizableWithDefaultHeightView;
         if (resizeView != null)
            this.AdjustLayoutItemSize(layoutItem, resizeView, resizeView.DefaultHeight);

         group.ExpandButtonVisible = true;
      }

      public void FinishedAddingViews()
      {
         layoutControl.ResumeLayout();
      }
   }
}