using System.Drawing;
using System.Windows.Forms;
using OSPSuite.UI.Extensions;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Core;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Charts
{
   public partial class IndividualSimulationComparisonMdiView : BaseMdiChildView, IIndividualSimulationComparisonMdiView
   {
      private readonly LabelControl _lblInfo;
      private IIndividualSimulationComparisonMdiPresenter _individualSimulationComparisonMdiPresenter;

      public IndividualSimulationComparisonMdiView(Shell shell)
         : base(shell)
      {
         InitializeComponent();
         _lblInfo = new LabelControl {Parent = this};
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         DragDrop += DragDropEventHandler;
         DragOver += (o,e) => _individualSimulationComparisonMdiPresenter.OnDragOver(o, new DragEvent(e));
      }

      private void DragDropEventHandler(object sender, DragEventArgs e)
      {
         _individualSimulationComparisonMdiPresenter.OnDragDrop(sender, new DragEvent(e));
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         AllowDrop = true;
         _lblInfo.AllowDrop = true;
         _lblInfo.Font = new Font(_lblInfo.Font.Name, 15.0f);
         _lblInfo.Text = PKSimConstants.Information.SimulationComparisonInfo;
         _lblInfo.AutoSizeMode = LabelAutoSizeMode.Vertical;
         _lblInfo.Width = 400;
         _lblInfo.AsDescription();
         _lblInfo.BackColor = Color.Transparent;
         _lblInfo.Top = 200;
         _lblInfo.Left = 200;
      }

      public void AttachPresenter(IIndividualSimulationComparisonMdiPresenter presenter)
      {
         _individualSimulationComparisonMdiPresenter = presenter;
         _presenter = presenter;
      }

      public bool ChartVisible
      {
         get { return panelControl.Visible; }
         set
         {
            panelControl.Visible = value;
            _lblInfo.Visible = !panelControl.Visible;
         }
      }

      public void AddChartView(IView view)
      {
         panelControl.FillWith(view);
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.IndividualSimulationComparison;
   }
}