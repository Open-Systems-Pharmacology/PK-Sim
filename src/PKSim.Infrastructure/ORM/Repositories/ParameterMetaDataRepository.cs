using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public abstract class ParameterMetaDataRepository<TParameterMetaData> : StartableRepository<TParameterMetaData>, IParameterMetaDataRepository<TParameterMetaData>
      where TParameterMetaData : ParameterMetaData
   {
      private readonly IMetaDataRepository<TParameterMetaData> _flatParameterMetaDataRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;
      protected readonly IValueOriginRepository _valueOriginRepository;
      protected IList<TParameterMetaData> _parameterMetaDataList;

      //TODO ZTMSE: Back to private
      protected readonly ICache<string, List<TParameterMetaData>> _parameterMetaDataCacheByContainer = new Cache<string, List<TParameterMetaData>>(s => new List<TParameterMetaData>());

      protected ParameterMetaDataRepository(
         IMetaDataRepository<TParameterMetaData> flatParameterMetaDataRepository,
         IFlatContainerRepository flatContainerRepository,
         IValueOriginRepository valueOriginRepository)
      {
         _flatParameterMetaDataRepository = flatParameterMetaDataRepository;
         _flatContainerRepository = flatContainerRepository;
         _valueOriginRepository = valueOriginRepository;
      }

      public IReadOnlyList<TParameterMetaData> AllFor(string containerPath)
      {
         Start();
         return _parameterMetaDataCacheByContainer[containerPath];
      }

      public TParameterMetaData ParameterMetaDataFor(string containerPath, string parameterName)
      {
         return AllFor(containerPath).Find(x => string.Equals(x.ParameterName, parameterName));
      }

      protected override void DoStart()
      {
         _parameterMetaDataList = _flatParameterMetaDataRepository.All().ToList();

         _parameterMetaDataList.Each(parameterMetaData =>
         {
            parameterMetaData.ParentContainerPath = _flatContainerRepository.ContainerPathFrom(parameterMetaData.ContainerId).ToString();
            //Use clone here to ensure that we are not modifying the reference stored in the repository
            var valueOrigin = _valueOriginRepository.FindBy(parameterMetaData.ValueOriginId).Clone();
            parameterMetaData.ValueOrigin = valueOrigin;
         });

         //now cache the parameter meta data by container path
         foreach (var parameterValueMetaDataGroup in _parameterMetaDataList.GroupBy(x => x.ParentContainerPath))
         {
            _parameterMetaDataCacheByContainer.Add(parameterValueMetaDataGroup.Key, new List<TParameterMetaData>(parameterValueMetaDataGroup));            
         }


      }

      public override IEnumerable<TParameterMetaData> All()
      {
         Start();
         return _parameterMetaDataList;
      }
   }
}