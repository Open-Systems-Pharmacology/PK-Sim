using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualTransporterExpressionsView : BaseContainerUserControl, IIndividualTransporterExpressionsView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly ScreenBinder<IndividualTransporterDTO> _screenBinder;
      private IIndividualTransporterExpressionsPresenter _presenter;

      public IndividualTransporterExpressionsView(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
         _screenBinder = new ScreenBinder<IndividualTransporterDTO>();
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.TransportType)
            .To(cbTransporterType)
            .WithImages(x => x.Icon.Index)
            .WithValues(x => _presenter.AllTransportTypes())
            .AndDisplays(x => x.DisplayName)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.UpdateTransportType(e.NewValue.TransportType));

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public void AttachPresenter(IIndividualTransporterExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IndividualTransporterDTO transporterExpressionDTO)
      {
         _screenBinder.BindToSource(transporterExpressionDTO);
      }

      public void AddMoleculePropertiesView(IView view) => AddViewTo(layoutItemMoleculeProperties, view);

      public void AddExpressionParametersView(IView view) => AddViewTo(layoutItemExpressionParameters, view);

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemTransporterDirection.TextVisible = false;
         cbTransporterType.SetImages(_imageListRetriever);
         layoutItemMoleculeProperties.TextVisible = false;
         layoutItemExpressionParameters.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeProperties.ExpandButtonVisible = true;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.DefaultTransporterDirection;
         layoutGroupMoleculeLocalization.ExpandButtonVisible = true;
      }
   }
}