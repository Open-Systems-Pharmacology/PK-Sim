using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Views.Simulations;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class CreatePopulationAnalysisView : WizardView, ICreatePopulationAnalysisView
   {
      private readonly IToolTipCreator _toolTipCreator;

      public CreatePopulationAnalysisView(Shell shell, IToolTipCreator toolTipCreator) : base(shell)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
      }

      public void AttachPresenter(ICreatePopulationAnalysisPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      protected override void SetActiveControl()
      {
         ActiveControl = TabControl;
      }

      public override XtraTabControl TabControl => tabControl;

      public override void InitializeResources()
      {
         base.InitializeResources();
         var dropDownButtonItem = createAnalysisPresenter.CreateTemplateButtonItem(_toolTipCreator, layoutControlBase);
         dropDownButtonItem.Move(emptySpaceItemBase, InsertType.Left);
         MaximizeBox = true;
         this.ResizeForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
      }

      private ICreatePopulationAnalysisPresenter createAnalysisPresenter => WizardPresenter as ICreatePopulationAnalysisPresenter;
   }
}