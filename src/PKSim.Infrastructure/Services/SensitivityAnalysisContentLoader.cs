using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;

namespace PKSim.Infrastructure.Services
{
   public class SensitivityAnalysisContentLoader : ISensitivityAnalysisContentLoader
   {
      private readonly ISensitivityAnalysisMetaDataContentQuery _sensitivityAnalysisMetaDataContentQuery;
      private readonly ICompressedSerializationManager _compressedSerializationManager;

      public SensitivityAnalysisContentLoader(
         ISensitivityAnalysisMetaDataContentQuery sensitivityAnalysisMetaDataContentQuery, 
         ICompressedSerializationManager compressedSerializationManager)
      {
         _sensitivityAnalysisMetaDataContentQuery = sensitivityAnalysisMetaDataContentQuery;
         _compressedSerializationManager = compressedSerializationManager;
      }

      public void LoadContentFor(SensitivityAnalysis sensitivityAnalysis)
      {
         var content = _sensitivityAnalysisMetaDataContentQuery.ResultFor(sensitivityAnalysis.Id);

         if (content?.Data == null) return;

         _compressedSerializationManager.Deserialize(sensitivityAnalysis, content.Data);
      }
   }
}