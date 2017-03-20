using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IOldCalculationMethodNameMappingRepository : IRepository<KeyValuePair<string, string>>
   {
      string OldCalculationMethodNameFrom(string newCalculationMethodName);
      string NewCalculationMethodNameFrom(string oldCalculationMethodName);
   }

   public class OldCalculationMethodNameMappingRepository : StartableRepository<KeyValuePair<string, string>>, IOldCalculationMethodNameMappingRepository
   {
      private readonly IFlatOldCalculationMethodNameMappingRepository _flatOldCalculationMethodNameMappingRepository;
      private readonly ICache<string, string> _calculationMethodNamesWithOldNameAsKey;
      private readonly ICache<string, string> _calculationMethodNamesWithNewNameAsKey;

      public OldCalculationMethodNameMappingRepository(IFlatOldCalculationMethodNameMappingRepository flatOldCalculationMethodNameMappingRepository)
      {
         _flatOldCalculationMethodNameMappingRepository = flatOldCalculationMethodNameMappingRepository;
         _calculationMethodNamesWithOldNameAsKey = new Cache<string, string>(s => s, s => s);
         _calculationMethodNamesWithNewNameAsKey = new Cache<string, string>(s => s, s => s);
      }

      public override IEnumerable<KeyValuePair<string, string>> All()
      {
         Start();
         return _calculationMethodNamesWithOldNameAsKey.KeyValues;
      }

      protected override void DoStart()
      {
         foreach (var flatMapping in _flatOldCalculationMethodNameMappingRepository.All())
         {
            _calculationMethodNamesWithOldNameAsKey.Add(flatMapping.OldName, flatMapping.NewName);
            _calculationMethodNamesWithNewNameAsKey.Add(flatMapping.NewName, flatMapping.OldName);
         }
      }

      public string OldCalculationMethodNameFrom(string newCalculationMethodName)
      {
         Start();
         return _calculationMethodNamesWithNewNameAsKey[newCalculationMethodName];
      }

      public string NewCalculationMethodNameFrom(string oldCalculationMethodName)
      {
         Start();
         return _calculationMethodNamesWithOldNameAsKey[oldCalculationMethodName];
      }
   }
}