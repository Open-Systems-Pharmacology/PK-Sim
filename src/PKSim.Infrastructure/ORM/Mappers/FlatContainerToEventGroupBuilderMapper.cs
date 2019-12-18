using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerToEventGroupBuilderMapper : IMapper<FlatContainer, IEventGroupBuilder>
   {
   }

   public class FlatContainerToEventGroupBuilderMapper :
      FlatContainerIdToContainerMapperBase<IEventGroupBuilder>, IFlatContainerToEventGroupBuilderMapper
   {
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFlatContainerToEventBuilderMapper _eventMapper;

      public FlatContainerToEventGroupBuilderMapper(
         IParameterContainerTask parameterContainerTask,
         IFlatContainerToEventBuilderMapper eventMapper,
         IObjectBaseFactory objectBaseFactory, 
         IFlatContainerRepository flatContainerRepository, 
         IFlatContainerTagRepository flatContainerTagRepository) : base(objectBaseFactory, flatContainerRepository, flatContainerTagRepository)
      {
         _parameterContainerTask = parameterContainerTask;
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
         return from flatContainer in _flatContainerRepository.All()
            let parentContainer = _flatContainerRepository.ParentContainerFrom(flatContainer.Id)
            where parentContainer != null
            where parentContainer.Id == eventGroupFlatContainer.Id
            select flatContainer;
      }
   }
}