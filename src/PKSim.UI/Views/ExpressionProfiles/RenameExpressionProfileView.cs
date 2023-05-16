using System.Windows.Forms;
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
      private IRenameExpressionProfilePresenter _presenter;

      //only for design time
      public RenameExpressionProfileView() : this(null)
      {
      }

      public RenameExpressionProfileView(IShell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IRenameExpressionProfilePresenter presenter)
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

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.MoleculeName)
            .To(cbMoleculeName);

         _screenBinder.Bind(x => x.Category)
            .To(cbCategory);

         RegisterValidationFor(_screenBinder);
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemCategory.Text = PKSimConstants.UI.ExpressionProfileCategory.FormatForLabel();

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