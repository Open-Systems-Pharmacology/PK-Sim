using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Journal;

namespace PKSim.Core.Services
{
   public class RelatedItemDescriptionCreator : IRelatedItemDescriptionCreator,
      IVisitor<Simulation>,
      IVisitor<IObjectBase>
   {
      private readonly IReportGenerator _reportGenerator;
      private string _report;

      public RelatedItemDescriptionCreator(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      public string DescriptionFor<T>(T relatedObject)
      {
         try
         {
            this.Visit(relatedObject);
            return _report;
         }
         finally
         {
            _report = string.Empty;
         }
      }

      public void Visit(Simulation simulation)
      {
         var report = new ReportPart();
         report.AddPart(_reportGenerator.ReportFor(simulation.UsedBuildingBlocks));
         report.AddPart(_reportGenerator.ReportFor(simulation.Creation));
         _report = report.ToStringReport();
      }

      public void Visit(IObjectBase objectBase)
      {
         _report = _reportGenerator.StringReportFor(objectBase);
      }
   }
}