using DevExpress.XtraLayout.Utils;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.DiseaseStates;
using PKSim.Presentation.Views.DiseaseStates;
using PKSim.UI.Extensions;
using Padding = System.Windows.Forms.Padding;

namespace PKSim.UI.Views.DiseaseStates
{
   public partial class DiseaseStateSelectionView : BaseUserControl, IDiseaseStateSelectionView
   {
      private readonly ScreenBinder<DiseaseStateDTO> _screenBinder = new ScreenBinder<DiseaseStateDTO>();
      private IDiseaseStateSelectionPresenter _presenter;

      public DiseaseStateSelectionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IDiseaseStateSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(DiseaseStateDTO diseaseStateDTO)
      {
         _screenBinder.BindToSource(diseaseStateDTO);

         var diseaseStateParameter = diseaseStateDTO.Parameter;
         var hasParameter = !diseaseStateParameter.IsNull();
         if (hasParameter)
            layoutItemDiseaseParameter.Text = diseaseStateParameter.DisplayName.FormatForLabel(checkCase: false);

         layoutItemDiseaseParameter.TextVisible = hasParameter;
         layoutItemDiseaseParameter.Visibility = LayoutVisibilityConvertor.FromBoolean(hasParameter);
      }

      public string SelectionLabel
      {
         set => layoutItemDiseaseState.Text = value.FormatForLabel();
      }

      public bool ShowDescription
      {
         set => layoutItemDescription.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();

         _screenBinder.Bind(dto => dto.Value)
            .To(cbDiseaseState)
            .WithValues(dto => _presenter.AllDiseaseStates)
            .AndDisplays(diseaseState => diseaseState.DisplayName)
            .Changed += () => _presenter.DiseaseStateChanged();

         _screenBinder.Bind(dto => dto.Value)
            .To(lblDescription)
            .WithFormat(x => x.Description);

         _screenBinder.Bind(dto => dto.Parameter)
            .To(uxDiseaseParameter);


         RegisterValidationFor(_screenBinder, _presenter.ViewChanged);
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutControl.Margin = new Padding(0);
         layoutControl.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
         layoutItemDiseaseState.Text = PKSimConstants.UI.Select.FormatForLabel();
         layoutItemDiseaseParameter.TextVisible = false;
         lblDescription.AsDescription();
      }
   }
}