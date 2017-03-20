using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Assets;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.AdvancedParameters
{
   internal partial class AdvancedParameterDistributionView : BaseUserControl, IAdvancedParameterDistributionView
   {
      private IAdvancedParameterDistributionPresenter _presenter;
      private readonly ScreenBinder<DistributionSettings> _screenBinder;

      public AdvancedParameterDistributionView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<DistributionSettings>();
      }

      public void BindToPlotOptions(DistributionSettings settings)
      {
         _screenBinder.BindToSource(settings);
      }

      public void UpdateParameterGroupView(IView view)
      {
         panelTreeGroup.FillWith(view);
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.BarType)
            .To(cbBarType)
            .WithValues(opt => _presenter.AllBarTypes());

         _screenBinder.Bind(x => x.SelectedGender)
            .To(cbGroupBy)
            .WithValues(opt => _presenter.AllGenders())
            .AndDisplays(group => _presenter.GenderDisplayFor(group));

         _screenBinder.Bind(x => x.AxisCountMode)
            .To(cbScalingMode)
            .WithValues(opt => _presenter.AllScalingModes());

         _screenBinder.Bind(x => x.UseInReport)
            .To(chkUseInReport)
            .Changed += () => OnEvent(() => _presenter.UseSelectedParameterInReport(chkUseInReport.Checked));

         RegisterValidationFor(_screenBinder, statusChangedNotify: NotifyViewChanged);
      }

      public void AttachPresenter(IAdvancedParameterDistributionPresenter presenter)
      {
         _presenter = presenter;
      }

      public bool BarTypeVisible
      {
         set { layoutItemBarType.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public bool GenderSelectionVisible
      {
         set { layoutItemGender.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public bool SettingsEnabled
      {
         get { return layoutGroupSettings.Enabled; }
         set { layoutGroupSettings.Enabled = value; }
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.ParameterDistribution; }
      }

      public void AddDistributionView(IView view)
      {
         splitContainer.Panel2.FillWith(view);
      }

      public override string Caption
      {
         get { return PKSimConstants.UI.Distribution; }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         chkUseInReport.Text = PKSimConstants.UI.UseHistogramInReport;
      }
   }
}