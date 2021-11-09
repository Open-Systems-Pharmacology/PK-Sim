using OSPSuite.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
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
      private readonly IExpressionProfileTask _expressionProfileTask;
      public ExpressionProfile ExpressionProfile { get; private set; }


      public CreateExpressionProfilePresenter(
         ICreateExpressionProfileView view,
         ISubPresenterItemManager<IExpressionProfileItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IExpressionProfileFactory expressionProfileFactory,
         IExpressionProfileTask expressionProfileTask) : base(view, subPresenterItemManager, ExpressionProfileItems.All, dialogCreator)
      {
         _expressionProfileFactory = expressionProfileFactory;
         _expressionProfileTask = expressionProfileTask;
      }


      public IPKSimCommand Create<TMolecule>() where TMolecule : IndividualMolecule
      {
         ExpressionProfile = _expressionProfileFactory.Create<TMolecule>();
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(ExpressionProfile));
         _view.ApplicationIcon = ApplicationIcons.IconByName(ExpressionProfile.Icon);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         //We need to rename molecule in the expression profile to match the new name
         _expressionProfileTask.UpdateMoleculeName(ExpressionProfile);
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