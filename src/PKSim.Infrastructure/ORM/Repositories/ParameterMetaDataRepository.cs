using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public abstract class ParameterMetaDataRepository<TParameterMetaData> : StartableRepository<TParameterMetaData>, IParameterMetaDataRepository<TParameterMetaData> where TParameterMetaData : ParameterMetaData
   {
      private readonly IMetaDataRepository<TParameterMetaData> _flatParameterMetaDataRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;
      protected IList<TParameterMetaData> _parameterMetaDataList;
      private readonly ICache<string, IList<TParameterMetaData>> _parameterMetaDataCache = new Cache<string, IList<TParameterMetaData>>(s => new List<TParameterMetaData>());

      protected ParameterMetaDataRepository(IMetaDataRepository<TParameterMetaData> flatParameterMetaDataRepository, IFlatContainerRepository flatContainerRepository)
      {
         _flatParameterMetaDataRepository = flatParameterMetaDataRepository;
         _flatContainerRepository = flatContainerRepository;
      }

      public IEnumerable<TParameterMetaData> AllFor(string containerPath)
      {
         Start();
         return _parameterMetaDataCache[containerPath];
      }

      protected override void DoStart()
      {
         _parameterMetaDataList = _flatParameterMetaDataRepository.All().ToList();

         foreach (var parameterMetaData in _parameterMetaDataList)
            parameterMetaData.ParentContainerPath = _flatContainerRepository.ContainerPathFrom(parameterMetaData.ContainerId).ToString();

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