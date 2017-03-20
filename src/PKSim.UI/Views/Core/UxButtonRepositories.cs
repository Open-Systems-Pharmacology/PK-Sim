using System.Linq;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Core.Chart;

namespace PKSim.UI.Views.Core
{
  
   public class UxSymbolsComboBoxRepository : UxRepositoryItemComboBox
   {
      public UxSymbolsComboBoxRepository(BaseView view) : base(view)
      {
         this.FillComboBoxRepositoryWith(EnumHelper.AllValuesFor<Symbols>());
      }
   }

   public class UxLineStylesComboBoxRepository : UxRepositoryItemComboBox
   {
      public UxLineStylesComboBoxRepository(BaseView view, bool removeLineStyleNone = false) : base(view)
      {
         var allLineStyles = EnumHelper.AllValuesFor<LineStyles>().ToList();
         if (removeLineStyleNone)
            allLineStyles.Remove(LineStyles.None);

         this.FillComboBoxRepositoryWith(allLineStyles);
      }
   }
}