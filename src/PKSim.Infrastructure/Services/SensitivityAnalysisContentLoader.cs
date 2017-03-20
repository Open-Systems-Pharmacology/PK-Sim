using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.Services
{
   public class SensitivityAnalysisContentLoader : ISensitivityAnalysisContentLoader
   {
      private readonly ISensitivityAnalysisMetaDataContentQuery _sensitivityAnalysisMetaDataContentQuery;
      private readonly ICompressedSerializationManager _compressedSerializationManager;
      private readonly IProjectRetriever _projectRetriever;
      private readonly ISerializationContextFactory _serializationContextFactory;

      public SensitivityAnalysisContentLoader(ISensitivityAnalysisMetaDataContentQuery sensitivityAnalysisMetaDataContentQuery, ICompressedSerializationManager compressedSerializationManager,
         IProjectRetriever projectRetriever, ISerializationContextFactory serializationContextFactory)
      {
         _sensitivityAnalysisMetaDataContentQuery = sensitivityAnalysisMetaDataContentQuery;
         _compressedSerializationManager = compressedSerializationManager;
         _projectRetriever = projectRetriever;
         _serializationContextFactory = serializationContextFactory;
      }

      public void LoadContentFor(SensitivityAnalysis sensitivityAnalysis)
      {
         var content = _sensitivityAnalysisMetaDataContentQuery.ResultFor(sensitivityAnalysis.Id);

         if(content?.Data == null) return;

         using (var context = _serializationContextFactory.Create(externalReferences: _projectRetriever.CurrentProject.All<ISimulation>()))
         {
            _compressedSerializationManager.Deserialize(sensitivityAnalysis, content.Data, context);
         }
      }
   }
}