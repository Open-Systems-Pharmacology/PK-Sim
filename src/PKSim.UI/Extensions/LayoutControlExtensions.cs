using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraLayout;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Extensions
{
   public static class LayoutControlExtensions
   {
      public static void InitializeDisabledColors(this LayoutControl layoutControl)
      {
         InitializeDisabledColors(layoutControl, UserLookAndFeel.Default);
      }

      public static void InitializeDisabledColors(this LayoutControl layoutControl, UserLookAndFeel lookAndFeel)
      {
         var currentSkin = CommonSkins.GetSkin(lookAndFeel);
         var clrText = currentSkin.Colors[CommonColors.WindowText];
         layoutControl.Appearance.ControlDisabled.ForeColor = clrText;
         layoutControl.Appearance.DisabledLayoutItem.ForeColor = clrText;
         layoutControl.Appearance.DisabledLayoutGroupCaption.ForeColor = clrText;
      }

      public static void HideBorderIfRequired(this LayoutControlGroup layoutControlGroup)
      {
         /*hide the border if the selected skin is one that does not support border layout*/
         if (UserLookAndFeel.Default.SkinName == "Metropolis")
            layoutControlGroup.GroupBordersVisible = false;
      }

      public static LayoutControlItem AddControlItemFor(this LayoutControl layoutControl, Control control)
      {
         return new LayoutControlItem
         {
            Control = control,
            Parent = layoutControl.Root,
            TextVisible = false
         };
      }

      public static LayoutControlItem AddButtonItemFor(this LayoutControl layoutControl, Control control)
      {
         var item = layoutControl.AddControlItemFor(control);
         item.AdjustLongButtonSize(layoutControl);
         return item;
      }

   
      public static void DoInBatch(this LayoutControl layoutControl, Action action)
      {
         try
         {
            layoutControl.BeginUpdate();
            action();
         }
         finally
         {
            layoutControl.EndUpdate();
         }
      }
   }
}