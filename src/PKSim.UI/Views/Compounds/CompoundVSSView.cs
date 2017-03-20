using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Binders;
using OSPSuite.Core.Domain;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundVSSView : BaseResizableUserControl, ICompoundVSSView
   {
      private ICompoundVSSPresenter _presenter;
      private readonly GridViewBinder<VSSValueDTO> _gridViewBinder;
      private readonly GridViewColumnUnitsMenuBinder<string> _columUnitsMenuBinder;
      private IGridViewColumn _vssColumn;
      private readonly DoubleFormatter _doubleFormatter;

      public CompoundVSSView()
      {
         InitializeComponent();
         gridView.ShowRowIndicator = false;
         gridView.AllowsFiltering = false;
         _gridViewBinder = new GridViewBinder<VSSValueDTO>(gridView);
         _columUnitsMenuBinder = new GridViewColumnUnitsMenuBinder<string>(gridView, col => col.Name);
         _doubleFormatter = new DoubleFormatter();
      }

      public void AttachPresenter(ICompoundVSSPresenter presenter)
      {
         _presenter = presenter;
         _columUnitsMenuBinder.BindTo(presenter);
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.Species)
            .WithCaption(PKSimConstants.UI.Species)
            .AsReadOnly();

         _vssColumn=_gridViewBinder.Bind(x => x.VSS)
            .WithCaption(CoreConstants.PKAnalysis.VssPhysChem)
            .WithFormat(_doubleFormatter)
            .AsReadOnly();

         btnCalculateVSS.Click += (o, e) => OnEvent(_presenter.CalculateVSS);
      }

      public void BindTo(IEnumerable<VSSValueDTO> allVSSValues)
      {
         _gridViewBinder.BindToSource(allVSSValues);
         _vssColumn.Caption = _presenter.VSSCaption;
         gridView.BestFitColumns();
         AdjustHeight();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemGrid.TextVisible = false;
         btnCalculateVSS.InitWithImage(ApplicationIcons.Run, PKSimConstants.UI.CalculateVSSValues);
         layoutItemCalculateVSS.AdjustLargeButtonSize();
         Caption = PKSimConstants.UI.PossibleVSSValuesForDefaultSpecies;
      }

      public override void AdjustHeight()
      {
         layoutItemGrid.AdjustControlHeight(gridView.OptimalHeight);
         base.AdjustHeight();
      }

      public override void Repaint()
      {
         gridView.LayoutChanged();
      }

      public override int OptimalHeight
      {
         get { return layoutControlGroup.Height; }
      }
   }
}