using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;

namespace PKSim.Core
{
   public interface IProjectChangedNotifier
   {
      void Changed();

      /// <summary>
      /// On top of notifying the project change, this method sets the <c>HasChanged</c> flag to <c>true</c> for a <see cref="IPKSimBuildingBlock"/>
      /// </summary>
      /// <param name="objectThatHasChanged"></param>
      void NotifyChangedFor(object objectThatHasChanged);
   }

   public class ProjectChangedNotifier : IProjectChangedNotifier
   {
      private readonly IProjectRetriever _workspace;
      private readonly IEventPublisher _eventPublisher;

      public ProjectChangedNotifier(IProjectRetriever workspace, IEventPublisher eventPublisher)
      {
         _workspace = workspace;
         _eventPublisher = eventPublisher;
      }

      public void Changed()
      {
         if (!canRecordChanges()) return;
         currentProject.HasChanged = true;
         _eventPublisher.PublishEvent(new ProjectChangedEvent(currentProject));
      }

      private bool canRecordChanges()
      {
         return currentProject != null;
      }

      public void NotifyChangedFor(object objectThatHasChanged)
      {
         if (!canRecordChanges()) return;

         var buildingBlock = objectThatHasChanged as IPKSimBuildingBlock;
         if (buildingBlock != null)
            buildingBlock.HasChanged = true;

         Changed();
      }

      private IProject currentProject => _workspace.CurrentProject;
   }
}