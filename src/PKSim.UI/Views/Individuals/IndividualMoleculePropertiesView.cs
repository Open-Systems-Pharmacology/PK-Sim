using System;
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
   public partial class IndividualMoleculePropertiesView : BaseContainerUserControl, IIndividualMoleculePropertiesView
   {
      private IResizableView _ontogenyView;
      private IResizableView _moleculeParametersView;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public IndividualMoleculePropertiesView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IIndividualMoleculePropertiesPresenter presenter)
      {
      }

      public void AddOntogenyView(IResizableView view)
      {
         OntogenyVisible = true;
         AddViewTo(layoutItemOntogeny, view);
         _ontogenyView = view;
      }
      public void AddMoleculeParametersView(IResizableView view)
      {
         _moleculeParametersView = view;
         AddViewTo(layoutItemMoleculeParameters, view);
      }

      public bool OntogenyVisible
      {
         set => layoutItemOntogeny.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemOntogeny.Visibility);
      }

      public bool MoleculeParametersVisible
      {
         set => layoutItemMoleculeParameters.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemMoleculeParameters.Visibility);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemOntogeny.Text = PKSimConstants.UI.OntogenyVariabilityLike.FormatForLabel();
         layoutItemMoleculeParameters.TextVisible = false;
         OntogenyVisible = false;
      }

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void Repaint()
      {
         layoutControl.Refresh();
      }

      //For some reasons, an extra 10 pixel is required that cannot be inferred from anywhere. Using the Root group does not returns the right height
      public int OptimalHeight => (MoleculeParametersVisible ? _moleculeParametersView.OptimalHeight : 0) +
                                  (OntogenyVisible ? _ontogenyView.OptimalHeight : 0) + 10 ;
   }
}