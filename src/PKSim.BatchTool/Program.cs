using System;
using System.Windows.Forms;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.BatchTool.Presenters;

namespace PKSim.BatchTool
{
   [Flags]
   enum ExitCodes
   {
      Success = 0,
      Error = 1 << 0,
   }

   static class Program
   {
      /// <summary>
      ///    The main entry point for the application.
      /// </summary>
      [STAThread]
      static int  Main(string[] args)
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         try
         {
            BatchStarter.Start();
            var mainPresenter = IoC.Resolve<IBatchMainPresenter>();
            var options = BatchStartOptions.From(args);
            mainPresenter.Initialize(options);
            Application.Run(mainPresenter.ActiveView.DowncastTo<Form>());
          
            return (int)ExitCodes.Success;
         }
         catch (Exception e)
         {
            e.LogError();
            return (int)ExitCodes.Error;
         }
      }
   }
}