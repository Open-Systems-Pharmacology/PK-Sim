using DevExpress.XtraEditors;

namespace PKSim.UI.Views.Core
{
   public class UxMRUEdit : MRUEdit
   {
      public UxMRUEdit()
      {
         Properties.AllowRemoveMRUItems = false;
      }
   }
}