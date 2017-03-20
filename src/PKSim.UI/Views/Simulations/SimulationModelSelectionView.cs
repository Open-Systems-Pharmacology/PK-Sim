using PKSim.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationModelSelectionView : BaseUserControl, ISimulationModelSelectionView
   {
      private ScreenBinder<ModelConfigurationDTO> _modelBinder;
      private ISimulationModelSelectionPresenter _presenter;

      public SimulationModelSelectionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationModelSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutGroupModelSettings.Text = PKSimConstants.UI.ModelSettings;
      }

      public override void InitializeBinding()
      {
         _modelBinder = new ScreenBinder<ModelConfigurationDTO>();
         var modelConfiguration = _modelBinder.Bind(x => x.ModelConfiguration)
            .To(cbModel).WithValues(x => _presenter.AllModels())
            .AndDisplays(model => _presenter.DisplayFor(model));

         modelConfiguration.OnValueSet += (o, e) => _presenter.ModelSelectionChanging(e.NewValue);
         modelConfiguration.Changed += () => _presenter.ModelSelectionChanged();

         RegisterValidationFor(_modelBinder,statusChangedNotify:NotifyViewChanged);
      }


      public void UpdateModelImage(ApplicationImage image)
      {
         pbModel.Image = image;
      }

      public void BindTo(ModelConfigurationDTO modelConfigurationDTO)
      {
         _modelBinder.BindToSource(modelConfigurationDTO);
      }

   }
}