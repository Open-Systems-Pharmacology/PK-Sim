﻿using DevExpress.Utils;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
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

      public void ShowWarning(string warning)
      {
         if(ReadOnly) 
            return;

         layoutItemWarning.Visibility = LayoutVisibility.Always;
         panelWarning.NoteText = warning;
      }

      public void HideWarning()
      {
         layoutItemWarning.Visibility = LayoutVisibility.Never;
      }

      public void AddMoleculePropertiesView(IView view) => AddViewTo(layoutItemMoleculeProperties, layoutControl, view);

      public void AddExpressionParametersView(IView view) => AddViewTo(layoutItemExpressionParameters, layoutControl, view);

      public bool ReadOnly
      {
         set
         {
            layoutItemTransporterDirection.Enabled = !value;

            //Don't show warning in readonly mode
            if (value)
               HideWarning();
         }
         get => !layoutItemTransporterDirection.Enabled;
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemTransporterDirection.TextVisible = false;
         cbTransporterType.SetImages(_imageListRetriever);
         cbTransporterType.Properties.AllowHtmlDraw = DefaultBoolean.True;
         layoutItemMoleculeProperties.TextVisible = false;
         layoutItemExpressionParameters.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeProperties.ExpandButtonVisible = true;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.DefaultTransporterDirection;
         layoutGroupMoleculeLocalization.ExpandButtonVisible = true;
         HideWarning();
         panelWarning.Image = ApplicationIcons.Warning;
      }
   }
}