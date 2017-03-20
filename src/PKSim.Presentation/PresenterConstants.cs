using OSPSuite.Utility.Collections;

namespace PKSim.Presentation
{
   public static class PresenterConstants
   {
      public static class PresenterKeys
      {
         private static readonly Cache<string, string> _presenterKeyCache = new Cache<string, string>();
         
         public static readonly string EditBoxWhiskerAnalysisChartPresenter = createConstant("EditBoxWhiskerAnalysisChartPresenter");
         public static readonly string EditRangeAnalysisChartPresenter = createConstant("EditRangeAnalysisChartPresenter");
         public static readonly string EditScatterAnalysisChartPresenter = createConstant("EditScatterAnalysisChartPresenter");
         public static readonly string EditTimeProfileAnalysisChartPresenter = createConstant("EditTimeProfileAnalysisChartPresenter");
         public static readonly string IndividualPKParametersPresenter = createConstant("IndividualPKParametersPresenter");
         public static readonly string PopulationPKAnalysisPresenter = createConstant("PopulationPKAnalysisPresenter");
         public static readonly string SimulationTimeProfileChartPresenter = createConstant("SimulationTimeProfileChartPresenter");
         public static readonly string GlobalPKAnalysisPresenter = createConstant("GlobalPKAnalysisPresenter");
         public static readonly string ParameterGroupPresenter = createConstant("ParameterGroupPresenter");
         public static readonly string EditAnalyzablePresenter = createConstant("EditAnalyzablePresenter");
         public static readonly string IndividualSimulationComparisonPresenter = createConstant("IndividualSimulationComparisonPresenter");

         private static string createConstant(string presenterName)
         {
            _presenterKeyCache.Add(presenterName, presenterName);
            return presenterName;
         }
      }
   }
}
