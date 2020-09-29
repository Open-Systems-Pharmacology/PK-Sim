using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using PKSim.UI.Starter.Views;

namespace PKSim.UI.Starter.Presenters
{

   public interface IStarterMainPresenter : IPresenter<IStarterMainView>
   {
      void ShowRelativeExpression();
   }

   public class StarterMainPresenter : AbstractPresenter<IStarterMainView, IStarterMainPresenter>, IStarterMainPresenter
   {
      private readonly IApplicationController _applicationController;

      public StarterMainPresenter(IStarterMainView view, IApplicationController applicationController) : base(view)
      {
         _applicationController = applicationController;
      }

     

      // private T start<T>() where T : IBatchPresenter
      // {
      //    var presenter = _applicationController.Start<T>();
      //    View.Hide();
      //    presenter.InitializeForStandAloneStart();
      //    return presenter;
      //}
      public void ShowRelativeExpression()
      {
         using (var presenter = _applicationController.Start<IStarterRelativeExpressionPresenter>())
            presenter.Start();

      }
   }
}