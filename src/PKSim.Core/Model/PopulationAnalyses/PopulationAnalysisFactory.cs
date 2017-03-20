using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using OSPSuite.Core.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public interface IPopulationAnalysisFactory
   {
      PopulationPivotAnalysis CreatePivotAnalysis();
      PopulationStatisticalAnalysis CreateStatisticalAnalysis();
      PopulationBoxWhiskerAnalysis CreateBoxWhiskerAnalysis();
      T Create<T>() where T : PopulationAnalysis;
   }

   public class PopulationAnalysisFactory : IPopulationAnalysisFactory
   {
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;

      public PopulationAnalysisFactory(IDimensionRepository dimensionRepository, IDisplayUnitRetriever displayUnitRetriever)
      {
         _dimensionRepository = dimensionRepository;
         _displayUnitRetriever = displayUnitRetriever;
      }

      public PopulationPivotAnalysis CreatePivotAnalysis()
      {
         return new PopulationPivotAnalysis();
      }

      public PopulationBoxWhiskerAnalysis CreateBoxWhiskerAnalysis()
      {
         return new PopulationBoxWhiskerAnalysis();
      }

      public PopulationStatisticalAnalysis CreateStatisticalAnalysis()
      {
         var statisticalAnalysis = new PopulationStatisticalAnalysis {TimeUnit =_displayUnitRetriever.PreferredUnitFor(_dimensionRepository.Time)};

         //add predefined single curve selection
         EnumHelper.AllValuesFor<StatisticalAggregationType>()
            .Each(x => statisticalAnalysis.AddStatistic(new PredefinedStatisticalAggregation {Method = x}));

         CoreConstants.DEFAULT_STATISTIC_PERCENTILES.Each(p => statisticalAnalysis.AddStatistic(new PercentileStatisticalAggregation {Percentile = p}));

         return statisticalAnalysis;
      }

      public T Create<T>() where T : PopulationAnalysis
      {
         if (typeof (T).IsAnImplementationOf<PopulationStatisticalAnalysis>())
            return CreateStatisticalAnalysis().DowncastTo<T>();

         if (typeof (T).IsAnImplementationOf<PopulationBoxWhiskerAnalysis>())
            return CreateBoxWhiskerAnalysis().DowncastTo<T>();

         return CreatePivotAnalysis().DowncastTo<T>();
      }
   }
}