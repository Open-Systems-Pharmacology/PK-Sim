using OSPSuite.Utility.Events;
using PKSim.Presentation.Views.Main;
using OSPSuite.Presentation.Presenters;
using PKSim.Core;

namespace PKSim.Presentation.Presenters.Main
{
   public interface ISplashViewPresenter : IPresenter<ISplashView>,
      IListener<ProgressDoneEvent>,
      IListener<ProgressInitEvent>,
      IListener<ProgressingEvent>
   {
   }

   public class SplashViewPresenter : AbstractPresenter<ISplashView, ISplashViewPresenter>, ISplashViewPresenter
   {
      private readonly IPKSimConfiguration _configuration;

      public SplashViewPresenter(ISplashView splashView, IPKSimConfiguration configuration)
         : base(splashView)
      {
         _configuration = configuration;
      }

      public void Handle(ProgressDoneEvent eventToHandle)
      {
         View.ProgressValue = 100;
      }

      public void Handle(ProgressInitEvent eventToHandle)
      {
         View.ProgressValue = 0;
         View.VersionInfo = _configuration.FullVersionDisplay;
         View.StatusInfo = eventToHandle.Message;
      }

      public void Handle(ProgressingEvent eventToHandle)
      {
         View.StatusInfo = eventToHandle.Message;
         View.ProgressValue = eventToHandle.ProgressPercent;
      }
   }
}