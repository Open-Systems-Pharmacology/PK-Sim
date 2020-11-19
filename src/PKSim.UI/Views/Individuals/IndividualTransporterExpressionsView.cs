using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
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
         panelWarning.Image = ApplicationIcons.Warning;

      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.TransportType)
            .To(cbTransporterType)
            .WithImages(transporterIconIndex)
            .WithValues(x => _presenter.AllTransportTypes())
            .AndDisplays(x => _presenter.TransportTypeCaptionFor(x))
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.UpdateTransportType(e.NewValue));
         


         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      private int transporterIconIndex(TransportType transportType) => TransportTypes.By(transportType).Icon.Index;

      public void AttachPresenter(IIndividualTransporterExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IndividualTransporterDTO transporterExpressionDTO)
      {
         _screenBinder.BindToSource(transporterExpressionDTO);
      }

      public void ShowWarning(string warning)
      {
         layoutItemWarning.Visibility = LayoutVisibility.Always;
         panelWarning.NoteText = warning;
      }

      public void HideWarning() => layoutItemWarning.Visibility = LayoutVisibility.Never;

      public void AddMoleculePropertiesView(IView view) => AddViewTo(layoutItemMoleculeProperties, view);

      public void AddExpressionParametersView(IView view) => AddViewTo(layoutItemExpressionParameters, view);

      public override bool HasError => _screenBinder.HasError;

 
      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemTransporterType.Text = PKSimConstants.UI.TransporterType.FormatForLabel();
         cbTransporterType.SetImages(_imageListRetriever);
         lblTransporterTypeDescription.AsDescription();
         lblTransporterTypeDescription.Text = PKSimConstants.UI.TransporterTypeDescription;
         layoutItemMoleculeProperties.TextVisible = false;
         layoutItemExpressionParameters.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.Localization;
      }
   }
}