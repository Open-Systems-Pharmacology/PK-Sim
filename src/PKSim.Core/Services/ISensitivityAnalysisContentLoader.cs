using OSPSuite.Core.Domain.SensitivityAnalyses;

namespace PKSim.Core.Services
{
   public interface ISensitivityAnalysisContentLoader
   {
      void LoadContentFor(SensitivityAnalysis sensitivityAnalysis);

   }
}