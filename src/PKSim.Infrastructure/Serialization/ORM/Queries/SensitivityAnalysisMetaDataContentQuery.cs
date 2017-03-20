using OSPSuite.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Infrastructure.Serialization.ORM.Queries
{
   public interface ISensitivityAnalysisMetaDataContentQuery : IQuery<MetaDataContent, string>
   {
   }

   public class SensitivityAnalysisMetaDataContentQuery : MetaDataContentQuery, ISensitivityAnalysisMetaDataContentQuery
   {
      public SensitivityAnalysisMetaDataContentQuery(ISessionManager sessionManager)
         : base(sessionManager, "SensitivityAnalysisMetaData")
      {
      }
   }
}