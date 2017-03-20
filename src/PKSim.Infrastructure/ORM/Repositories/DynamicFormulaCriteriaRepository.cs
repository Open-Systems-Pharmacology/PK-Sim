using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class DynamicFormulaCriteriaRepository : StartableRepository<DynamicFormulaCriteria>, IDynamicFormulaCriteriaRepository
   {
      private readonly IFlatDynamicFormulaCriteriaRepository _flatDynamicFormulaCriteriaRepository;
      private readonly ICache<RateKey, DynamicFormulaCriteria> _criteria;

      public DynamicFormulaCriteriaRepository(IFlatDynamicFormulaCriteriaRepository flatDynamicFormulaCriteriaRepository)
      {
         _flatDynamicFormulaCriteriaRepository = flatDynamicFormulaCriteriaRepository;
         _criteria=new Cache<RateKey, DynamicFormulaCriteria>(dc=>dc.RateKey);
      }

      protected override void DoStart()
      {
         foreach (var flatFormulaCriteria in _flatDynamicFormulaCriteriaRepository.All())
         {
            var rateKey = new RateKey(flatFormulaCriteria.CalculationMethod, flatFormulaCriteria.Rate);

            DynamicFormulaCriteria criteria;
            if (_criteria.Contains(rateKey))
            {
               criteria = _criteria[rateKey];
            }
            else
            {
               criteria = new DynamicFormulaCriteria(rateKey);
               _criteria.Add(criteria);
            }

            if (flatFormulaCriteria.ShouldHave)
               criteria.DescriptorCriteria.Add(new MatchTagCondition(flatFormulaCriteria.Tag));
            else
               criteria.DescriptorCriteria.Add(new NotMatchTagCondition(flatFormulaCriteria.Tag));
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

         if (!_criteria.Contains(rateKey))
            return new DescriptorCriteria();

         return _criteria[rateKey].DescriptorCriteria;
      }

      public DescriptorCriteria CriteriaFor(string calculationMethod, string rate)
      {
         return CriteriaFor(new RateKey(calculationMethod, rate));
      }
   }
}
