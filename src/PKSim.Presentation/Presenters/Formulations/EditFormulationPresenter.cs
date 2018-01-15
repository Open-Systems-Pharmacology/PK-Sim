using PKSim.Assets;
using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Formulations;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface IEditFormulationPresenter : IEditBuildingBockPresenter<Formulation>, IListener<BuildingBlockUpdatedEvent>
   {
   }

   public class EditFormulationPresenter : SingleStartContainerPresenter<IEditFormulationView, IEditFormulationPresenter, Formulation, IFormulationItemPresenter>, IEditFormulationPresenter
   {
      private Formulation _formulation;

      public EditFormulationPresenter(IEditFormulationView view, ISubPresenterItemManager<IFormulationItemPresenter> subPresenterItemManager)
         : base(view, subPresenterItemManager, FormulationItems.All)
      {
      }

      public override void Edit(Formulation formulation)
      {
         _formulation = formulation;
         UpdateCaption();
         formulationSettingsPresenter.CanEditFormulationType = false;
         formulationSettingsPresenter.AutoSave = true;
         formulationSettingsPresenter.EditFormulation(_formulation);
         _view.Display();
      }

      public override object Subject => _formulation;

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditFormulation(_formulation.Name);
      }

      private IFormulationSettingsPresenter formulationSettingsPresenter => PresenterAt(FormulationItems.Settings);

      public void Handle(BuildingBlockUpdatedEvent eventToHandle)
      {
         if (!canHandle(eventToHandle.BuildingBlock))
            return;

         Edit(_formulation);
      }

      private bool canHandle(IPKSimBuildingBlock buildingBlock)
      {
         return Equals(_formulation, buildingBlock);
      }
   }
}