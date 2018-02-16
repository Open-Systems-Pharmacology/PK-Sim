using OSPSuite.Core.Domain.ParameterIdentifications;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;

namespace PKSim.Infrastructure.Services
{
   public class ParameterIdentificationContentLoader : IParameterIdentificationContentLoader
   {
      private readonly IParameterIdentificationMetaDataContentQuery _parameterIdentificationMetaDataContentQuery;
      private readonly ICompressedSerializationManager _compressedSerializationManager;

      public ParameterIdentificationContentLoader(
         IParameterIdentificationMetaDataContentQuery parameterIdentificationMetaDataContentQuery,
         ICompressedSerializationManager compressedSerializationManager
      )
      {
         _parameterIdentificationMetaDataContentQuery = parameterIdentificationMetaDataContentQuery;
         _compressedSerializationManager = compressedSerializationManager;
      }

      public void LoadContentFor(ParameterIdentification parameterIdentification)
      {
         var content = _parameterIdentificationMetaDataContentQuery.ResultFor(parameterIdentification.Id);
         if (content == null) return;

         _compressedSerializationManager.Deserialize(parameterIdentification, content.Data);
      }
   }
}