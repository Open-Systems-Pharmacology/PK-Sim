using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatRateObjectPathRepository : IMetaDataRepository<FlatRateObjectPath>
   {
   }

   public class FlatRateObjectPathRepository : MetaDataRepository<FlatRateObjectPath>, IFlatRateObjectPathRepository
   {
      public FlatRateObjectPathRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatRateObjectPath> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_RATE_OBJECT_PATHS)
      {
      }
   }

   public class RateObjectPathsRepository : StartableRepository<IRateObjectPaths>, IRateObjectPathsRepository
   {
      private readonly IFlatRateObjectPathRepository _flatRateObjectPathRepo;
      private readonly IFlatRateObjectToFormulaUsablePathMapper _flatRateObjectToFormulaUsablePathMapper;
      private readonly ICache<RateKey, IRateObjectPaths> _rateObjectPaths;

      public RateObjectPathsRepository(IFlatRateObjectPathRepository flatRateObjectPathRepo,
                                       IFlatRateObjectToFormulaUsablePathMapper flatRateObjectToFormulaUsablePathMapper)
      {
         _rateObjectPaths = new Cache<RateKey, IRateObjectPaths>(rk => new NullObjectPaths());
         _flatRateObjectPathRepo = flatRateObjectPathRepo;
         _flatRateObjectToFormulaUsablePathMapper = flatRateObjectToFormulaUsablePathMapper;
      }

      public override IEnumerable<IRateObjectPaths> All()
      {
         Start();
         return _rateObjectPaths;
      }

      protected override void DoStart()
      {
         foreach (var flatRateObjectPath in _flatRateObjectPathRepo.All())
         {
            var rateObjectPaths = rateObjectPathsFor(flatRateObjectPath.CalculationMethod, flatRateObjectPath.Rate);

            var rateObjectPath = _flatRateObjectToFormulaUsablePathMapper.MapFrom(flatRateObjectPath);
            rateObjectPaths.AddObjectPath(rateObjectPath);
         }
      }

      private IRateObjectPaths rateObjectPathsFor(string calculationMethod, string rate)
      {
         var rateKey = new RateKey(calculationMethod, rate);

         if (_rateObjectPaths.Contains(rateKey))
            return _rateObjectPaths[rateKey];

         var rateObjectPaths = new RateObjectPaths(calculationMethod, rate);
         _rateObjectPaths.Add(rateKey, rateObjectPaths);
         return rateObjectPaths;
      }

      public IEnumerable<FormulaUsablePath> ObjectPathsFor(string rate, string calculationMethod)
      {
         return ObjectPathsFor(new RateKey(calculationMethod, rate));
      }

      public IEnumerable<FormulaUsablePath> ObjectPathsFor(RateKey rateKey)
      {
         Start();
         return _rateObjectPaths[rateKey].ObjectPaths;
      }

      public FormulaUsablePath PathWithAlias(RateKey rateKey, string alias)
      {
         return ObjectPathsFor(rateKey).FirstOrDefault(path => string.Equals(path.Alias, alias));
      }
   }
}