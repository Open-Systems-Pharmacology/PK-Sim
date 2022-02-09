using OSPSuite.Assets;
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
      private readonly IImageListRetriever _imageListRetriever;
      private readonly ScreenBinder<ExpressionProfileDTO> _screenBinder = new ScreenBinder<ExpressionProfileDTO>();

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
      }

      public void AttachPresenter(ICreateExpressionProfilePresenter presenter)
      {
      }

      public void BindTo(ExpressionProfileDTO expressionProfileDTO)
      {
         Icon = expressionProfileDTO.Icon.WithSize(IconSizes.Size16x16);
         cbMoleculeName.FillWith(expressionProfileDTO.AllMolecules);
         layoutItemMoleculeName.Text = expressionProfileDTO.MoleculeType.FormatForLabel();
         _screenBinder.BindToSource(expressionProfileDTO);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(dto => dto.Species)
            .To(cbSpecies)
            .WithImages(species => _imageListRetriever.ImageIndex(species.Icon))
            .WithValues(dto => dto.AllSpecies)
            .AndDisplays(species => species.DisplayName);

         _screenBinder.Bind(x => x.MoleculeName)
            .To(cbMoleculeName);

         _screenBinder.Bind(x => x.Category)
            .To(tbCategory);

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemCategory.Text = PKSimConstants.UI.ExpressionProfileCategory.FormatForLabel();
         layoutItemSpecies.Text = PKSimConstants.UI.Species.FormatForLabel();
      }

      protected override void SetActiveControl()
      {
         ActiveControl = cbMoleculeName;
      }
   }
}