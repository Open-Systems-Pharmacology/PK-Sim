using System;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;

namespace PKSim.UI.Binders
{
   public class GridViewColumnUnitsMenuBinder<T> : UnitsMenuBinder<T>
   {
      private readonly Func<GridColumn, T> _columnIdentifierFunc;

      public GridViewColumnUnitsMenuBinder(GridView gridView, Func<GridColumn, T> columnIdentifierFunc)
      {
         _columnIdentifierFunc = columnIdentifierFunc;
         gridView.PopupMenuShowing += (o, e) => this.DoWithinExceptionHandler(() => onPopUpMenuShowing(e));
         gridView.OptionsMenu.EnableColumnMenu = true;
      }

      private void onPopUpMenuShowing(PopupMenuShowingEventArgs e)
      {
         if (_presenter == null) return;
         if (e.MenuType != GridMenuType.Column) return;

         var menu = e.Menu as GridViewColumnMenu;
         if (menu == null) return;
         if (menu.Column == null) return;

         T columnIdentifier = _columnIdentifierFunc(menu.Column);
         CreateMenuUnits(columnIdentifier, menu);
      }
   }
}