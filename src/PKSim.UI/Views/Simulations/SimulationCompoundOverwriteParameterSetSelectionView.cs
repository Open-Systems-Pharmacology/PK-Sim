using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundOverwriteParameterSetSelectionView : BaseResizableUserControl, ISimulationCompoundOverwriteParameterSetSelectionView
   {
      private ISimulationCompoundOverwriteParameterSetSelectionPresenter _presenter;
      private readonly ScreenBinder<SimulationCompoundOverwriteParameterSetSelectionDTO> _screenBinder;

      private readonly int OPTIMAL_HEIGHT = ScaleForScreenDPI(35);

      public SimulationCompoundOverwriteParameterSetSelectionView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<SimulationCompoundOverwriteParameterSetSelectionDTO>();
      }

      public void AttachPresenter(ISimulationCompoundOverwriteParameterSetSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(SimulationCompoundOverwriteParameterSetSelectionDTO dto)
      {
         _screenBinder.BindToSource(dto);
         cbOverwriteParameterSet.Enabled = dto.AllOverwriteParameterSets.Count > 1;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.SelectedOverwriteParameterSet)
            .To(cbOverwriteParameterSet)
            .WithValues(x => x.AllOverwriteParameterSets)
            .AndDisplays(x => x?.Name ?? PKSimConstants.UI.None);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.OverwriteParameterSetSelection;
         layoutItemOverwriteParameterSet.Text = PKSimConstants.UI.OverwriteParameterSetInCompound.FormatForLabel();
      }

      public override int OptimalHeight => OPTIMAL_HEIGHT;

      public override bool HasError => _screenBinder.HasError;
   }
}