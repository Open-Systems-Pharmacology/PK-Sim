using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public abstract class ParameterMetaDataRepository<TParameterMetaData> : StartableRepository<TParameterMetaData>, IParameterMetaDataRepository<TParameterMetaData> where TParameterMetaData : ParameterMetaData
   {
      private readonly IMetaDataRepository<TParameterMetaData> _flatParameterMetaDataRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatValueOriginRepository _flatValueOriginRepository;
      private readonly IFlatValueOriginToValueOriginMapper _valueOriginMapper;
      protected IList<TParameterMetaData> _parameterMetaDataList;
      private readonly ICache<string, IList<TParameterMetaData>> _parameterMetaDataCache = new Cache<string, IList<TParameterMetaData>>(s => new List<TParameterMetaData>());

      protected ParameterMetaDataRepository(
         IMetaDataRepository<TParameterMetaData> flatParameterMetaDataRepository, 
         IFlatContainerRepository flatContainerRepository, 
         IFlatValueOriginRepository flatValueOriginRepository,
         IFlatValueOriginToValueOriginMapper valueOriginMapper)
      {
         _flatParameterMetaDataRepository = flatParameterMetaDataRepository;
         _flatContainerRepository = flatContainerRepository;
         _flatValueOriginRepository = flatValueOriginRepository;
         _valueOriginMapper = valueOriginMapper;
      }

      public IEnumerable<TParameterMetaData> AllFor(string containerPath)
      {
         Start();
         return _parameterMetaDataCache[containerPath];
      }

      protected override void DoStart()
      {
         _parameterMetaDataList = _flatParameterMetaDataRepository.All().ToList();

         _parameterMetaDataList.Each(parameterMetaData =>
         {
            parameterMetaData.ParentContainerPath = _flatContainerRepository.ContainerPathFrom(parameterMetaData.ContainerId).ToString();
            parameterMetaData.ValueOrigin = _valueOriginMapper.MapFrom(_flatValueOriginRepository.FindBy(parameterMetaData.ValueOriginId));
         });

         //now cache the _parameter values
         foreach (var parameterValueMetaDataGroup in _parameterMetaDataList.GroupBy(x => x.ParentContainerPath))
         {
            _parameterMetaDataCache.Add(parameterValueMetaDataGroup.Key, new List<TParameterMetaData>(parameterValueMetaDataGroup));
         }
      }

      public override IEnumerable<TParameterMetaData> All()
      {
         Start();
         return _parameterMetaDataList;
      }
   }
}