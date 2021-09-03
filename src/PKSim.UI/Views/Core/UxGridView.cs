using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using OSPSuite.Utility.Extensions;
using PKSim.Core;

namespace PKSim.UI.Views.Core
{
   public class UxGridView : OSPSuite.UI.Controls.UxGridView
   {
      public UxGridView(GridControl ownerGrid) : base(ownerGrid)
      {
      }

      public UxGridView()
      {
      }

      protected override void DoInit()
      {
         base.DoInit();
         _colorDisabled = PKSimColors.Disabled;
         EditorShowMode = EditorShowMode.Default;
         ShowColumnChooser = true;
         OptionsNavigation.AutoFocusNewRow = true;
      }

   }
}