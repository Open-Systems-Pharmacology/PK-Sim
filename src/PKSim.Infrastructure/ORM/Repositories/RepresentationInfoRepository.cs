using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class RepresentationInfoRepository : MetaDataRepository<RepresentationInfo>, IRepresentationInfoRepository
   {
      private readonly ITypeToRepresentationObjectTypeMapper _representationObjectTypeMapper;
      private readonly Cache<string, RepresentationInfo> _representationInfosCache;

      public RepresentationInfoRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<RepresentationInfo> mapper, ITypeToRepresentationObjectTypeMapper representationObjectTypeMapper) :
         base(dbGateway, mapper, CoreConstants.ORM.ViewRepresentationInfos)
      {
         _representationObjectTypeMapper = representationObjectTypeMapper;
         _representationInfosCache = new Cache<string, RepresentationInfo>(getKey, x => null);
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var representationInfo in AllElements())
         {
            var key = getKey(representationInfo);
            _representationInfosCache[key] = representationInfo;
         }
      }

      public IEnumerable<RepresentationInfo> AllOfType(RepresentationObjectType objectType)
      {
         return from reprInfo in All()
            where reprInfo.ObjectType == objectType
            select reprInfo;
      }

      public RepresentationInfo InfoFor(RepresentationObjectType objectType, string objectName)
      {
         var key = getKey(objectType, objectName);
         Start();
         return _representationInfosCache[key] ?? new RepresentationInfo {DisplayName = objectName};
      }

      public RepresentationInfo InfoFor(IObjectBase objectBase)
      {
         var info = InfoFor(representationTypeFor(objectBase), objectBase.Name);
         if (string.IsNullOrEmpty(info.IconName))
            info.IconName = objectBase.Icon;

         return info;
      }

      public RepresentationInfo ContainerInfoFor(string objectName) => InfoFor(RepresentationObjectType.CONTAINER, objectName);

      public string DisplayNameFor(IObjectBase objectBase) => InfoFor(objectBase).DisplayName;

      public string DisplayNameFor(StatisticalAggregation statisticalAggregation) => DisplayNameFor(RepresentationObjectType.CURVE_SELECTION, statisticalAggregation.Id);

      public string DescriptionFor(IObjectBase objectBase) => InfoFor(objectBase).Description;

      public string DescriptionFor(RepresentationObjectType objectType, string objectName) => InfoFor(objectType, objectName).Description;

      public string DisplayNameFor(RepresentationObjectType objectType, string objectName) => InfoFor(objectType, objectName).DisplayName;

      public bool ContainsInfoFor(IObjectBase objectBase) => _representationInfosCache.Contains(getKey(objectBase));

      private string getKey(RepresentationObjectType objectType, string objectName) => $"{objectType}{ObjectPath.PATH_DELIMITER}{objectName}";

      private string getKey(RepresentationInfo representationInfo) => getKey(representationInfo.ObjectType, representationInfo.Name);

      private string getKey(IObjectBase objectBase) => getKey(representationTypeFor(objectBase), objectBase.Name);

      private RepresentationObjectType representationTypeFor(IObjectBase objectBase) => _representationObjectTypeMapper.MapFrom(objectBase.GetType());
   }
}