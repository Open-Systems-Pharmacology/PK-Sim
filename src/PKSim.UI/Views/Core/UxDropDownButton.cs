﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using OSPSuite.Assets;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;

namespace PKSim.UI.Views.Core
{
   public class UxDropDownButton : DropDownButton
   {
      private readonly DXPopupMenu _popupMenu;

      public UxDropDownButton()
      {
         DropDownArrowStyle = DropDownArrowStyle.Show;
         ImageLocation = ImageLocation.MiddleLeft;
         _popupMenu = new DXPopupMenu();

         DropDownControl = _popupMenu;
      }

      // public UxDropDownButton(string caption= null, ApplicationIcon icon = null, SuperToolTip superTip = null)
      // {
      //    Text = caption;
      //
      //    Image = imageFrom(icon);
      //    SuperTip = superTip;
      // }

      public void AddMenu(string caption, Action action, ApplicationIcon icon = null)
      {
         _popupMenu.Items.Add(new DXMenuItem(caption, (o, e) => this.DoWithinExceptionHandler(action), imageFrom(icon)));
      }

      public void AddMenu(string caption, Func<Task> action, ApplicationIcon icon = null)
      {
         async void executeAction()
         {
            try
            {
               await action();
            }
            catch (Exception e)
            {
               IoC.Resolve<IExceptionManager>().LogException(e);
            }
         }

         _popupMenu.Items.Add(new DXMenuItem(caption, (o, e) => executeAction(), imageFrom(icon)));
      }

      private Image imageFrom(ApplicationIcon icon)
      {
         return (icon ?? ApplicationIcons.EmptyIcon).ToImage(IconSizes.Size16x16);
      }
   }
}