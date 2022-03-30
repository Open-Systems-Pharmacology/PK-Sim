using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IEditIndividualPresenter : IIndividualPresenter, IEditBuildingBockPresenter<Individual>
   {
   }

   public class EditIndividualPresenter : SingleStartContainerPresenter<IEditIndividualView, IEditIndividualPresenter, Individual, IIndividualItemPresenter>, IEditIndividualPresenter
   {
      public EditIndividualPresenter(
         IEditIndividualView view, 
         ISubPresenterItemManager<IIndividualItemPresenter> subPresenterItemManager)
         : base(view, subPresenterItemManager, IndividualItems.All)
      {
      }

      public override void Edit(Individual individualToEdit)
      {
         _subPresenterItemManager.AllSubPresenters.Each(presenter => presenter.EditIndividual(individualToEdit));
         _view.ApplicationIcon = ApplicationIcons.IconByName(individualToEdit.Species.Icon);
         _view.EnableControl(IndividualItems.Settings);
         _view.EnableControl(IndividualItems.Parameters);
         _view.EnableControl(IndividualItems.Expression);
         _view.ActivateControl(IndividualItems.Parameters);
         UpdateCaption();
         _view.Display();
      }

      public override object Subject => Individual;

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditIndividual(Individual.Name);
      }

      public virtual Individual Individual => PresenterAt(IndividualItems.Settings).Individual;
   }
}