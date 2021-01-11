using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.UI.BootStrapping;
using PKSim.UI.Starter.Presenters;

namespace PKSim.UI.Starter
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

         //   WindowsFormsSettings.SetDPIAware();
         WindowsFormsSettings.TouchUIMode = TouchUIMode.False;

         try
         {
            ApplicationStartup.Start();
            var mainPresenter = IoC.Resolve<IStarterMainPresenter>();
            Application.Run(mainPresenter.BaseView.DowncastTo<Form>());
         }
         catch (Exception e)
         {
            MessageBox.Show(e.ExceptionMessageWithStackTrace(), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
   }
}