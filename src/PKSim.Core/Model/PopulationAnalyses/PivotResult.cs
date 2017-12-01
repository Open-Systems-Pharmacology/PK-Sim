using System.Data;

namespace PKSim.Core.Model.PopulationAnalyses
{
   /// <summary>
   /// Contains the information required to create the ChartData for a given analysis
   /// </summary>
   public class PivotResult
   {
      /// <summary>
      /// Analysis for which chart data should be created
      /// </summary>
      public PopulationPivotAnalysis Analysis { get; }
      
      /// <summary>
      /// Data resulting of the pivotation
      /// </summary>
      public DataTable PivotedData { get; }

      /// <summary>
      /// Reference to the <see cref="IPopulationDataCollector"/> used to create the <see cref="PivotedData"/> based on the <see cref="Analysis"/>
      /// </summary>
      public IPopulationDataCollector PopulationDataCollector { get; }

      /// <summary>
      /// Reference to the <see cref="ObservedDataCollection"/> that might be associated to the <see cref="Analysis"/> that should be displayed
      /// </summary>
      public ObservedDataCollection ObservedDataCollection { get; }

      /// <summary>
      /// Name of the column in the <see cref="PivotedData"/> that will contain the value of the aggregation
      /// </summary>
      public string AggregationName { get; }

      /// <summary>
      /// Name of the column in the <see cref="PivotedData"/> that will contain the key representing the aggregated data
      /// </summary>
      public string DataColumnName { get; }

      public PivotResult(PopulationPivotAnalysis analysis, DataTable pivotedData, IPopulationDataCollector populationDataCollector, ObservedDataCollection observedDataCollection, string aggregationName, string dataColumnName)
      {
         Analysis = analysis;
         PivotedData = pivotedData;
         PopulationDataCollector = populationDataCollector;
         ObservedDataCollection = observedDataCollection;
         AggregationName = aggregationName;
         DataColumnName = dataColumnName;
      }
   }
}