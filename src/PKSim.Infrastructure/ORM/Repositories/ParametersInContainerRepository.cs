using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatParameterInContainsRepository : IMetaDataRepository<ParameterMetaData>
   {
   }

   public class FlatParameterInContainsRepository : MetaDataRepository<ParameterMetaData>, IFlatParameterInContainsRepository
   {
      public FlatParameterInContainsRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<ParameterMetaData> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewParametersInContainers)
      {
      }
   }

   public class ParametersInContainerRepository : ParameterMetaDataRepository<ParameterMetaData>, IParametersInContainerRepository
   {
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly Cache<string, ParameterMetaData> _parameterMetaDataCache = new Cache<string, ParameterMetaData>(onMissingKey: x => null);

      public ParametersInContainerRepository(
         IFlatParameterInContainsRepository flatParameterInContainsRepository,
         IFlatContainerRepository flatContainerRepository,
         IFlatValueOriginRepository flatValueOriginRepository,
         IFlatValueOriginToValueOriginMapper valueOriginMapper,
         IEntityPathResolver entityPathResolver)
         : base(flatParameterInContainsRepository, flatContainerRepository, flatValueOriginRepository, valueOriginMapper)
      {
         _entityPathResolver = entityPathResolver;
      }

      public ParameterMetaData ParameterMetaDataFor(IParameter parameter)
      {
         if (parameter == null)
            return null;

         return ParameterMetaDataFor(_entityPathResolver.PathFor(parameter));
      }

      protected override void DoStart()
      {
         base.DoStart();
         _parameterMetaDataList.Each(x => _parameterMetaDataCache.Add($"{x.ParentContainerPath}{ObjectPath.PATH_DELIMITER}{x.ParameterName}", x));
      }

      public ParameterMetaData ParameterMetaDataFor(string parameterPath)
      {
         Start();
         return _parameterMetaDataCache[parameterPath];
      }
   }
}