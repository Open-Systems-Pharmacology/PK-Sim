using System;
using System.Windows.Forms;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Presenters;
using OSPSuite.Presentation.Services;

namespace PKSim.BatchTool
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args)
      { 
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         try
         { 
            BatchStarter.Start();
            var mainPresenter = IoC.Resolve<IBatchMainPresenter>();
            mainPresenter.Initialize(BatchStartOptions.From(args));
            Application.Run(mainPresenter.View.DowncastTo<Form>());
         }
         catch (Exception e)
         {
            MessageBox.Show(ExceptionManager.ExceptionMessageWithStackTraceFrom(e), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.LogError();
         }
      }
   }
}
