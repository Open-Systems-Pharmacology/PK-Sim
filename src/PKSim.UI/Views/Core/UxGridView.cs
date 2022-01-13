using DevExpress.Utils;
using DevExpress.XtraGrid;
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