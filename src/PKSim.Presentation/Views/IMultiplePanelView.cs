using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface IMultiplePanelView : IView<IMultiplePanelPresenter>
   {
      /// <summary>
      ///    Add the view as a new panel
      /// </summary>
      /// <param name="view">The view being inserted</param>
      void ActivateView(IView view);

      /// <summary>
      ///    Hides the panel conmtaing the <paramref name="view"/>
      /// </summary>
      void HideView(IView view);

      void AddEmptyPlaceHolder();
   }
}