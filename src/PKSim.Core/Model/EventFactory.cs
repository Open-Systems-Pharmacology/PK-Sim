using System;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Model
{
   public interface IEventFactory
   {
      PKSimEvent Create();
      PKSimEvent Create(string eventName);
      PKSimEvent Create(IEventGroupBuilder eventGroupBuilder);
   }

   public class EventFactory : IEventFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IEventGroupRepository _eventGroupRepository;
      private readonly ICloner _cloner;

      public EventFactory(IObjectBaseFactory objectBaseFactory, IEventGroupRepository eventGroupRepository, ICloner cloner)
      {
         _objectBaseFactory = objectBaseFactory;
         _eventGroupRepository = eventGroupRepository;
         _cloner = cloner;
      }

      public PKSimEvent Create()
      {
         return Create(_eventGroupRepository.All().FirstOrDefault());
      }

      public PKSimEvent Create(string eventName)
      {
         return Create(_eventGroupRepository.FindByName(eventName));
      }

      public PKSimEvent Create(IEventGroupBuilder eventGroupBuilder)
      {
         if (eventGroupBuilder == null)
            throw new ArgumentException("Event template not defined", "eventGroupBuilder");

         var newEvent = _objectBaseFactory.Create<PKSimEvent>();
         newEvent.TemplateName = eventGroupBuilder.Name;
         var clonedEvent = _cloner.Clone(eventGroupBuilder);

         foreach (var parameter in clonedEvent.GetAllChildren<IParameter>())
         {
            newEvent.Add(parameter);
         }
         newEvent.IsLoaded = true;
         return newEvent;
      }
   }
}