using System;
using System.Drawing;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;

namespace PKSim.UI.Views.Core
{
   public class UxDropDownButton : DropDownButton
   {
      private readonly DXPopupMenu _popupMenu;

      public UxDropDownButton(string caption, ApplicationIcon icon = null, SuperToolTip superTip = null)
      {
         Text = caption;

         Image = imageFrom(icon);
         SuperTip = superTip;
         DropDownArrowStyle = DropDownArrowStyle.Show;
         ImageLocation = ImageLocation.MiddleLeft;
         _popupMenu = new DXPopupMenu();

         DropDownControl = _popupMenu;
      }

      public void AddMenu(string caption, Action action, ApplicationIcon icon = null)
      {
         _popupMenu.Items.Add(new DXMenuItem(caption, (o, e) => this.DoWithinExceptionHandler(action), imageFrom(icon)));
      }

      private Image imageFrom(ApplicationIcon icon)
      {
         return (icon ?? ApplicationIcons.EmptyIcon).ToImage(IconSizes.Size16x16);
      }
   }
}