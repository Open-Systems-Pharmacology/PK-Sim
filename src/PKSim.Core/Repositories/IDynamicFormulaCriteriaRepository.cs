using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public class DynamicFormulaCriteria
   {
      public RateKey RateKey { get; set; }
      public DescriptorCriteria DescriptorCriteria { get; set; }

      public DynamicFormulaCriteria(RateKey rateKey)
      {
         RateKey = rateKey;
         DescriptorCriteria = new DescriptorCriteria();
      }
   }

   public interface IDynamicFormulaCriteriaRepository : IStartableRepository<DynamicFormulaCriteria>
   {
      DescriptorCriteria CriteriaFor(RateKey rateKey);
      DescriptorCriteria CriteriaFor(string calculationMethod, string rate);
   }
}