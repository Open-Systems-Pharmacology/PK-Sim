using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationBoxWhiskerAnalysis : PopulationPivotAnalysis
   {
      public bool ShowOutliers { get; set; }

      public PopulationBoxWhiskerAnalysis()
      {
         ShowOutliers = true;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var sourcePopulationBoxWhiskerAnalysis = source as PopulationBoxWhiskerAnalysis;
         if (sourcePopulationBoxWhiskerAnalysis == null) return;
         ShowOutliers = sourcePopulationBoxWhiskerAnalysis.ShowOutliers;
      }
   }
}