using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.UI.Views.ExpressionProfiles
{
   public partial class RenameExpressionProfileView : BaseModalView, IRenameExpressionProfileView
   {
      private readonly ScreenBinder<ExpressionProfileDTO> _screenBinder = new ScreenBinder<ExpressionProfileDTO>();

      //only for design time
      public RenameExpressionProfileView() : this(null)
      {
      }

      public RenameExpressionProfileView(IShell shell):base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IRenameExpressionProfilePresenter presenter)
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
         _screenBinder.Bind(x => x.MoleculeName)
            .To(cbMoleculeName);

         _screenBinder.Bind(x => x.Category)
            .To(tbCategory);

         RegisterValidationFor(_screenBinder);
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemCategory.Text = PKSimConstants.UI.ExpressionProfileCategory.FormatForLabel();
      }

      protected override void SetActiveControl()
      {
         ActiveControl = cbMoleculeName;
      }
   }
}