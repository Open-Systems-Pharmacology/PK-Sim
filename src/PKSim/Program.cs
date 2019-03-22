using System;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Extensions;
using OSPSuite.Engine;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.UI.BootStrapping;
using SimModelNET;

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
            ApplicationStartup.Initialize(LogLevel.Debug, registrationAction);
            IoC.Resolve<PKSimApplication>().Run(args);
         }
         catch (Exception e)
         {
            MessageBox.Show(e.ExceptionMessageWithStackTrace(), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private static void registrationAction(IContainer container)
      {
         container.AddRegister(x => x.FromType<EngineRegister>());
         var pkSimConfiguration = container.Resolve<IPKSimConfiguration>();
         XMLSchemaCache.InitializeFromFile(pkSimConfiguration.SimModelSchemaFilePath);
      }
   }
}