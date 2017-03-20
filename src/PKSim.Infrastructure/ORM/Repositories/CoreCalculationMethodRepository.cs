using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;
using OSPSuite.Core.Domain.Builder;
using ICoreCalculationMethodRepository = PKSim.Core.Repositories.ICoreCalculationMethodRepository;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CoreCalculationMethodRepository : StartableRepository<ICoreCalculationMethod>, ICoreCalculationMethodRepository
   {
      private readonly IFlatCalculationMethodParameterRateRepository _flatCalculationMethodParameterRateRepository;
      private readonly ICalculationMethodToCoreCalculationMethodMapper _calculationMethodMapper;

      private readonly IList<ICoreCalculationMethod> _coreCalculationMethods;

      public CoreCalculationMethodRepository(IFlatCalculationMethodParameterRateRepository flatCalculationMethodParameterRateRepository,
         ICalculationMethodToCoreCalculationMethodMapper calculationMethodMapper)
      {
         _flatCalculationMethodParameterRateRepository = flatCalculationMethodParameterRateRepository;
         _calculationMethodMapper = calculationMethodMapper;
         _coreCalculationMethods = new List<ICoreCalculationMethod>();
      }

      public override IEnumerable<ICoreCalculationMethod> All()
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