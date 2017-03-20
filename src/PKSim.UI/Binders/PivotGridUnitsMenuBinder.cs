using OSPSuite.Utility.Extensions;
using DevExpress.XtraPivotGrid;
using OSPSuite.UI.Extensions;
using PopupMenuShowingEventArgs = DevExpress.XtraPivotGrid.PopupMenuShowingEventArgs;

namespace PKSim.UI.Binders
{
   public class PivotGridUnitsMenuBinder : UnitsMenuBinder<string>
   {
      private readonly PivotGridField _pivotField;

      public PivotGridUnitsMenuBinder(PivotGridControl pivotGrid, PivotGridField pivotField)
      {
         _pivotField = pivotField;
         pivotGrid.PopupMenuShowing += (o, e) => this.DoWithinExceptionHandler(() => onPopUpMenuShowing(e));
      }

      private void onPopUpMenuShowing(PopupMenuShowingEventArgs e)
      {
         if (e.MenuType != PivotGridMenuType.FieldValue)
         {
            return;
         }

         var hi = e.HitInfo;
         if (hi.HitTest != PivotGridHitTest.Value || hi.ValueInfo.Field != _pivotField)
         {
            e.Allow = false;
            return;
         }

         var ds = hi.ValueInfo.CreateDrillDownDataSource();
         CreateMenuUnits(ds.StringValue(_pivotField), e.Menu);
      }
   }
}