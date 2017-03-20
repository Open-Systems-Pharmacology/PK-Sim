using OSPSuite.Core.Domain;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;

namespace PKSim.Infrastructure.Services
{
   public class ContentLoader : IContentLoader
   {
      private readonly IBuildingBlockMetaDataContentQuery _buildingBlockMetaDataContentQuery;
      private readonly ICompressedSerializationManager _serializationManager;

      public ContentLoader(IBuildingBlockMetaDataContentQuery buildingBlockMetaDataContentQuery,
                           ICompressedSerializationManager serializationManager)
      {
         _buildingBlockMetaDataContentQuery = buildingBlockMetaDataContentQuery;
         _serializationManager = serializationManager;
      }

      public void LoadContentFor<T>(T objectToLoad) where T : IObjectBase
      {
         var content = _buildingBlockMetaDataContentQuery.ResultFor(objectToLoad.Id);
         if (content == null) return;

         _serializationManager.Deserialize(objectToLoad, content.Data);
      }
   }
}