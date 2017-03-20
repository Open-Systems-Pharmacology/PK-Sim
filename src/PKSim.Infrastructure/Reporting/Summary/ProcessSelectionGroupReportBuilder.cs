using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ProcessSelectionGroupReportBuilder : ReportBuilder<ProcessSelectionGroup>
   {
      protected override void FillUpReport(ProcessSelectionGroup processSelectionGroup, ReportPart reportPart)
      {
         foreach(var proc in processSelectionGroup.AllEnabledProcesses())
         {
            var systemicProc = proc as SystemicProcessSelection;
            if(systemicProc!=null)
               reportPart.AddToContent("Using {0} {1}", systemicProc.ProcessType.DisplayName.ToLower(), systemicProc.ProcessName);
            else
               reportPart.AddToContent("Mapping {0} with {1}", proc.MoleculeName, proc.ProcessName);
         }
      }
   }
}