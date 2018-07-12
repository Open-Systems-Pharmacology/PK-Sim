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

namespace PKSim.UI.BootStrapping
{
   public class PKSimApplication : WindowsFormsApplicationBase
   {
      private readonly ApplicationStartup _applicationStartup;
      private readonly IEventPublisher _eventPublisher;
      private readonly IApplicationController _applicationController;
      private readonly StartOptions _startOptions;
      private ISplashViewPresenter _splashPresenter;

      public PKSimApplication(IApplicationController applicationController, ApplicationStartup applicationStartup, IEventPublisher eventPublisher, StartOptions startOptions)
      {
         _applicationStartup = applicationStartup;
         _eventPublisher = eventPublisher;
         _applicationController = applicationController;
         _startOptions = startOptions;
      }

      protected override bool OnInitialize(ReadOnlyCollection<string> commandLineArgs)
      {
         _applicationStartup.InitializeUserInterace();
         _startOptions.InitializeFrom(commandLineArgs.ToArray());
         return base.OnInitialize(commandLineArgs);
      }

      protected override void OnCreateSplashScreen()
      {
         _splashPresenter = _applicationController.Start<ISplashViewPresenter>();
//         SplashScreen = _splashPresenter.View.DowncastTo<Form>();
      }

      protected override void OnCreateMainForm()
      {
         var mainViewPresenter = _applicationController.Start<IMainViewPresenter>().DowncastTo<IPKSimMainViewPresenter>();
         mainViewPresenter.StartOptions = _startOptions;
         MainForm = mainViewPresenter.BaseView.DowncastTo<Form>();
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
   }
}