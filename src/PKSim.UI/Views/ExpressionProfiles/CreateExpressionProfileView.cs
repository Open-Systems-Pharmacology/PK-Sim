using System.Windows.Forms;
using DevExpress.XtraLayout.Utils;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.UI.Views.ExpressionProfiles
{
   public partial class CreateExpressionProfileView : BaseModalView, ICreateExpressionProfileView
   {
      private const int OPTIMAL_HEIGHT_NO_DISEASE_STATE = 220;
      private const int OPTIMAL_HEIGHT_WITH_DISEASE_STATE = 270;

      private readonly IImageListRetriever _imageListRetriever;
      private readonly ScreenBinder<ExpressionProfileDTO> _screenBinder = new ScreenBinder<ExpressionProfileDTO>();
      private IExpressionProfilePresenter _presenter;

      //only for design time
      public CreateExpressionProfileView() : this(null, null)
      {
      }

      public CreateExpressionProfileView(IShell shell, IImageListRetriever imageListRetriever) : base(shell)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
      }

      public void AttachPresenter(ICloneExpressionProfilePresenter presenter)
      {
         _presenter = presenter;
      }

      public void AttachPresenter(ICreateExpressionProfilePresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(ExpressionProfileDTO expressionProfileDTO)
      {
         ApplicationIcon = expressionProfileDTO.Icon;
         cbMoleculeName.FillWith(expressionProfileDTO.AllMolecules);
         cbCategory.FillWith(expressionProfileDTO.AllCategories);
         layoutItemMoleculeName.Text = expressionProfileDTO.MoleculeType.FormatForLabel();
         _screenBinder.BindToSource(expressionProfileDTO);
      }

      public void AddDiseaseStateView(IView view)
      {
         panelDiseaseState.FillWith(view);
      }

      public void UpdateDiseaseStateVisibility(bool visible)
      {
         layoutItemDiseaseState.Visibility = LayoutVisibilityConvertor.FromBoolean(visible);
         Height = visible ? OPTIMAL_HEIGHT_WITH_DISEASE_STATE : OPTIMAL_HEIGHT_NO_DISEASE_STATE;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(dto => dto.Species)
            .To(cbSpecies)
            .WithImages(species => _imageListRetriever.ImageIndex(species.Icon))
            .WithValues(dto => dto.AllSpecies)
            .AndDisplays(species => species.DisplayName)
            .OnValueUpdated += (o, e) => OnEvent(onSpeciesChanged);

         _screenBinder.Bind(x => x.MoleculeName)
            .To(cbMoleculeName);

         _screenBinder.Bind(x => x.Category)
            .To(cbCategory);

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      private void onSpeciesChanged()
      {
         var createPresenter = _presenter as ICreateExpressionProfilePresenter;
         createPresenter?.SpeciesChanged();
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemCategory.Text = PKSimConstants.UI.ExpressionProfileCategory.FormatForLabel();
         layoutItemSpecies.Text = PKSimConstants.UI.Species.FormatForLabel();
         labelCategoryDescription.AsDescription();
         labelCategoryDescription.Text = PKSimConstants.UI.ExpressionProfileCategoryDescription.FormatForDescription();

         //Do not close on OK
         ButtonOk.DialogResult = DialogResult.None;
      }

      protected override void SetActiveControl()
      {
         ActiveControl = cbMoleculeName;
      }

      protected override void OkClicked() => _presenter.Save();
   }
}