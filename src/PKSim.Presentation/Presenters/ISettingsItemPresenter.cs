using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface ISettingsItemPresenter : ISubPresenter
   {
      void EditSettings();
      void SaveSettings(); 
   }
}