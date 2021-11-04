using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Services;
using PKSim.Assets;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.UI.Views.ExpressionProfiles
{
   public partial class ExpressionProfileMoleculesView : BaseUserControl, IExpressionProfileMoleculesView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly ScreenBinder<ExpressionProfileDTO> _screenBinder = new ScreenBinder<ExpressionProfileDTO>();
      private IExpressionProfileMoleculesPresenter _presenter;

      public ExpressionProfileMoleculesView(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
      }

      public void AttachPresenter(IExpressionProfileMoleculesPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(ExpressionProfileDTO expressionProfileDTO)
      {
         _screenBinder.BindToSource(expressionProfileDTO);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();

         _screenBinder.Bind(dto => dto.Species)
            .To(cbSpecies)
            .WithImages(species => _imageListRetriever.ImageIndex(species.Icon))
            .WithValues(dto => _presenter.AllSpecies())
            .AndDisplays(species => species.DisplayName)
            .Changed += () => _presenter.SpeciesChanged();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemSpecies.Text = PKSimConstants.UI.Species.FormatForLabel();
      }
   }
}
