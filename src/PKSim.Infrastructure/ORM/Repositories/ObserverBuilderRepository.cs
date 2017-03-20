using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{

   public class ObserverBuilderRepository : StartableRepository<IPKSimObserverBuilder>, IObserverBuilderRepository
   {
      private readonly IFlatObserverRepository _flatObserverRepository;
      private readonly IFlatObserverToObserverBuilderMapper _observerMapper;
      private readonly IFlatModelObserverRepository _modelObserversRepository;
      private readonly IList<IPKSimObserverBuilder> _observers = new List<IPKSimObserverBuilder>();

      public ObserverBuilderRepository(IFlatObserverRepository flatObserverRepository,
                                       IFlatObserverToObserverBuilderMapper observerMapper,
                                       IFlatModelObserverRepository modelObserversRepository)
      {
         _flatObserverRepository = flatObserverRepository;
         _observerMapper = observerMapper;
         _modelObserversRepository = modelObserversRepository;
      }

      public override IEnumerable<IPKSimObserverBuilder> All()
      {
         Start();
         return _observers;
      }

      protected override void DoStart()
      {
         foreach (var flatObserver in _flatObserverRepository.All())
         {
            _observers.Add(_observerMapper.MapFrom(flatObserver));
         }
      }


      public IEnumerable<IPKSimObserverBuilder> AllFor(ModelProperties modelProperties)
      {
         Start();

         return from pkSimObserverBuilder in All()
                where _modelObserversRepository.Contains(modelProperties.ModelConfiguration.ModelName, pkSimObserverBuilder.ObserverBuilder.Name)
                where modelProperties.ContainsCalculationMethod(pkSimObserverBuilder.CalculationMethod)
                select pkSimObserverBuilder;
      }
   }
}
