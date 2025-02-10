using System.Collections.Generic;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Infrastructure.ORM.Mappers;
using ICoreCalculationMethodRepository = PKSim.Core.Repositories.ICoreCalculationMethodRepository;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CoreCalculationMethodRepository : StartableRepository<CoreCalculationMethod>, ICoreCalculationMethodRepository
   {
      private readonly IFlatCalculationMethodParameterRateRepository _flatCalculationMethodParameterRateRepository;
      private readonly ICalculationMethodToCoreCalculationMethodMapper _calculationMethodMapper;

      private readonly IList<CoreCalculationMethod> _coreCalculationMethods;

      public CoreCalculationMethodRepository(IFlatCalculationMethodParameterRateRepository flatCalculationMethodParameterRateRepository,
         ICalculationMethodToCoreCalculationMethodMapper calculationMethodMapper)
      {
         _flatCalculationMethodParameterRateRepository = flatCalculationMethodParameterRateRepository;
         _calculationMethodMapper = calculationMethodMapper;
         _coreCalculationMethods = new List<CoreCalculationMethod>();
      }

      public override IEnumerable<CoreCalculationMethod> All()
      {
         Start();
         return _coreCalculationMethods;
      }

      protected override void DoStart()
      {
         _flatCalculationMethodParameterRateRepository.AllCalculationMethodNames
            .Each(cm => _coreCalculationMethods.Add(_calculationMethodMapper.MapFrom(cm)));
      }
   }
}