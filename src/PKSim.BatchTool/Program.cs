using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool
{
   static class Program
   {
      /// <summary>
      ///    The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         WindowsFormsSettings.SetDPIAware();
         WindowsFormsSettings.SetPerMonitorDpiAware();
         WindowsFormsSettings.TouchUIMode = TouchUIMode.False;

         try
         {
            ApplicationStartup.Start();
            var mainPresenter = IoC.Resolve<IBatchMainPresenter>();
            Application.Run(mainPresenter.BaseView.DowncastTo<Form>());
         }
         catch (Exception e)
         {
            MessageBox.Show(e.ExceptionMessageWithStackTrace(), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
   }
}