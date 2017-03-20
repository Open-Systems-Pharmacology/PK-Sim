using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class RepresentationInfoRepository : MetaDataRepository<RepresentationInfo>, IRepresentationInfoRepository
   {
      private readonly ITypeToRepresentationObjectTypeMapper _representationObjectTypeMapper;
      private ICache<string,RepresentationInfo> _representationInfosCache;

      public RepresentationInfoRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<RepresentationInfo> mapper, ITypeToRepresentationObjectTypeMapper representationObjectTypeMapper) :
         base(dbGateway, mapper, CoreConstants.ORM.ViewRepresentationInfos)
      {
         _representationObjectTypeMapper = representationObjectTypeMapper;
      }

      protected override void PerformPostStartProcessing()
      {
         _representationInfosCache = new Cache<string, RepresentationInfo>(getKey);

         foreach (var representationInfo in AllElements())
         {
            var key = getKey(representationInfo);
            if (!_representationInfosCache.Contains(key))
               _representationInfosCache.Add(key,representationInfo);
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
         Start();
         string key = getKey(objectType, objectName);
         if (_representationInfosCache.Contains(key))
            return _representationInfosCache[key];

         return new RepresentationInfo {DisplayName = objectName};
      }

      public RepresentationInfo ContainerInfoFor(string objectName)
      {
         return InfoFor(RepresentationObjectType.CONTAINER, objectName);
      }

      public RepresentationInfo InfoFor(IObjectBase objectBase)
      {
         var info= InfoFor(_representationObjectTypeMapper.MapFrom(objectBase.GetType()), objectBase.Name);
         if (string.IsNullOrEmpty(info.IconName))
            info.IconName = objectBase.Icon;

         return info;
      }

      public string DisplayNameFor(IObjectBase objectBase)
      {
         return InfoFor(objectBase).DisplayName;
      }

      public string DisplayNameFor(StatisticalAggregation statisticalAggregation)
      {
         return DisplayNameFor(RepresentationObjectType.CURVE_SELECTION, statisticalAggregation.Id);
      }

      public string DescriptionFor(IObjectBase objectBase)
      {
         return InfoFor(objectBase).Description;
      }

      public string DisplayNameFor(RepresentationObjectType objectType, string objectName)
      {
         return InfoFor(objectType, objectName).DisplayName;
      }

      public bool ContainsInfoFor(IObjectBase objectBase)
      {
         return _representationInfosCache.Contains(getKey(objectBase));
      }

      private string getKey(RepresentationObjectType objectType, string objectName)
      {
         return Enum.GetName(typeof (RepresentationObjectType), objectType) + ObjectPath.PATH_DELIMITER + objectName;
      }

      private string getKey(RepresentationInfo representationInfo)
      {
         return getKey(representationInfo.ObjectType, representationInfo.Name);
      }

      private string getKey(IObjectBase objectBase)
      {
         return getKey(_representationObjectTypeMapper.MapFrom(objectBase.GetType()), objectBase.Name);
      }
   }
}