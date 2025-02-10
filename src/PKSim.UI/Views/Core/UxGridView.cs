using DevExpress.Utils;
using DevExpress.XtraGrid;

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
         EditorShowMode = EditorShowMode.Default;
         ShowColumnChooser = true;
         OptionsNavigation.AutoFocusNewRow = true;
      }
   }
}