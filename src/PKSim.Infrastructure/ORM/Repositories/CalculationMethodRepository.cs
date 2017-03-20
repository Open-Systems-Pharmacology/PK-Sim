using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CalculationMethodRepository : StartableRepository<CalculationMethod>, ICalculationMethodRepository
   {
      private readonly IFlatCalculationMethodRepository _flatCalculationMethodRepository;
      private readonly IFlatSpeciesCalculationMethodRepository _flatSpeciesCalculationMethodRepository;
      private readonly IFlatModelCalculationMethodRepository _flatModelCalculationMethodRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICache<string, CalculationMethod> _calculationMethods = new Cache<string, CalculationMethod>(cm => cm.Name);

      public CalculationMethodRepository(IFlatCalculationMethodRepository flatCalculationMethodRepository,
                                         IFlatSpeciesCalculationMethodRepository flatSpeciesCalculationMethodRepository,
                                         IFlatModelCalculationMethodRepository flatModelCalculationMethodRepository,
                                         IRepresentationInfoRepository representationInfoRepository)
      {
         _flatCalculationMethodRepository = flatCalculationMethodRepository;
         _flatSpeciesCalculationMethodRepository = flatSpeciesCalculationMethodRepository;
         _flatModelCalculationMethodRepository = flatModelCalculationMethodRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public override IEnumerable<CalculationMethod> All()
      {
         Start();
         return _calculationMethods;
      }

      protected override void DoStart()
      {
         _flatCalculationMethodRepository.All().Each(cm => _calculationMethods.Add(mapFrom(cm)));
      }

      private CalculationMethod mapFrom(FlatCalculationMethod flatCalculationMethod)
      {
         var cm = new CalculationMethod {Category = flatCalculationMethod.Category, Name = flatCalculationMethod.Id};
         cm.DisplayName = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.CALCULATION_METHOD, cm.Name);
         _flatSpeciesCalculationMethodRepository.SpeciesListFor(cm.Name).Each(cm.AddSpecies);
         _flatModelCalculationMethodRepository.ModelListFor(cm.Name).Each(cm.AddModel);
         return cm;
      }

      public CalculationMethod FindBy(string name)
      {
         Start();
         if (_calculationMethods.Contains(name))
            return _calculationMethods[name];

         //not licensed
         throw new CalculationMethodNotLicensedException(name);

      }
   }
}