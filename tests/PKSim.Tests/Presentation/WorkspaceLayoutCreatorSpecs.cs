using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_WorkspaceLayoutCreator : ContextSpecification<IWorkspaceLayoutUpdater>
   {
      protected IApplicationController _applicationController;
      private IOpenSingleStartPresenterInvoker _openSingleStartPresenterInvoker;
      private IWithIdRepository _withIdRepository;
      private ILazyLoadTask _lazyLoadTask;
      private IEventPublisher _eventPublisher;
      protected IWithWorkspaceLayout _workspace;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         _openSingleStartPresenterInvoker = A.Fake<IOpenSingleStartPresenterInvoker>();
         _withIdRepository = A.Fake<IWithIdRepository>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _workspace = A.Fake<IWithWorkspaceLayout>();
         sut = new WorkspaceLayoutUpdater(_applicationController, _withIdRepository, _openSingleStartPresenterInvoker, _lazyLoadTask, _eventPublisher, _workspace);
      }
   }

   public class When_updating_workspace_layout : concern_for_WorkspaceLayoutCreator
   {
      private ISingleStartPresenter _presenter1;
      private ISingleStartPresenter _presenter2;
      private Individual _individual;
      private Compound _compound;
      private IWorkspaceLayout _existingWorkspaceLayout;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual().WithId("individual");
         _compound = new Compound().WithId("compound");

         _presenter1 = A.Fake<ISingleStartPresenter>();
         A.CallTo(() => _presenter1.Subject).Returns(_individual);
         _presenter2 = A.Fake<ISingleStartPresenter>();
         A.CallTo(() => _presenter2.Subject).Returns(_compound);
         A.CallTo(() => _applicationController.OpenedPresenters()).Returns(new[] {_presenter1, _presenter2});

         _existingWorkspaceLayout = A.Fake<IWorkspaceLayout>();
         _workspace.WorkspaceLayout = _existingWorkspaceLayout;
         A.CallTo(() => _existingWorkspaceLayout.LayoutItems).Returns(new List<WorkspaceLayoutItem> {new WorkspaceLayoutItem()});
      }

      protected override void Because()
      {
         sut.SaveCurrentLayout();
      }

      [Observation]
      public void should_return_an_object_containing_one_layout_item_for_each_opened_presenter_and_one_for_the_existing_layout_item()
      {
         _workspace.WorkspaceLayout.LayoutItems.Count().ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_mark_the_new_layout_items_as_open()
      {
         _workspace.WorkspaceLayout.LayoutItems.Count(x => !_existingWorkspaceLayout.LayoutItems.Contains(x)).ShouldBeEqualTo(2);
         _workspace.WorkspaceLayout.LayoutItems.Where(x => !_existingWorkspaceLayout.LayoutItems.Contains(x)).Each(x => x.WasOpenOnSave.ShouldBeTrue());
      }

      [Observation]
      public void should_mark_the_existing_layout_item_as_not_open()
      {
         _workspace.WorkspaceLayout.LayoutItems.Count(x => _existingWorkspaceLayout.LayoutItems.Contains(x)).ShouldBeEqualTo(1);
         _workspace.WorkspaceLayout.LayoutItems.First(x => _existingWorkspaceLayout.LayoutItems.Contains(x)).WasOpenOnSave.ShouldBeFalse();
      }
   }
}