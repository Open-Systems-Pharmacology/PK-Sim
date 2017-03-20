using System;
using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class AbstractSubPresenterContainer<TView, TPresenter, TSubPresenter> : AbstractSubPresenter<TView, TPresenter>, IContainerPresenter
      where TView : IView<TPresenter>
      where TPresenter : IPresenter
      where TSubPresenter : ISubPresenter

   {
      protected readonly ISubPresenterItemManager<TSubPresenter> _subPresenterItemManager;
      private readonly IReadOnlyList<ISubPresenterItem> _subPresenterItems;

      protected AbstractSubPresenterContainer(TView view, ISubPresenterItemManager<TSubPresenter> subPresenterItemManager, IReadOnlyList<ISubPresenterItem> subPresenterItems)
         : base(view)
      {
         _subPresenterItemManager = subPresenterItemManager;
         _subPresenterItems = subPresenterItems;
      }

      public override bool CanClose => _subPresenterItemManager.CanClose && base.CanClose;

      public abstract void AddSubItemView(ISubPresenterItem subPresenterItem, IView view);

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _subPresenterItemManager.ReleaseFrom(eventPublisher);
      }

      public override void InitializeWith(ICommandCollector commandRegister)
      {
         base.InitializeWith(commandRegister);
         _subPresenterItemManager.InitializeWith(this, _subPresenterItems);
      }

      protected virtual void Cleanup()
      {
         ReleaseFrom(_subPresenterItemManager.EventPublisher);
      }

      public virtual bool ShouldCancel => true;

      #region Disposable properties

      private bool _disposed;

      public void Dispose()
      {
         if (_disposed) return;

         Cleanup();
         GC.SuppressFinalize(this);
         _disposed = true;
      }

      ~AbstractSubPresenterContainer()
      {
         Cleanup();
      }

      #endregion
   }
}