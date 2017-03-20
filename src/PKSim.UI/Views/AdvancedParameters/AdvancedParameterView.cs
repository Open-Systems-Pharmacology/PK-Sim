using PKSim.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.AdvancedParameters
{
   public partial class AdvancedParameterView : BaseUserControl, IAdvancedParameterView
   {
      private ScreenBinder<AdvancedParameterDTO> _screenBinder;
      private IAdvancedParameterPresenter _presenter;

      public AdvancedParameterView()
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<AdvancedParameterDTO>();
         _screenBinder.Bind(x => x.DistributionType).To(cbDistributionType)
            .WithValues(dto => _presenter.AllDistributions(dto))
            .Changed += () => OnEvent(_presenter.DistributionTypeChanged);
      }

      public void AttachPresenter(IAdvancedParameterPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(AdvancedParameterDTO advancedParameterDTO)
      {
         _screenBinder.BindToSource(advancedParameterDTO);
         layoutGroupDistributionType.Text = advancedParameterDTO.ParameterFullDisplayName;
      }

      public void AddParameterView(IView view)
      {
         panelParameters.FillWith(view);
      }

      public void DeleteBinding()
      {
         _screenBinder.DeleteBinding();
         layoutGroupDistributionType.Text = PKSimConstants.UI.DistributionType;
         cbDistributionType.Properties.Items.Clear();
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }

      public override void InitializeResources()
      {
         layoutGroupDistributionType.Text = PKSimConstants.UI.DistributionType;
      }
   }
}