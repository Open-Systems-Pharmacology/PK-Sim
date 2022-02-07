using PKSim.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI.Controls;
using PKSim.Assets.Images;

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

      public override void InitializeBinding()
      {
         _modelBinder = new ScreenBinder<ModelConfigurationDTO>();
         var modelConfiguration = _modelBinder.Bind(x => x.ModelConfiguration)
            .To(cbModel).WithValues(x => _presenter.AllModels())
            .AndDisplays(model => _presenter.DisplayFor(model));

         modelConfiguration.OnValueUpdating += (o, e) => _presenter.ModelSelectionChanging(e.NewValue);
         modelConfiguration.Changed += () => _presenter.ModelSelectionChanged();

         RegisterValidationFor(_modelBinder,statusChangedNotify:NotifyViewChanged);
      }


      public void UpdateModelImage(string imageName)
      {
         pbModel.Image = ApplicationImages.ImageByName(imageName);
      }

      public void BindTo(ModelConfigurationDTO modelConfigurationDTO)
      {
         _modelBinder.BindToSource(modelConfigurationDTO);
      }

   }
}