using System;
using System.Windows.Forms;
using OSPSuite.DataBinding;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Parameters;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.Utility.Extensions;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualMoleculePropertiesView : BaseUserControl, IIndividualMoleculePropertiesView
   {
      private readonly ScreenBinder<MoleculePropertiesDTO> _screenBinder;
      private readonly UxParameterDTOEdit _uxHalfLifeLiver;
      private readonly UxParameterDTOEdit _uxHalfLifeIntestine;
      private readonly UxParameterDTOEdit _uxReferenceConcentration;
      private IIndividualMoleculePropertiesPresenter _presenter;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public IndividualMoleculePropertiesView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<MoleculePropertiesDTO>();

         _uxHalfLifeLiver = new UxParameterDTOEdit();
         _uxHalfLifeIntestine = new UxParameterDTOEdit();
         _uxReferenceConcentration = new UxParameterDTOEdit();
      }

      public void AttachPresenter(IIndividualMoleculePropertiesPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddOntogenyView(IView view)
      {
         OntogenyVisible = true;
         panelOntogeny.FillWith(view);
      }

      public bool OntogenyVisible
      {
         set { layoutItemOntogeny.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public bool MoleculeParametersVisible
      {
         set
         {
            layoutItemReferenceConcentration.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            layoutItemHalfLifeIntestine.Visibility = layoutItemReferenceConcentration.Visibility;
            layoutItemHalfLifeLiver.Visibility = layoutItemReferenceConcentration.Visibility;
         }
      }

      public void BindTo(MoleculePropertiesDTO moleculePropertiesDTO)
      {
         _screenBinder.BindToSource(moleculePropertiesDTO);
         AdjustHeight();
         updateToolTips(moleculePropertiesDTO);
      }

      private void updateToolTips(MoleculePropertiesDTO moleculePropertiesDTO)
      {
         var toolTip = PKSimConstants.UI.ReferenceConcentrationDescription(moleculePropertiesDTO.MoleculeType);
         updateToolTip(layoutItemReferenceConcentration, _uxReferenceConcentration, toolTip);

         toolTip = PKSimConstants.UI.HalfLifeLiverDescription(moleculePropertiesDTO.MoleculeType);
         updateToolTip(layoutItemHalfLifeLiver, _uxHalfLifeLiver, toolTip);

         toolTip = PKSimConstants.UI.HalfLifeIntestineDescription(moleculePropertiesDTO.MoleculeType);
         updateToolTip(layoutItemHalfLifeIntestine, _uxHalfLifeIntestine, toolTip);
      }

      private void updateToolTip(LayoutControlItem layoutControlItem, UxParameterDTOEdit parameterDTOEdit, string toolTip)
      {
         layoutControlItem.OptionsToolTip.ToolTip = toolTip;
         parameterDTOEdit.ToolTip = toolTip;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();

         _screenBinder.Bind(x => x.ReferenceConcentrationParameter).To(_uxReferenceConcentration);
         _uxReferenceConcentration.RegisterEditParameterEvents(_presenter);

         _screenBinder.Bind(x => x.HalfLifeLiverParameter).To(_uxHalfLifeLiver);
         _uxHalfLifeLiver.RegisterEditParameterEvents(_presenter);

         _screenBinder.Bind(x => x.HalfLifeIntestineParameter).To(_uxHalfLifeIntestine);
         _uxHalfLifeIntestine.RegisterEditParameterEvents(_presenter);

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemReferenceConcentration.Text = PKSimConstants.UI.ReferenceConcentration.FormatForLabel();
         layoutItemHalfLifeLiver.Text = PKSimConstants.UI.HalfLifeLiver.FormatForLabel();
         layoutItemHalfLifeIntestine.Text = PKSimConstants.UI.HalfLifeIntestine.FormatForLabel();
         layoutItemOntogeny.Text = PKSimConstants.UI.OntogenyVariabilityLike.FormatForLabel();
         panelReferenceConcentration.FillWith(_uxReferenceConcentration.DowncastTo<Control>());
         panelHalfLifeLiver.FillWith(_uxHalfLifeLiver.DowncastTo<Control>());
         panelHalfLifeIntestine.FillWith(_uxHalfLifeIntestine.DowncastTo<Control>());
         OntogenyVisible = false;
      }

      public override bool HasError => _screenBinder.HasError;

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void Repaint()
      {
         layoutControl.Refresh();
      }

      public int OptimalHeight => layoutControlGroup.Height;
   }
}