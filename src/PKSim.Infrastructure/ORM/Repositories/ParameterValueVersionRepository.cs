using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ParameterValueVersionRepository : StartableRepository<ParameterValueVersion>, IParameterValueVersionRepository
   {
      private readonly IFlatParameterValueVersionRepository _flatParameterValueVersionRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICache<string, ParameterValueVersion> _parameterValueVersions = new Cache<string, ParameterValueVersion>(pvv => pvv.Name, x => null);

      public ParameterValueVersionRepository(IFlatParameterValueVersionRepository flatParameterValueVersionRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _flatParameterValueVersionRepository = flatParameterValueVersionRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public override IEnumerable<ParameterValueVersion> All()
      {
         Start();
         return _parameterValueVersions;
      }

      protected override void DoStart()
      {
         var distinctParameterValueVersion = _flatParameterValueVersionRepository.All().Distinct(new PVVComparer());
         distinctParameterValueVersion.All().Each(pvv => _parameterValueVersions.Add(mapFrom(pvv)));
      }

      private ParameterValueVersion mapFrom(FlatParameterValueVersion flatPvv)
      {
         var pvv = new ParameterValueVersion {Category = flatPvv.Category, Name = flatPvv.Id};
         pvv.DisplayName = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.PARAMETER_VALUE_VERSION, pvv.Name);
         return pvv;
      }

      public ParameterValueVersion FindBy(string name)
      {
         Start();
         return _parameterValueVersions[name];
      }

      private class PVVComparer : IEqualityComparer<FlatParameterValueVersion>
      {
         public bool Equals(FlatParameterValueVersion x, FlatParameterValueVersion y)
         {
            return x.Id.Equals(y.Id);
         }

         public int GetHashCode(FlatParameterValueVersion obj)
         {
            return obj.Id.GetHashCode();
         }
      }
   }
}