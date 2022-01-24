using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationSubjectConfigurationView : BaseResizableUserControl, ISimulationSubjectConfigurationView
   {
      private ISimulationSubjectConfigurationPresenter _presenter;
      private readonly ScreenBinder<SimulationSubjectDTO> _screenBinder;
      private readonly UxBuildingBlockSelection _uxSimulationSubjectSelection;

      private const int OPTIMAL_HEIGHT = 60;
      public SimulationSubjectConfigurationView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<SimulationSubjectDTO>();
         _uxSimulationSubjectSelection = new UxBuildingBlockSelection();
         layoutItemIndividual.FillWith(_uxSimulationSubjectSelection);
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.BuildingBlock)
            .To(_uxSimulationSubjectSelection)
            .OnValueUpdating += (o, e) => _presenter.UpdateSelectedSubject(e.NewValue);

         _screenBinder.Bind(x => x.AllowAging)
            .To(chkAllowAging);

         //We are using changed instead of changing to that the presenter reacts to check box change as well
         RegisterValidationFor(_screenBinder, statusChangedNotify: NotifyViewChanged);
      }

      public void AttachPresenter(ISimulationSubjectConfigurationPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(SimulationSubjectDTO simulationSubjectDTO)
      {
         _screenBinder.BindToSource(simulationSubjectDTO);
      }

      public bool AllowAgingVisible
      {
         set
         {
            layoutItemAllowAging.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            AdjustHeight();
         }
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemAllowAging.Visibility);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         chkAllowAging.Text = PKSimConstants.UI.AllowAging;
         layoutItemIndividual.TextVisible = false;
      }

      public override int OptimalHeight => OPTIMAL_HEIGHT - (AllowAgingVisible ? 0 : layoutItemAllowAging.Height - layoutItemAllowAging.Padding.Height);
   }
}