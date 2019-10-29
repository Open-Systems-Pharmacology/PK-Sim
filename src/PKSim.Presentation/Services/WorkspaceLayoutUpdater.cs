using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Events;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public interface IWorkspaceLayoutUpdater
   {
      void SaveCurrentLayout();
      void RestoreLayout();
   }

   public class WorkspaceLayoutUpdater : IWorkspaceLayoutUpdater
   {
      private readonly IApplicationController _applicationController;
      private readonly IWithIdRepository _withIdRepository;
      private readonly IOpenSingleStartPresenterInvoker _openSingleStartPresenterInvoker;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IEventPublisher _eventPublisher;
      private readonly IWithWorkspaceLayout _withWorkspaceLayout;

      public WorkspaceLayoutUpdater(IApplicationController applicationController, 
         IWithIdRepository withIdRepository,
         IOpenSingleStartPresenterInvoker openSingleStartPresenterInvoker, 
         ILazyLoadTask lazyLoadTask, 
         IEventPublisher eventPublisher, 
         IWithWorkspaceLayout withWorkspaceLayout)
      {
         _applicationController = applicationController;
         _withIdRepository = withIdRepository;
         _openSingleStartPresenterInvoker = openSingleStartPresenterInvoker;
         _lazyLoadTask = lazyLoadTask;
         _eventPublisher = eventPublisher;
         _withWorkspaceLayout = withWorkspaceLayout;
      }

      public void SaveCurrentLayout()
      {
         var existingLayoutItems = _withWorkspaceLayout.WorkspaceLayout.LayoutItems;

         var workspaceLayout = createNewWorkspaceLayoutWithOpenPresenters();
         addExistingLayoutItemsToWorkspaceLayout(existingLayoutItems, workspaceLayout);

         _withWorkspaceLayout.WorkspaceLayout = workspaceLayout;
      }

      private void addExistingLayoutItemsToWorkspaceLayout(IEnumerable<WorkspaceLayoutItem> existingLayoutItems, IWorkspaceLayout workspaceLayout)
      {
         existingLayoutItems.Where(x => !workspaceAlreadyContainsLayoutItem(workspaceLayout, x)).Each(item =>
         {
            item.WasOpenOnSave = false;
            workspaceLayout.AddLayoutItem(item);
         });
      }

      private WorkspaceLayout createNewWorkspaceLayoutWithOpenPresenters()
      {
         var workspaceLayout = new WorkspaceLayout();
         foreach (var presenter in _applicationController.OpenedPresenters())
         {
            var withId = presenter.Subject.DowncastTo<IWithId>();
            if (withId == null) continue;
            var workspaceLayoutItem = new WorkspaceLayoutItem {WasOpenOnSave = true, SubjectId = withId.Id, PresentationSettings = presenter.GetSettings()};
            workspaceLayout.AddLayoutItem(workspaceLayoutItem);
         }

         return workspaceLayout;
      }

      private bool workspaceAlreadyContainsLayoutItem(IWorkspaceLayout workspaceLayout, WorkspaceLayoutItem x)
      {
         return workspaceLayout.LayoutItems.FirstOrDefault(layoutItem => string.Equals(layoutItem.PresentationKey, x.PresentationKey) && string.Equals(layoutItem.SubjectId, x.SubjectId)) != null;
      }

      public void RestoreLayout()
      {
         var workspaceLayout = _withWorkspaceLayout.WorkspaceLayout;
         if (workspaceLayout == null) return;

         try
         {
            _eventPublisher.PublishEvent(new HeavyWorkStartedEvent(forceHourGlassCursor: true));
            //To list is important here as restoring the layout may creates new layout items
            workspaceLayout.LayoutItems.Where(x => x.WasOpenOnSave).ToList().Each(restoreLayout);
         }
         finally
         {
            _eventPublisher.PublishEvent(new HeavyWorkFinishedEvent(forceHourGlassCursor: true));
         }
      }

      private void restoreLayout(WorkspaceLayoutItem layoutItem)
      {
         var subject = subjectFor(layoutItem);
         if (subject == null) return;

         var presenter = _openSingleStartPresenterInvoker.OpenPresenterFor(subject);
         try
         {
            presenter.Edit(subject);
            presenter.RestoreSettings(layoutItem.PresentationSettings);
         }
         catch (Exception)
         {
            //exception while loading the subject. We need to close the presenter to avoid memory leaks
            _applicationController.Close(subject);
            throw;
         }
      }

      private IPKSimBuildingBlock subjectFor(WorkspaceLayoutItem layoutItem)
      {
         if (!_withIdRepository.ContainsObjectWithId(layoutItem.SubjectId))
            return null;

         var subject = _withIdRepository.Get<IPKSimBuildingBlock>(layoutItem.SubjectId);
         _lazyLoadTask.Load(subject);
         return subject;
      }
   }
}