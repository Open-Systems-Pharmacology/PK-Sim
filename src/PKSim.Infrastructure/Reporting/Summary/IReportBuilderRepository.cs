using OSPSuite.Utility;
using OSPSuite.Utility.Container;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public interface IReportBuilderRepository : IBuilderRepository<IReportBuilder>
   {
   }

   public class ReportBuilderRepository : BuilderRepository<IReportBuilder>, IReportBuilderRepository
   {
      public ReportBuilderRepository(IContainer container) : base(container, typeof(IReportBuilder<>))
      {
      }
   }
}