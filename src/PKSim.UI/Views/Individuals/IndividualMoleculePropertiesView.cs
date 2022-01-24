using System;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualMoleculePropertiesView : BaseContainerUserControl, IIndividualMoleculePropertiesView
   {
      private IResizableView _moleculeParametersView;
      private IResizableView _ontogenyView;
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
         _ontogenyView = view;
         AddViewTo(layoutItemOntogeny, view);
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
         layoutItemOntogeny.TextVisible = false;
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

      public int OptimalHeight => (MoleculeParametersVisible ? _moleculeParametersView.OptimalHeight : 0) +
                                  (OntogenyVisible ? _ontogenyView.OptimalHeight : 0);
   }
}