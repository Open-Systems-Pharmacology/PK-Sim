using System;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualMoleculePropertiesView : BaseContainerUserControl, IIndividualMoleculePropertiesView
   {
      private IResizableView _view;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

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

      public void AdjustHeight()
      {
         _view.AdjustHeight();
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void Repaint()
      {
         layoutControl.Refresh();
      }

      public int OptimalHeight => layoutControlGroup.Height;
   }
}