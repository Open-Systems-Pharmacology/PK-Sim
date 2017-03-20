using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundInSimulationView : IView<ICompoundInSimulationPresenter>
   {
      void AddView(IView view);

      /// <summary>
      /// Hides a view that was added using caching. If the view cannot be found nothing will happen
      /// </summary>
      /// <seealso cref="AddCachedView"/>
      /// <param name="view">The view being hidden.</param>
      void HideCachedView(IView view);

      /// <summary>
      /// Adds a view to the this view that can be hidden later using <seealso cref="HideCachedView"/>
      /// </summary>
      /// <param name="baseView">The view being added</param>
      void AddCachedView(IView baseView);
   }
}