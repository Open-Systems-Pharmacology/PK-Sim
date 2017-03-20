using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using PKSim.Presentation.DTO.Formulations;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Views.Formulations;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Formulations
{
   public partial class FormulationSettingsView : BaseUserControl, IFormulationSettingsView
   {
      private IFormulationSettingsPresenter _presenter;
      private ScreenBinder<FormulationDTO> _screenBinder;

      public FormulationSettingsView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IFormulationSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<FormulationDTO>();
         _screenBinder.Bind(x => x.Description)
            .To(lblFormulationDescription);

         _screenBinder.Bind(x => x.Type)
            .To(cbFormulationType)
            .WithValues(dto => _presenter.AllFormulationTypes())
            .AndDisplays(type => type.DisplayName)
            .Changed += () => _presenter.OnFormulationTypeChanged();
      }

      public void BindTo(FormulationDTO formulationDTO)
      {
         _screenBinder.BindToSource(formulationDTO);
      }

      public void AddParameterView(IView view)
      {
         splitControl.Panel1.FillWith(view);
      }

      public void AddChartView(IView view)
      {
         splitControl.Panel2.FillWith(view);
      }

      public bool FormulationTypeVisible
      {
         set { layoutItemFormulationType.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
         get { return LayoutVisibilityConvertor.ToBoolean(layoutItemFormulationType.Visibility); }
      }

      public bool ChartVisible
      {
         set { splitControl.PanelVisibility = value ? SplitPanelVisibility.Both : SplitPanelVisibility.Panel1; }
         get { return splitControl.PanelVisibility == SplitPanelVisibility.Both; }
      }
   }
}