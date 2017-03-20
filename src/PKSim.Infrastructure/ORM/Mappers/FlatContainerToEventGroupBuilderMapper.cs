using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerToEventGroupBuilderMapper:IMapper<FlatContainer,IEventGroupBuilder>
   {}

   public class FlatContainerToEventGroupBuilderMapper :
      FlatContainerIdToContainerMapperBase<IEventGroupBuilder>,IFlatContainerToEventGroupBuilderMapper
   {
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly IFlatContainerToEventBuilderMapper _eventMapper;

      public FlatContainerToEventGroupBuilderMapper(IParameterContainerTask parameterContainerTask,
                                                    IFlatContainerRepository flatContainerRepo,
                                                    IFlatContainerToEventBuilderMapper eventMapper)
      {
         _parameterContainerTask = parameterContainerTask;
         _flatContainerRepo = flatContainerRepo;
         _eventMapper = eventMapper;
      }

      public IEventGroupBuilder MapFrom(FlatContainer eventGroupFlatContainer)
      {
         var eventGroup = MapCommonPropertiesFrom(eventGroupFlatContainer);

         foreach (var flatSubContainer in eventGroupSubContainers(eventGroupFlatContainer))
         {
            if (flatSubContainer.Type.Equals(CoreConstants.ContainerType.EventGroup))
            {
               eventGroup.Add(MapFrom(flatSubContainer));
               continue;
            }

            if (flatSubContainer.Type.Equals(CoreConstants.ContainerType.Event))
            {
               eventGroup.Add(_eventMapper.MapFrom(flatSubContainer));
               continue;
            }

            throw new PKSimException(PKSimConstants.Error.EventGroupSubContainerHasInvalidType);
         }

         addParametersTo(eventGroup);

         //every event group will be added into events-subcontainer of simulation
         eventGroup.SourceCriteria.Add(new MatchTagCondition(CoreConstants.Tags.EVENTS));

         return eventGroup;
      }

      private void addParametersTo(IContainer container)
      {
         _parameterContainerTask.AddEventParametersTo(container);

         foreach (var subContainer in container.GetChildren<IContainer>())
         {
            addParametersTo(subContainer);
         }
      }


      private IEnumerable<FlatContainer> eventGroupSubContainers(FlatContainer eventGroupFlatContainer)
      {
         return from flatContainer in _flatContainerRepo.All()
                let parentContainer = _flatContainerRepo.ParentContainerFrom(flatContainer.Id)
                where parentContainer != null
                where parentContainer.Id == eventGroupFlatContainer.Id
                select flatContainer;
      }

   }
}
