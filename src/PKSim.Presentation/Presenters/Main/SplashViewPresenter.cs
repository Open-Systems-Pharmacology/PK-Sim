using OSPSuite.Utility.Events;
using PKSim.Presentation.Views.Main;
using OSPSuite.Presentation.Presenters;

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
      public SplashViewPresenter(ISplashView splashView)
         : base(splashView)
      {
      }

      public void Handle(ProgressDoneEvent eventToHandle)
      {
         View.ProgressValue = 100;
      }

      public void Handle(ProgressInitEvent eventToHandle)
      {
         View.ProgressValue = 0;
         View.StatusInfo = eventToHandle.Message;
      }

      public void Handle(ProgressingEvent eventToHandle)
      {
         View.StatusInfo = eventToHandle.Message;
         View.ProgressValue = eventToHandle.ProgressPercent;
      }
   }
}