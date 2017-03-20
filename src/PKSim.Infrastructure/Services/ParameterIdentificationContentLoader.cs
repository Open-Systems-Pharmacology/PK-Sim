using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.Services
{
   public class ParameterIdentificationContentLoader : IParameterIdentificationContentLoader
   {
      private readonly IParameterIdentificationMetaDataContentQuery _parameterIdentificationMetaDataContentQuery;
      private readonly ICompressedSerializationManager _compressedSerializationManager;
      private readonly IProjectRetriever _projectRetriever;
      private readonly ISerializationContextFactory _serializationContextFactory;

      public ParameterIdentificationContentLoader(IParameterIdentificationMetaDataContentQuery parameterIdentificationMetaDataContentQuery, ICompressedSerializationManager compressedSerializationManager,
         IProjectRetriever projectRetriever, ISerializationContextFactory serializationContextFactory)
      {
         _parameterIdentificationMetaDataContentQuery = parameterIdentificationMetaDataContentQuery;
         _compressedSerializationManager = compressedSerializationManager;
         _projectRetriever = projectRetriever;
         _serializationContextFactory = serializationContextFactory;
      }

      public void LoadContentFor(ParameterIdentification parameterIdentification)
      {
         var content = _parameterIdentificationMetaDataContentQuery.ResultFor(parameterIdentification.Id);
         if (content == null) return;

         using (var context = _serializationContextFactory.Create(externalReferences: _projectRetriever.CurrentProject.All<ISimulation>()))
         {
            _compressedSerializationManager.Deserialize(parameterIdentification, content.Data, context);
         }
      }
   }
}