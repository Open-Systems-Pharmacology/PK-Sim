using OSPSuite.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface ICreateExpressionProfilePresenter : ICreateBuildingBlockPresenter<ExpressionProfile>, IContainerPresenter
   {
      ExpressionProfile ExpressionProfile { get; }
      IPKSimCommand Create<TMolecule>() where TMolecule : IndividualMolecule;
   }

   public class CreateExpressionProfilePresenter : AbstractSubPresenterContainerPresenter<ICreateExpressionProfileView, ICreateExpressionProfilePresenter, IExpressionProfileItemPresenter>, ICreateExpressionProfilePresenter
   {
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      public ExpressionProfile ExpressionProfile { get; private set; }

      public CreateExpressionProfilePresenter(
         ICreateExpressionProfileView view,
         ISubPresenterItemManager<IExpressionProfileItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IExpressionProfileFactory expressionProfileFactory) : base(view, subPresenterItemManager, ExpressionProfileItems.All, dialogCreator)
      {
         _expressionProfileFactory = expressionProfileFactory;
      }

      public IPKSimCommand Create<TMolecule>() where TMolecule : IndividualMolecule
      {
         ExpressionProfile = _expressionProfileFactory.Create<TMolecule>();
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(ExpressionProfile));
         _view.ApplicationIcon = ApplicationIcons.IconByName(ExpressionProfile.Icon);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         //Make sure we save all information that was edited by the user
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Save());
         return _macroCommand;
      }

      public IPKSimCommand Create() => Create<IndividualEnzyme>();

      public ExpressionProfile BuildingBlock => ExpressionProfile;

      public override void ViewChanged()
      {
         base.ViewChanged();
         View.OkEnabled = CanClose;
      }
   }
}