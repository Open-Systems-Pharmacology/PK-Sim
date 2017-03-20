using System;
using System.Linq;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils.Menu;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Presenters;

namespace PKSim.UI.Binders
{
   public abstract class UnitsMenuBinder<T>
   {
      protected IUnitsInColumnPresenter<T> _presenter;

      public void BindTo(IUnitsInColumnPresenter<T> presenter)
      {
         _presenter = presenter;
      }

      protected void CreateMenuUnits(T identifier, DXPopupMenu menu)
      {
         menu.Items.Clear();
         var allAvailableUnits = _presenter.AvailableUnitsFor(identifier).ToList();
         if (allAvailableUnits.Count <= 1) return;

         allAvailableUnits.Each(unit=> menu.Items.Add(createUnitMenuItem(identifier, unit)));
      }

      private DXMenuItem createUnitMenuItem(T columnIndentifier, Unit unit)
      {
         var currentUnit = _presenter.DisplayUnitFor(columnIndentifier);
         var tag = new Tuple<T, Unit>(columnIndentifier, unit);
         EventHandler handler = (o, e) => this.DoWithinExceptionHandler(() => setUnit(o));

         if (Equals(currentUnit, unit))
            return new DXMenuCheckItem(unit.Name, check: true, image: null, checkedChanged: handler) {Tag = tag};

         return new DXMenuItem(unit.Name, handler) {Tag = tag};
      }

      private void setUnit(object sender)
      {
         var item = sender as DXMenuItem;
         if (item == null) return;

         var tag = item.Tag as Tuple<T, Unit>;
         if (tag == null) return;
         _presenter.ChangeUnit(tag.Item1, tag.Item2);
      }
   }
}