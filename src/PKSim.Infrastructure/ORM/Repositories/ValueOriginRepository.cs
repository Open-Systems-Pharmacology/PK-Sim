using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ValueOriginRepository : IValueOriginRepository
   {
      private readonly IFlatValueOriginRepository _flatValueOriginRepository;
      private readonly IFlatValueOriginToValueOriginMapper _valueOriginMapper;

      private static readonly ValueOrigin _defaultValueOrigin = new ValueOrigin
      {
         Method = ValueOriginDeterminationMethods.Undefined,
         Source = ValueOriginSources.Undefined
      };

      private readonly Cache<int, ValueOrigin> _allValueOrigins = new Cache<int, ValueOrigin>(onMissingKey: x => _defaultValueOrigin);

      public ValueOriginRepository(IFlatValueOriginRepository flatValueOriginRepository, IFlatValueOriginToValueOriginMapper valueOriginMapper)
      {
         _flatValueOriginRepository = flatValueOriginRepository;
         _valueOriginMapper = valueOriginMapper;
      }

      public IEnumerable<ValueOrigin> All()
      {
         Start();
         return _allValueOrigins;
      }

      public void Start()
      {
         _flatValueOriginRepository.All().Each(x => { _allValueOrigins[x.Id] = _valueOriginMapper.MapFrom(x); });
      }

      public ValueOrigin ValueOriginFor(IParameter parameter)
      {
         return FindBy(parameter?.ValueOrigin.Id);
      }

      public ValueOrigin FindBy(int? id)
      {
         Start();
         return id == null ? _defaultValueOrigin : _allValueOrigins[id.Value];
      }
   }
}