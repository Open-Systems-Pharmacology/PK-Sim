using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.UI.Starter.Presenters;

namespace PKSim.UI.Starter.Views
{
   public partial class StarterRelativeExpressionView : BaseModalView, IStarterRelativeExpressionView
   {
      private IStarterRelativeExpressionPresenter _presenter;

      public StarterRelativeExpressionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IStarterRelativeExpressionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddExpressionPresenter(IView view)
      {
         panelControl.FillWith(view);
      }
   }
}