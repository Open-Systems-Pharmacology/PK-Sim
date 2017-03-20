using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using PKSim.UI.Extensions;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Parameters
{
   public partial class ScaleParametersView : BaseUserControl, IScaleParametersView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IScaleParametersPresenter _presenter;
      private ScreenBinder<ParameterScaleWithFactorDTO> _screenBinder;

      public ScaleParametersView(IToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<ParameterScaleWithFactorDTO>();
         _screenBinder.Bind(x => x.Factor).To(tbValue);
         btnScale.Click += (o, e) => OnEvent(_presenter.Scale);
         btnReset.Click += (o, e) => OnEvent(_presenter.Reset);
         RegisterValidationFor(_screenBinder, setScaleEnabled);
      }

      private void setScaleEnabled()
      {
         btnScale.Enabled = !HasError;
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }

      public void AttachPresenter(IScaleParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(ParameterScaleWithFactorDTO parameterScaleWithFactorDTO)
      {
         _screenBinder.BindToSource(parameterScaleWithFactorDTO);
      }

      public bool ReadOnly
      {
         set { layoutControl.Enabled = !value; }
      }

      public override void InitializeResources()
      {
         var resetImage = ApplicationIcons.Reset.ToImage(IconSizes.Size16x16);
         btnReset.Text = PKSimConstants.UI.ResetAll;
         btnReset.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ResetAllVisibleButtonToolTip, PKSimConstants.UI.ResetAll, resetImage);
         btnReset.Image = resetImage;
         btnReset.ImageLocation = ImageLocation.MiddleLeft;
         btnScale.Text = PKSimConstants.UI.ScaleButton;
         btnScale.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ScaleButtonToolTip, PKSimConstants.UI.ScaleButton);
         layoutControl.InitializeDisabledColors();
      }
   }
}