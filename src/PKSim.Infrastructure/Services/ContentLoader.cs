using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;

namespace PKSim.Infrastructure.Services
{
   public class ContentLoader : IContentLoader
   {
      private readonly IBuildingBlockMetaDataContentQuery _buildingBlockMetaDataContentQuery;
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public ContentLoader(IBuildingBlockMetaDataContentQuery buildingBlockMetaDataContentQuery,
         ICompressedSerializationManager serializationManager,
         IObjectTypeResolver objectTypeResolver)
      {
         _buildingBlockMetaDataContentQuery = buildingBlockMetaDataContentQuery;
         _serializationManager = serializationManager;
         _objectTypeResolver = objectTypeResolver;
      }

      public void LoadContentFor<T>(T objectToLoad) where T : IObjectBase
      {
         var content = _buildingBlockMetaDataContentQuery.ResultFor(objectToLoad.Id);
         if (content == null) return;

         //The content row exists but holds no data (see issue 3861). The serializer cannot deal with that
         if (content.Data == null)
            throw new PKSimException(PKSimConstants.Error.BuildingBlockContentIsMissing(objectToLoad.Name, _objectTypeResolver.TypeFor((IObjectBase) objectToLoad)));

         _serializationManager.Deserialize(objectToLoad, content.Data);
      }
   }
}
