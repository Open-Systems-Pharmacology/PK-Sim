using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.Core;
using PKSim.UI.Starter.Presenters;

namespace PKSim.UI.Starter.Views
{
   public partial class StarterMainView : BaseView, IStarterMainView
   {
      private IStarterMainPresenter _presenter;

      public StarterMainView()
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         btnRelativeExpression.Click += (o, e) => _presenter.ShowRelativeExpression();
      }

      public void AttachPresenter(IStarterMainPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = CoreConstants.PRODUCT_NAME;
         ShowInTaskbar = true;
         StartPosition = FormStartPosition.CenterScreen;
         ApplicationIcon = ApplicationIcons.PKSim;
         Icon = ApplicationIcon.WithSize(IconSizes.Size32x32);
         btnRelativeExpression.Text = "Show Relative Expressions";
      }
   }
}