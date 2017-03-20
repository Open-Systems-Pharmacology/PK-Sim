using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface IConfigurableLayoutView : IView<IConfigurableLayoutPresenter>
   {
      void Clear();

      /// <summary>
      ///    Replace the collection style controls view with any view. Normally used when only a single view is being displayed
      ///    eliminating the need for the collection style controls
      /// </summary>
      /// <param name="view">The view being added</param>
      void SetView(IView view);
   }
}