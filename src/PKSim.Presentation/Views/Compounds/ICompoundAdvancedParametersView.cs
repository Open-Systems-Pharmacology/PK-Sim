using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundAdvancedParametersView : IView<ICompoundAdvancedParametersPresenter>
   {
      void AddViewForGroup(ISubPresenterItem subPresenterItem,IView view);
      void AddEmptyPlaceHolder();
   }
}