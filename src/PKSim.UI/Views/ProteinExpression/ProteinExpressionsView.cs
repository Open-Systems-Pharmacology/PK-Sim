using OSPSuite.Assets;
using DevExpress.XtraTab;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.ProteinExpression
{
   public partial class ProteinExpressionsView : WizardView, IProteinExpressionsView
   {
      public ProteinExpressionsView(Shell shell):base(shell)
      {
         InitializeComponent();
         ClientSize = new System.Drawing.Size(CoreConstants.UI.EXPRESSION_QUERY_VIEW_WIDTH, CoreConstants.UI.EXPRESSION_QUERY_VIEW_HEIGHT);

      }

      public override XtraTabControl TabControl
      {
         get { return xtraTabController; }
      }

      public void AttachPresenter(IProteinExpressionsPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Text = PKSimConstants.ProteinExpressions.MainView.MainText;
         Icon = ApplicationIcons.ProteinExpression.WithSize(IconSizes.Size16x16);
         this.ReziseForCurrentScreen(fractionHeight: UIConstants.Size.SCREEN_RESIZE_FRACTION, fractionWidth: UIConstants.Size.SCREEN_RESIZE_FRACTION);
         MaximizeBox = true;
      }
   }
}