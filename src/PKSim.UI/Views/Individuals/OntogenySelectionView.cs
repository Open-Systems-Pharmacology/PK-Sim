using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Extensions;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Individuals
{
   public partial class OntogenySelectionView : BaseResizableUserControl, IOntogenySelectionView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IOntogenySelectionPresenter _presenter;
      private ScreenBinder<IndividualMolecule> _screenBinder;

      public override int OptimalHeight => layoutControlGroup.Height;

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
         AdjustHeight();
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
         layoutItemButtonOntogeny.AdjustButtonSizeWithImageOnly();
         btnShowOntogeny.Image = ApplicationIcons.TimeProfileAnalysis.ToImage(IconSizes.Size16x16);
         btnShowOntogeny.ImageLocation = ImageLocation.MiddleCenter;
         btnShowOntogeny.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ShowOntogeny, ApplicationIcons.TimeProfileAnalysis);

         layoutItemLoadOntogeny.AdjustButtonSizeWithImageOnly();
         btnLoadOntogenyFromFile.Image = ApplicationIcons.Excel.ToImage(IconSizes.Size16x16);
         btnLoadOntogenyFromFile.ImageLocation = ImageLocation.MiddleCenter;
         btnLoadOntogenyFromFile.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ImportOntogenyToolTip, PKSimConstants.UI.ImportOntogeny, ApplicationIcons.Excel);
      }
   }
}