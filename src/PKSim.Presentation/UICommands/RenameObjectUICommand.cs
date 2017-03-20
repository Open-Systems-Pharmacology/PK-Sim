using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class RenameObjectUICommand<T> : ObjectUICommand<T> where T: class, IWithName
   {
      private readonly IEntityTask _entityTask;
      private readonly IEventPublisher _eventPublisher;

      public RenameObjectUICommand(IEntityTask entityTask, IEventPublisher eventPublisher)
      {
         _entityTask = entityTask;
         _eventPublisher = eventPublisher;
      }

      protected override void PerformExecute()
      {
         var newName = _entityTask.NewNameFor(Subject, ForbiddenNamesFor(Subject));
         if (string.IsNullOrEmpty(newName)) return;

         Subject.Name = newName;
         _eventPublisher.PublishEvent(new RenamedEvent(Subject));
      }

      protected virtual IEnumerable<string> ForbiddenNamesFor(T subject)
      {
         return Enumerable.Empty<string>();
      }
   }

   public class RenameObjectUICommand : RenameObjectUICommand<IWithName>
   {
      public RenameObjectUICommand(IEntityTask entityTask, IEventPublisher eventPublisher) : base(entityTask, eventPublisher)
      {
      }
   }
}