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
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.ProteinExpression
{
   public partial class ProteinExpressionsView : WizardView, IProteinExpressionsView
   {
      public ProteinExpressionsView(Shell shell):base(shell)
      {
         InitializeComponent();
         ClientSize = new System.Drawing.Size(UIConstants.Size.EXPRESSION_QUERY_VIEW_WIDTH, UIConstants.Size.EXPRESSION_QUERY_VIEW_HEIGHT);

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
         ApplicationIcon = ApplicationIcons.ProteinExpression;
         this.ReziseForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
         MaximizeBox = true;
      }
   }
}