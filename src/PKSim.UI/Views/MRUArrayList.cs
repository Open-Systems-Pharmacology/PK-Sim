using System;
using System.Windows.Forms;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;

namespace PKSim.UI.Views
{
   public class MRUArrayList
   {
      private readonly ApplicationMenu _applicationMenu;
      private readonly PanelControl _container;
      private readonly IToolTipCreator _toolTipCreator;

      public MRUArrayList(ApplicationMenu applicationMenu, PanelControl container, IToolTipCreator toolTipCreator)
      {
         _applicationMenu = applicationMenu;
         _container = container;
         _toolTipCreator = toolTipCreator;
      }

      public void ClearEntries()
      {
         foreach (AppMenuFileLabel ml in _container.Controls)
         {
            ml.LabelClick -= menuClicked;
            ml.Dispose();
         }
         _container.Controls.Clear();
      }

      public void InsertElement(IMenuBarItem menuBarItem)
      {
         var ml = new AppMenuFileLabel {ShowCheckButton = false};
         _container.Controls.Add(ml);
         ml.Appearance.TextOptions.HotkeyPrefix = HKeyPrefix.None;
         ml.Text = menuBarItem.Caption;
         ml.Tag = menuBarItem;
         ml.SuperTip = _toolTipCreator.CreateToolTip(menuBarItem.Description, PKSimConstants.UI.FilePath);
         ml.AutoHeight = true;
         ml.Dock = DockStyle.Top;
         ml.LabelClick += menuClicked;
         setElementsRange();
      }

      private void menuClicked(object sender, EventArgs e)
      {
         var ml = sender.DowncastTo<AppMenuFileLabel>();
         var menuBarItem = ml.Tag.DowncastTo<IMenuBarButton>();
         _applicationMenu.HidePopup();
         menuBarItem.Click();
      }

      private void setElementsRange()
      {
         int i = 0;
         foreach (AppMenuFileLabel ml in _container.Controls)
         {
            ml.Caption = string.Format("&{0}", _container.Controls.Count - i);
            i++;
         }
      }
   }
}