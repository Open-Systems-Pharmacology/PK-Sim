using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface IConfigurableLayoutPresenter : IPresenter
   {
      /// <summary>
      ///    Adds a set of views dynamically to this view. If there is only one view, then no collection type controls will be
      ///    used.
      ///    If multiple views are in the list, then they will be all added as separate elements in the view using a collection
      ///    type control
      /// </summary>
      /// <param name="views">The view being added</param>
      void AddViews(IEnumerable<IView> views);

      /// <summary>
      ///    Removes all views dynamically added to the presenter
      /// </summary>
      void RemoveViews();
   }

   public class ConfigurableLayoutPresenter : AbstractPresenter<IConfigurableLayoutView, IConfigurableLayoutPresenter>, IConfigurableLayoutPresenter
   {
      private readonly IConfigurableContainerLayoutViewFactory _configurableContainerLayoutViewFactory;

      public ConfigurableLayoutPresenter(IConfigurableLayoutView view, IConfigurableContainerLayoutViewFactory configurableContainerLayoutViewFactory)
         : base(view)
      {
         _configurableContainerLayoutViewFactory = configurableContainerLayoutViewFactory;
      }

      public void AddViews(IEnumerable<IView> views)
      {
         var allViews = views.ToList();
         if (allViews.Count > 1)
         {
            var configurableView = _configurableContainerLayoutViewFactory.Create();
            allViews.Each(configurableView.AddView);
            configurableView.FinishedAddingViews();
            _view.SetView(configurableView);
         }

         else if (allViews.Count == 1)
            _view.SetView(allViews[0]);
      }

      public void RemoveViews()
      {
         _view.Clear();
      }
   }
}