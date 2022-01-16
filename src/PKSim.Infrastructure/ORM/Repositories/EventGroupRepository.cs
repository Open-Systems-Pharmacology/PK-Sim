using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class EventGroupRepository : StartableRepository<IEventGroupBuilder>, IEventGroupRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly IFlatContainerToEventGroupBuilderMapper _eventGroupMapper;
      private readonly ICache<string, IEventGroupBuilder> _eventGroupBuilders;

      public EventGroupRepository(
         IFlatContainerRepository flatContainerRepo, 
         IFlatContainerToEventGroupBuilderMapper eventGroupMapper)
      {
         _flatContainerRepo = flatContainerRepo;
         _eventGroupMapper = eventGroupMapper;
         _eventGroupBuilders = new Cache<string, IEventGroupBuilder>(eb => eb.Name);
      }

      public override IEnumerable<IEventGroupBuilder> All()
      {
         Start();
         return _eventGroupBuilders;
      }

      protected override void DoStart()
      {
         foreach (var flatEventGroupContainer in _flatContainerRepo.All()
            .Where(c => c.Type.Equals(CoreConstants.ContainerType.EVENT_GROUP) &&
                        !c.ParentId.HasValue))
         {
            var eventGroupBuilder = _eventGroupMapper.MapFrom(flatEventGroupContainer);

            _eventGroupBuilders.Add(eventGroupBuilder);
         }
      }
   }
}