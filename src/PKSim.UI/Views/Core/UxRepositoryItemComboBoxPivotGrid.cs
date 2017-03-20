using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;

namespace PKSim.UI.Views.Core
{
   public class UxRepositoryItemComboBoxPivotGrid : RepositoryItemComboBox
   {
      public UxRepositoryItemComboBoxPivotGrid(PivotGridControl pivotGrid)
      {
         TextEditStyle = TextEditStyles.DisableTextEditor;
         EditValueChanged += (o, e) => pivotGrid.PostEditor();
      }
   }
}