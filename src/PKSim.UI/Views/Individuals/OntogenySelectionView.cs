using DevExpress.XtraEditors;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views.Individuals
{
   public partial class OntogenySelectionView : BaseResizableUserControl, IOntogenySelectionView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IOntogenySelectionPresenter _presenter;
      private ScreenBinder<IndividualMolecule> _screenBinder;

      public override int OptimalHeight => OSPSuite.UI.UIConstants.Size.ScaleForScreenDPI(30);

      public OntogenySelectionView(IToolTipCreator toolTipCreator)
      {
         InitializeComponent();
         _toolTipCreator = toolTipCreator;
      }

      public void AttachPresenter(IOntogenySelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IndividualMolecule individualMolecule)
      {
         _screenBinder.BindToSource(individualMolecule);
      }

      public bool ShowOntogenyEnabled
      {
         set => btnShowOntogeny.Enabled = value;
      }

      public bool ReadOnly
      {
         set
         {
            cbOntogey.ReadOnly = value;
            btnLoadOntogenyFromFile.Enabled = !value;
         }
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<IndividualMolecule> {BindingMode = BindingMode.OneWay};
         _screenBinder.Bind(x => x.Ontogeny).To(cbOntogey)
            .WithValues(x => _presenter.AllOntogenies())
            .AndDisplays(x => x.DisplayName)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SelectedOntogenyIs(e.NewValue));

         btnShowOntogeny.Click += (o, e) => OnEvent(_presenter.ShowOntogeny);
         btnLoadOntogenyFromFile.Click += (o, e) => OnEvent(_presenter.LoadOntogeny);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnShowOntogeny.InitWithImage(ApplicationIcons.TimeProfileAnalysis, imageLocation: ImageLocation.MiddleCenter);
         btnShowOntogeny.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ShowOntogeny, ApplicationIcons.TimeProfileAnalysis);
         btnShowOntogeny.AdjustButtonWithImageOnly();
         btnLoadOntogenyFromFile.InitWithImage(ApplicationIcons.Excel, imageLocation: ImageLocation.MiddleCenter);
         btnLoadOntogenyFromFile.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ImportOntogenyToolTip, PKSimConstants.UI.ImportOntogeny, ApplicationIcons.Excel);
         btnLoadOntogenyFromFile.AdjustButtonWithImageOnly();
         cbOntogey.Height = btnLoadOntogenyFromFile.Height;
         layoutItemOntogeny.Text = PKSimConstants.UI.OntogenyVariabilityLike.FormatForLabel();
      }
   }
}