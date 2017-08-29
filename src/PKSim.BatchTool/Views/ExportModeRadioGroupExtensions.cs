using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool.Views
{
   public static class ExportModeRadioGroupExtensions
   {
      public static void AddExportModes(this RadioGroup radioGroup, LayoutControlItem layoutControlItem)
      {
         radioGroup.Properties.Items.AddRange(new[]
         {
            new RadioGroupItem(SnapshotExportMode.Project, Captions.SnapshotExportModeProject),
            new RadioGroupItem(SnapshotExportMode.Snapshot, Captions.SnapshotExportModeSnapshot)
         });

         layoutControlItem.AdjustControlHeight(UIConstants.Size.RADIO_GROUP_HEIGHT);
      }
   }
}