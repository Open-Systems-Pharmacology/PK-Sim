using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatModelObserverRepository : IMetaDataRepository<FlatModelObserver>
   {
      bool Contains(string modelName, string observerName);
   }

   public class FlatModelObserverRepository : MetaDataRepository<FlatModelObserver>, IFlatModelObserverRepository
   {
      private readonly ICache<string, ICache<string, FlatModelObserver>> _observersSortedByModel;

      public FlatModelObserverRepository(IDbGateway dbGateway,
         IDataTableToMetaDataMapper<FlatModelObserver> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewModelObservers)
      {
         _observersSortedByModel = new Cache<string, ICache<string, FlatModelObserver>>();
      }

      public bool Contains(string modelName, string observerName)
      {
         Start();

         return _observersSortedByModel[modelName].Contains(observerName);
      }

      protected override void PerformPostStartProcessing()
      {
         var allModels = AllElements().Select(x => x.Model).Distinct();
         allModels.Each(fillObserversForModel);
      }

      private void fillObserversForModel(string modelName)
      {
         var observersForModel = new Cache<string, FlatModelObserver>(fp => fp.Observer);

         foreach (var flatModelObserver in All().Where(x => x.Model == modelName))
         {
            observersForModel.Add(flatModelObserver);
         }

         _observersSortedByModel.Add(modelName, observersForModel);
      }
   }
}