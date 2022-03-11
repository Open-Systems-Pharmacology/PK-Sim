using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public abstract class ParameterMetaDataRepository<TParameterMetaData> : StartableRepository<TParameterMetaData>, IParameterMetaDataRepository<TParameterMetaData>
      where TParameterMetaData : ParameterMetaData
   {
      private readonly IMetaDataRepository<TParameterMetaData> _flatParameterMetaDataRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;
      protected readonly IValueOriginRepository _valueOriginRepository;
      private readonly IFlatContainerParameterDescriptorConditionRepository _flatContainerParameterDescriptorConditionRepository;
      protected IList<TParameterMetaData> _parameterMetaDataList;
      private readonly ICriteriaConditionToDescriptorConditionMapper _descriptorConditionMapper;

      private readonly ICache<string, List<TParameterMetaData>> _parameterMetaDataCacheByContainer = new Cache<string, List<TParameterMetaData>>(s => new List<TParameterMetaData>());

      protected ParameterMetaDataRepository(
         IMetaDataRepository<TParameterMetaData> flatParameterMetaDataRepository,
         IFlatContainerRepository flatContainerRepository,
         IValueOriginRepository valueOriginRepository,
         IFlatContainerParameterDescriptorConditionRepository flatContainerParameterDescriptorConditionRepository,
         ICriteriaConditionToDescriptorConditionMapper descriptorConditionMapper)
      {
         _flatParameterMetaDataRepository = flatParameterMetaDataRepository;
         _flatContainerRepository = flatContainerRepository;
         _valueOriginRepository = valueOriginRepository;
         _flatContainerParameterDescriptorConditionRepository = flatContainerParameterDescriptorConditionRepository;
         _descriptorConditionMapper = descriptorConditionMapper;
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

            //update only if available in db
            parameterMetaData.ContainerCriteria = containerCriteriaFor(parameterMetaData.ContainerId, parameterMetaData.ParameterName);
         });

         //now cache the parameter meta data by container path
         foreach (var parameterValueMetaDataGroup in _parameterMetaDataList.GroupBy(x => x.ParentContainerPath))
         {
            _parameterMetaDataCacheByContainer.Add(parameterValueMetaDataGroup.Key, new List<TParameterMetaData>(parameterValueMetaDataGroup));
         }
      }

      private DescriptorCriteria containerCriteriaFor(int containerId, string parameterName)
      {
         var allConditions = _flatContainerParameterDescriptorConditionRepository.All()
            .Where(x => x.ContainerId == containerId && x.ParameterName == parameterName)
            .ToList();

         if (!allConditions.Any())
            return null;

         var operators = allConditions.Select(x => x.Operator).Distinct().ToList();
         if (operators.Count != 1)
            throw new ArgumentException(PKSimConstants.Error.MultipleOperatorFoundForContainer(containerId, parameterName));

         var criteria = new DescriptorCriteria();
         allConditions.Each(x => criteria.Add(_descriptorConditionMapper.MapFrom(x.Condition, x.Tag)));
         criteria.Operator = allConditions.First().Operator;
         return criteria;
      }

      public override IEnumerable<TParameterMetaData> All()
      {
         Start();
         return _parameterMetaDataList;
      }
   }
}