using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using PKSim.UI.Views;
using static PKSim.UI.UIConstants.Size;

namespace PKSim.UI.Extensions
{
   public static class LayoutControlItemExtensions
   {
      public static void FillWith(this LayoutControlItem layoutControlItem, UxBuildingBlockSelection buildingBlockSelection)
      {
         var panel = layoutControlItem.Control as PanelControl;
         if (panel == null) return;
         panel.Margin = new Padding(0);
         panel.FillWith((IView) buildingBlockSelection);
         panel.BorderStyle = BorderStyles.NoBorder;
         var size = new Size(layoutControlItem.Size.Width, BUILDING_BLOCK_SELECTION_SIZE);
         layoutControlItem.SizeConstraintsType = SizeConstraintsType.Custom;
         layoutControlItem.MinSize = size;
         layoutControlItem.Size = size;
     }
   }
}