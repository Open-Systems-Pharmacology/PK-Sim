using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface IMultiplePanelPresenter : IPresenter
   {
   }

   public abstract class MultiplePanelPresenter : AbstractSubPresenter<IMultiplePanelView, IMultiplePanelPresenter>, IMultiplePanelPresenter
   {
      protected MultiplePanelPresenter(IMultiplePanelView view, params IPresenter[] subPresenters)
         : base(view)
      {
         AddSubPresenters(subPresenters);
         subPresenters.Each(p => _view.ActivateView(p.BaseView));
      }
   }
}