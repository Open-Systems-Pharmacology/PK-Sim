using System.Collections.Generic;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class DynamicFormulaCriteriaRepository : StartableRepository<DynamicFormulaCriteria>, IDynamicFormulaCriteriaRepository
   {
      private readonly IFlatDynamicFormulaCriteriaRepository _flatDynamicFormulaCriteriaRepository;
      private readonly ICriteriaConditionToDescriptorConditionMapper _descriptorConditionMapper;
      private readonly ICache<RateKey, DynamicFormulaCriteria> _criteria;

      public DynamicFormulaCriteriaRepository(
         IFlatDynamicFormulaCriteriaRepository flatDynamicFormulaCriteriaRepository,
         ICriteriaConditionToDescriptorConditionMapper descriptorConditionMapper)
      {
         _flatDynamicFormulaCriteriaRepository = flatDynamicFormulaCriteriaRepository;
         _descriptorConditionMapper = descriptorConditionMapper;
         _criteria = new Cache<RateKey, DynamicFormulaCriteria>(dc => dc.RateKey);
      }

      protected override void DoStart()
      {
         foreach (var flatFormulaCriteria in _flatDynamicFormulaCriteriaRepository.All())
         {
            var rateKey = new RateKey(flatFormulaCriteria.CalculationMethod, flatFormulaCriteria.Rate);

            if (!_criteria.Contains(rateKey))
               _criteria.Add(new DynamicFormulaCriteria(rateKey));

            var criteria = _criteria[rateKey];
            criteria.DescriptorCriteria.Add(_descriptorConditionMapper.MapFrom(flatFormulaCriteria.Condition, flatFormulaCriteria.Tag));
         }
      }

      public override IEnumerable<DynamicFormulaCriteria> All()
      {
         Start();
         return _criteria;
      }

      public DescriptorCriteria CriteriaFor(RateKey rateKey)
      {
         Start();
         return _criteria.Contains(rateKey) ? _criteria[rateKey].DescriptorCriteria :  new DescriptorCriteria();
      }

      public DescriptorCriteria CriteriaFor(string calculationMethod, string rate)
      {
         return CriteriaFor(new RateKey(calculationMethod, rate));
      }
   }
}