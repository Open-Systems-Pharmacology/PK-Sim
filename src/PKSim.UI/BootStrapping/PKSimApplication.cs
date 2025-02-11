using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using Microsoft.VisualBasic.ApplicationServices;
using PKSim.Presentation;
using PKSim.Presentation.Presenters.Main;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Main;
using Keys = System.Windows.Forms.Keys;

namespace PKSim.UI.BootStrapping
{
   public class PKSimApplication : WindowsFormsApplicationBase, IMessageFilter
   {
      private readonly ApplicationStartup _applicationStartup;
      private readonly IEventPublisher _eventPublisher;
      private readonly IApplicationController _applicationController;
      private readonly StartOptions _startOptions;
      private ISplashViewPresenter _splashPresenter;
      private IPKSimMainViewPresenter _mainViewPresenter;

      public PKSimApplication(IApplicationController applicationController, ApplicationStartup applicationStartup, IEventPublisher eventPublisher, StartOptions startOptions)
      {
         _applicationStartup = applicationStartup;
         _eventPublisher = eventPublisher;
         _applicationController = applicationController;
         _startOptions = startOptions;
      }

      protected override bool OnInitialize(ReadOnlyCollection<string> commandLineArgs)
      {
         _applicationStartup.InitializeUserInterface();
         _startOptions.InitializeFrom(commandLineArgs.ToArray());
         return base.OnInitialize(commandLineArgs);
      }

      protected override void OnCreateSplashScreen()
      {
         _splashPresenter = _applicationController.Start<ISplashViewPresenter>();
         SplashScreen = _splashPresenter.View.DowncastTo<Form>();
      }

      protected override void OnCreateMainForm()
      {
         _mainViewPresenter = _applicationController.Start<IMainViewPresenter>().DowncastTo<IPKSimMainViewPresenter>();
         Application.AddMessageFilter(this);
         _mainViewPresenter.StartOptions = _startOptions;
         MainForm = _mainViewPresenter.BaseView.DowncastTo<Form>();
         _eventPublisher.RemoveListener(_splashPresenter);
         _splashPresenter = null;
      }

      protected override void OnRun()
      {
         try
         {
            _applicationStartup.Start();
            Application.DoEvents();
            base.OnRun();
         }
         catch (Exception)
         {
            HideSplashScreen();
            throw;
         }
      }

      public bool PreFilterMessage(ref Message m)
      {
         if (!isWindowsKeyDownMessage(m))
            return false;

         switch (keyCodeFrom(m))
         {
            case (int)(Keys.Control | Keys.W):
               _mainViewPresenter.ActivePresenter?.Close();
               return true;
         }

         return false;
      }

      private static int keyCodeFrom(Message m)
      {
         return (int)m.WParam | (int)Control.ModifierKeys;
      }

      private static bool isWindowsKeyDownMessage(Message m)
      {
         return m.Msg == 256;
      }
   }
}