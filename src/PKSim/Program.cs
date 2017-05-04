using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.UI.BootStrapping;

namespace PKSim
{
   internal static class Program
   {
      /// <summary>
      ///    The main entry point for the application.
      /// </summary>
      [STAThread]
      private static void Main(string[] args)
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         WindowsFormsSettings.SetDPIAware();
         WindowsFormsSettings.TouchUIMode = TouchUIMode.False;

         try
         {
            ApplicationStartup.Initialize();
            IoC.Resolve<PKSimApplication>().Run(args);
         }
         catch (Exception e)
         {
            MessageBox.Show(e.ExceptionMessageWithStackTrace(), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.LogError();
         }
      }
   }
}