using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Services;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;
using PKSim.UI.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Extensions;

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

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }

      public void BindTo(SimpleProtocolDTO simpleProtocolDTO)
      {
         _screenBinder.BindToSource(simpleProtocolDTO);
      }

      public bool EndTimeVisible
      {
         set { layoutItemEndTime.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public bool DyamicParameterVisible
      {
         set { layoutItemDynamicParameters.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
         get { return LayoutVisibilityConvertor.ToBoolean(layoutItemDynamicParameters.Visibility); }
      }

      public bool TargetDefinitionVisible
      {
         set
         {
            layoutItemTargetOrgan.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            layoutItemTargetCompartment.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         }
         get { return LayoutVisibilityConvertor.ToBoolean(layoutItemTargetOrgan.Visibility); }
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
            .OnValueSet += (o, e) => OnEvent(() => _presenter.SetApplicationType(e.NewValue));

         _screenBinder.Bind(x => x.DosingInterval)
            .To(cbDosingType)
            .WithValues(x => _presenter.AllDosingIntervals())
            .OnValueSet += (o, e) => OnEvent(() => _presenter.SetDosingInterval(e.NewValue));

         _screenBinder.Bind(x => x.TargetOrgan)
            .To(cbTargetOrgan)
            .WithImages(o => _imageListRetriever.ImageIndex(o))
            .WithValues(dto => _presenter.AllOrgans())
            .AndDisplays(o => _presenter.DisplayFor(o))
            .OnValueSet += (o, e) => OnEvent(() => _presenter.SetTargetOrgan(e.NewValue));

         _screenBinder.Bind(x => x.TargetCompartment)
            .To(cbTargetCompartment)
            .WithImages(c => _imageListRetriever.ImageIndex(c))
            .WithValues(dto => _presenter.AllCompartmentsFor(dto.TargetOrgan))
            .AndDisplays(c => _presenter.DisplayFor(c))
            .OnValueSet += (o, e) => OnEvent(() => _presenter.SetTargetCompartment(e.NewValue));

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemDose.Text = PKSimConstants.UI.Dose.FormatForLabel();
         layoutItemEndTime.Text = PKSimConstants.UI.ProtocolEndTime.FormatForLabel();
         layoutItemDosingInterval.Text = PKSimConstants.UI.DosingInterval.FormatForLabel();
         layoutItemApplicationType.Text = PKSimConstants.UI.ApplicationType.FormatForLabel();
         layoutGroupProperties.Text = PKSimConstants.UI.ProtocolProperties;
         layoutItemTargetCompartment.Text = PKSimConstants.UI.TargetCompartment.FormatForLabel();
         layoutItemTargetOrgan.Text = PKSimConstants.UI.TargetOrgan.FormatForLabel();
         cbApplicationType.SetImages(_imageListRetriever);
         cbTargetOrgan.SetImages(_imageListRetriever);
         cbTargetCompartment.SetImages(_imageListRetriever);
      }
   }
}