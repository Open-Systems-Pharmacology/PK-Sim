using System.Windows.Forms;
using DevExpress.Utils.Layout;
using DevExpress.XtraLayout.Utils;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using PKSim.Assets;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;
using PKSim.UI.Extensions;
using Padding = System.Windows.Forms.Padding;

namespace PKSim.UI.Views.Protocols
{
   public partial class SimpleProtocolView : BaseUserControl, ISimpleProtocolView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private ISimpleProtocolPresenter _presenter;
      private ScreenBinder<SimpleProtocolDTO> _screenBinder;

      public SimpleProtocolView(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
      }

      public void AttachPresenter(ISimpleProtocolPresenter presenter)
      {
         _presenter = presenter;
      }

      public override bool HasError => _screenBinder.HasError;

      public void BindTo(SimpleProtocolDTO simpleProtocolDTO)
      {
         _screenBinder.BindToSource(simpleProtocolDTO);
      }

      public bool EndTimeVisible
      {
         set => tablePanel.RowFor(uxEndTime).Visible = value;
      }

      public bool DynamicParameterVisible
      {
         set => layoutItemDynamicParameters.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemDynamicParameters.Visibility);
      }

      public bool TargetDefinitionVisible
      {
         set
         {
            tablePanel.RowFor(cbTargetOrgan).Visible = value;
            tablePanel.RowFor(cbTargetCompartment).Visible = value;
         }
         get => tablePanel.RowFor(cbTargetOrgan).Visible;
      }

      public void AddDynamicParameterView(IView view)
      {
         panelDynamicParameters.FillWith(view);
      }

      public void RefreshCompartmentList()
      {
         _screenBinder.RefreshListElements();
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<SimpleProtocolDTO> {BindingMode = BindingMode.OneWay};

         _screenBinder.Bind(x => x.Dose).To(uxDose);
         uxDose.RegisterEditParameterEvents(_presenter);

         _screenBinder.Bind(x => x.EndTime).To(uxEndTime);
         uxEndTime.RegisterEditParameterEvents(_presenter);

         _screenBinder.Bind(x => x.ApplicationType)
            .To(cbApplicationType)
            .WithImages(x => _imageListRetriever.ImageIndex(x.IconName))
            .WithValues(x => _presenter.AllApplications())
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SetApplicationType(e.NewValue));

         _screenBinder.Bind(x => x.DosingInterval)
            .To(cbDosingType)
            .WithValues(x => _presenter.AllDosingIntervals())
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SetDosingInterval(e.NewValue));

         _screenBinder.Bind(x => x.TargetOrgan)
            .To(cbTargetOrgan)
            .WithImages(o => _imageListRetriever.ImageIndex(o))
            .WithValues(dto => _presenter.AllOrgans())
            .AndDisplays(o => _presenter.DisplayFor(o))
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SetTargetOrgan(e.NewValue));

         _screenBinder.Bind(x => x.TargetCompartment)
            .To(cbTargetCompartment)
            .WithImages(c => _imageListRetriever.ImageIndex(c))
            .WithValues(dto => _presenter.AllCompartmentsFor(dto.TargetOrgan))
            .AndDisplays(c => _presenter.DisplayFor(c))
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SetTargetCompartment(e.NewValue));

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         labelDose.Text = PKSimConstants.UI.Dose.FormatForLabel();
         labelEndTime.Text = PKSimConstants.UI.ProtocolEndTime.FormatForLabel();
         labelDosingInterval.Text = PKSimConstants.UI.DosingInterval.FormatForLabel();
         labelApplicationType.Text = PKSimConstants.UI.ApplicationType.FormatForLabel();
         layoutGroupProperties.Text = PKSimConstants.UI.ProtocolProperties;
         labelTargetCompartment.Text = PKSimConstants.UI.TargetCompartment.FormatForLabel();
         labelTargetOrgan.Text = PKSimConstants.UI.TargetOrgan.FormatForLabel();
         cbApplicationType.SetImages(_imageListRetriever);
         cbTargetOrgan.SetImages(_imageListRetriever);
         cbTargetCompartment.SetImages(_imageListRetriever);
         uxDose.Margin = cbApplicationType.Margin;
         uxEndTime.Margin = cbApplicationType.Margin;
         tablePanel.LabelVertAlignment = LabelVertAlignment.Center;
      }
   }
}