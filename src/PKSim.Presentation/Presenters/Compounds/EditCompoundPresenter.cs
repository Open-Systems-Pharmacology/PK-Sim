using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IEditCompoundPresenter : IEditBuildingBockPresenter<Compound>
   {
   }

   public class EditCompoundPresenter : SingleStartContainerPresenter<IEditCompoundView, IEditCompoundPresenter, Compound, ICompoundItemPresenter>, IEditCompoundPresenter
   {
      private Compound _compound;

      public EditCompoundPresenter(IEditCompoundView view, ISubPresenterItemManager<ICompoundItemPresenter> subPresenterSubjectManager)
         : base(view, subPresenterSubjectManager, CompoundItems.All)
      {
      }

      public override void Edit(Compound compound)
      {
         _compound = compound;
         _subPresenterItemManager.AllSubPresenters.Each(x => x.EditCompound(_compound));
         UpdateCaption();
         _view.Display();
      }

      public override object Subject => _compound;

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditCompound(_compound.Name);
      }
   }
}