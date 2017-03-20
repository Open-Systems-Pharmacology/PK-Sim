using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundAdvancedParametersPresenter : ICompoundItemPresenter, IContainerPresenter
   {
   }

   public class CompoundAdvancedParametersPresenter : AbstractSubPresenterContainer<ICompoundAdvancedParametersView, ICompoundAdvancedParametersPresenter, ICompoundAdvancedParameterGroupPresenter>, ICompoundAdvancedParametersPresenter
   {
      public CompoundAdvancedParametersPresenter(ICompoundAdvancedParametersView view, ISubPresenterItemManager<ICompoundAdvancedParameterGroupPresenter> subPresenterItemManager)
         : base(view, subPresenterItemManager, CompoundAdvancedParameterGroupItems.All)
      {
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem,IView view)
      {
         View.AddViewForGroup(subPresenterItem, view);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         //this should be done last to allow smooth resizing
         View.AddEmptyPlaceHolder();
      }

      public void EditCompound(PKSim.Core.Model.Compound compound)
      {
         _subPresenterItemManager.AllSubPresenters.Each(presenter => presenter.EditCompound(compound));
      }
   }
}