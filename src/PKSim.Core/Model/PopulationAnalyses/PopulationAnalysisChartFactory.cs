using System;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public interface IPopulationAnalysisChartFactory
   {
      TPopulationAnalysisChart Create<TPopulationAnalysis, TPopulationAnalysisChart>()
         where TPopulationAnalysisChart : PopulationAnalysisChart<TPopulationAnalysis>, new()
         where TPopulationAnalysis : PopulationAnalysis;

      PopulationAnalysisChart Create(PopulationAnalysisType populationAnalysisType);
   }

   public class PopulationAnalysisChartFactory : IPopulationAnalysisChartFactory
   {
      private readonly IPopulationAnalysisFactory _populationAnalysisFactory;

      public PopulationAnalysisChartFactory(IPopulationAnalysisFactory populationAnalysisFactory)
      {
         _populationAnalysisFactory = populationAnalysisFactory;
      }

      public TPopulationAnalysisChart Create<TPopulationAnalysis, TPopulationAnalysisChart>()
         where TPopulationAnalysisChart : PopulationAnalysisChart<TPopulationAnalysis>, new()
         where TPopulationAnalysis : PopulationAnalysis
      {
         return new TPopulationAnalysisChart
         {
            PopulationAnalysis = _populationAnalysisFactory.Create<TPopulationAnalysis>()
         };
      }

      public PopulationAnalysisChart Create(PopulationAnalysisType populationAnalysisType)
      {
         switch (populationAnalysisType)
         {
            case PopulationAnalysisType.TimeProfile:
               return Create<PopulationStatisticalAnalysis, TimeProfileAnalysisChart>();
            case PopulationAnalysisType.BoxWhisker:
               return Create<PopulationBoxWhiskerAnalysis, BoxWhiskerAnalysisChart>();
            case PopulationAnalysisType.Scatter:
               return Create<PopulationPivotAnalysis, ScatterAnalysisChart>();
            case PopulationAnalysisType.Range:
               return Create<PopulationPivotAnalysis, RangeAnalysisChart>();
            default:
               throw new ArgumentOutOfRangeException("populationAnalysisType", populationAnalysisType, null);
         }
      }
   }
}