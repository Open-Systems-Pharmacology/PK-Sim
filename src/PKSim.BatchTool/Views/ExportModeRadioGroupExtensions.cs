using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using OSPSuite.CLI.Core.Services;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;

namespace PKSim.BatchTool.Views
{
   public static class ExportModeRadioGroupExtensions
   {
      public static void AddExportModes(this RadioGroup radioGroup, LayoutControlItem layoutControlItem, LayoutControl layoutControl)
      {
         radioGroup.Properties.Items.AddRange(new[]
         {
            new RadioGroupItem(SnapshotExportMode.Project, Captions.SnapshotExportModeProject),
            new RadioGroupItem(SnapshotExportMode.Snapshot, Captions.SnapshotExportModeSnapshot)
         });

         layoutControlItem.AdjustControlHeight(UIConstants.Size.RADIO_GROUP_HEIGHT, layoutControl);
      }
   }
}