using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundParametersPresenter : ICompoundItemPresenter, IContainerPresenter
   {
   }

   public class CompoundParametersPresenter : AbstractSubPresenterContainer<ICompoundParametersView, ICompoundParametersPresenter, ICompoundParameterGroupPresenter>, ICompoundParametersPresenter
   {
      public CompoundParametersPresenter(ICompoundParametersView view, ISubPresenterItemManager<ICompoundParameterGroupPresenter> subPresenterItemManager)
         : base(view, subPresenterItemManager, CompoundParameterGroupItems.All)
      {
      }

      public virtual void EditCompound(PKSim.Core.Model.Compound compound)
      {
         _subPresenterItemManager.AllSubPresenters.Each(presenter => presenter.EditCompound(compound));
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem,IView view)
      {
         View.AddViewForGroup(subPresenterItem, view);
      }
   }
}