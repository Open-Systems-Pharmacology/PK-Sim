using DevExpress.Utils;
using DevExpress.XtraEditors;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class ExpressionProfileSelectionView : BaseModalView, IExpressionProfileSelectionView
   {
      private readonly ScreenBinder<ExpressionProfileSelectionDTO> _screenBinder = new ScreenBinder<ExpressionProfileSelectionDTO>();

      private IExpressionProfileSelectionPresenter _presenter;
      public bool DescriptionVisible { get; set; }
      public bool NameVisible { get; set; }
      public bool NameEditable { get; set; }

      public ExpressionProfileSelectionView(Shell shell)
         : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IExpressionProfileSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.ExpressionProfile)
            .To(cbExpressionProfile)
            .WithValues(x => _presenter.AllExpressionProfiles());

         RegisterValidationFor(_screenBinder, NotifyViewChanged);

         btnCreate.Click += (o, e) => OnEvent(_presenter.CreateExpressionProfile);
         btnLoad.Click += (o, e) => OnEvent(_presenter.LoadExpressionProfileAsync);
      }

      public override bool HasError => _screenBinder.HasError;

      public void BindTo(ExpressionProfileSelectionDTO expressionProfileSelectionDTO)
      {
         _screenBinder.BindToSource(expressionProfileSelectionDTO);
         _presenter.ViewChanged();
      }

      public void RefreshList() => _screenBinder.RefreshListElements();

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.SelectExpressionProfile;
         btnCreate.InitWithImage(ApplicationIcons.Create, imageLocation: ImageLocation.MiddleCenter);
         btnLoad.InitWithImage(ApplicationIcons.LoadFromTemplate, imageLocation: ImageLocation.MiddleCenter);
         layoutItemCreate.AdjustButtonSizeWithImageOnly(layoutControl);
         layoutItemLoad.AdjustButtonSizeWithImageOnly(layoutControl);
         cbExpressionProfile.Properties.AllowHtmlDraw = DefaultBoolean.True;
         layoutItemExpressionProfileSelection.TextVisible = false;
         cbExpressionProfile.Properties.AutoHeight = false;
         cbExpressionProfile.Height = btnLoad.Height;
      }
   }
}