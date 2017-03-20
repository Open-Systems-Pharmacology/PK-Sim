using OSPSuite.Utility.Reflection;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class BoxWhiskerNumericFieldDTO : Notifier
   {
      private bool _showOutliers;

      public virtual bool ShowOutliers
      {
         get { return _showOutliers; }
         set
         {
            _showOutliers = value;
            OnPropertyChanged(() => ShowOutliers);
         }
      }
   }
}