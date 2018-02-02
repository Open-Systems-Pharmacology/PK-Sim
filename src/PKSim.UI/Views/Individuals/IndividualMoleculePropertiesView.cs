using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualMoleculePropertiesView : BaseResizableUserControl, IIndividualMoleculePropertiesView
   {
      private IResizableView _view;

      public IndividualMoleculePropertiesView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IIndividualMoleculePropertiesPresenter presenter)
      {
      }

      public void AddOntogenyView(IView view)
      {
         OntogenyVisible = true;
         AddViewTo(layoutItemOntogeny, view);
      }

      public void AddMoleculeParametersView(IView view)
      {
         _view = view as IResizableView;
         AddViewTo(layoutItemMoleculeParameters, view);
      }

      public void AddViewTo(LayoutControlItem layoutControlItem, IView view)
      {
         layoutControlItem.Control.FillWith(view);
      }

      public bool OntogenyVisible
      {
         set => layoutItemOntogeny.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
      }

      public bool MoleculeParametersVisible
      {
         set => layoutItemMoleculeParameters.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemOntogeny.Text = PKSimConstants.UI.OntogenyVariabilityLike.FormatForLabel();
         layoutItemMoleculeParameters.TextVisible = false;
         OntogenyVisible = false;
      }

      public override void AdjustHeight()
      {
         layoutItemMoleculeParameters.AdjustControlHeight(_view.OptimalHeight);
         base.AdjustHeight();
      }

      public override void Repaint()
      {
         layoutControl.Refresh();
      }

      public override int OptimalHeight => layoutControlGroup.Height;
   }
}