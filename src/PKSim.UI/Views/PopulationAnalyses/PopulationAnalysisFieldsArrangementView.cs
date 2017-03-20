using System;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisFieldsArrangementView : BaseUserControl, IPopulationAnalysisFieldsArrangementView
   {
      private IPopulationAnalysisFieldsArrangementPresenter _presenter;

      public PopulationAnalysisFieldsArrangementView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IPopulationAnalysisFieldsArrangementPresenter presenter)
      {
         _presenter = presenter;
      }

      public void SetAreaView(PivotArea area, IView view)
      {
         panelFromArea(area).FillWith(view);
      }

      private PanelControl panelFromArea(PivotArea area)
      {
         switch (area)
         {
            case PivotArea.RowArea:
               return panelRowArea;
            case PivotArea.ColorArea:
               return panelColorArea;
            case PivotArea.FilterArea:
               return panelFilterArea;
            case PivotArea.SymbolArea:
               return panelSymbolArea;
            default:
               throw new ArgumentOutOfRangeException("area");
         }
      }

      public void SetAreaVisibility(PivotArea area, bool visible)
      {
         var items = layoutControl.Items.Cast<BaseLayoutItem>().ToList();
         var layoutItem = items.OfType<LayoutControlItem>().First(x => x.Control == panelFromArea(area));
         layoutItem.Visibility = LayoutVisibilityConvertor.FromBoolean(visible);
         //for columns area (Color and symbol) also hide the empty space item to allow correct resizing
         if(area.Is(PivotArea.ColumnArea))
            emptySpaceItemColorToSymbol.Visibility = layoutItem.Visibility;
      }
   }
}