using System.Collections.Generic;
using PKSim.Core.Commands;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class PKSimWizardPresenter<TView, TPresenter, TSubPresenter> : WizardPresenter<TView, TPresenter, TSubPresenter> where TSubPresenter : ISubPresenter where TView : IWizardView, IModalView<TPresenter> where TPresenter : IContainerPresenter
   {
      protected PKSimMacroCommand _macroCommand;

      protected PKSimWizardPresenter(TView view, ISubPresenterItemManager<TSubPresenter> subPresenterItemManager, IReadOnlyList<ISubPresenterItem> subPresenterItems, IDialogCreator dialogCreator)
         : base(view, subPresenterItemManager, subPresenterItems, dialogCreator)
      {
      }

      public override void Initialize()
      {
         _macroCommand = new PKSimMacroCommand();
         InitializeWith(_macroCommand);
      }
   }
}