using System.Collections.Generic;
using PKSim.Core.Commands;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class AbstractSubPresenterContainerPresenter<TView, TPresenter, TSubPresenter> : AbstractDisposableSubPresenterContainerPresenter<TView, TPresenter, TSubPresenter> where TView : IModalView<TPresenter>, IContainerView where TPresenter : IContainerPresenter where TSubPresenter : ISubPresenter
   {
      protected PKSimMacroCommand _macroCommand;

      protected AbstractSubPresenterContainerPresenter(TView view, ISubPresenterItemManager<TSubPresenter> subPresenterItemManager, IReadOnlyList<ISubPresenterItem> subPresenterItems, IDialogCreator dialogCreator)
         : base(view, subPresenterItemManager,subPresenterItems, dialogCreator)
      {
      }

      public override void Initialize()
      {
         _macroCommand = new PKSimMacroCommand();
         InitializeWith(_macroCommand);
      }
   }
}