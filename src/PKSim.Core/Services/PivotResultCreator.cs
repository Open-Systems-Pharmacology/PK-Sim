using OSPSuite.Utility.Data;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public interface IPivotResultCreator
   {
      /// <summary>
      ///    Using the row data displayed in the pivot, creates the pivot result data that will be pass along to the populaton
      ///    analysis chart
      /// </summary>
      /// <param name="pivotAnalysis">The population pivot analysis being run</param>
      /// <param name="populationDataCollector">The analyzer containing the data for the analysis</param>
      /// <param name="observedDataCollection">Collection of observed data that ought to be displayed</param>
      /// <param name="aggregate">The aggregation function used to create the plot data</param>
      PivotResult Create(PopulationPivotAnalysis pivotAnalysis, IPopulationDataCollector populationDataCollector, ObservedDataCollection observedDataCollection, Aggregate aggregate);
   }

   public class PivotResultCreator : IPivotResultCreator
   {
      private readonly IPivoter _pivoter;
      private readonly IPopulationAnalysisFlatTableCreator _flatTableCreator;
      private const string DATA_FIELD_NAME = "Data";

      public PivotResultCreator(IPivoter pivoter, IPopulationAnalysisFlatTableCreator flatTableCreator)
      {
         _pivoter = pivoter;
         _flatTableCreator = flatTableCreator;
      }

      public PivotResult Create(PopulationPivotAnalysis pivotAnalysis, IPopulationDataCollector populationDataCollector, ObservedDataCollection observedDataCollection, Aggregate aggregate)
      {
         var data = _flatTableCreator.Create(populationDataCollector, pivotAnalysis);
         var pivotInfo = getPivotInfo(pivotAnalysis, aggregate);
         var pivotedData = _pivoter.PivotData(data.DefaultView, pivotInfo);
         return new PivotResult(pivotAnalysis, pivotedData, populationDataCollector, observedDataCollection, aggregate.Name, pivotInfo.DataFieldColumnName);
      }

      private PivotInfo getPivotInfo(PopulationPivotAnalysis pivotAnalysis, Aggregate aggregate)
      {
         return new PivotInfo(
            rowFields: pivotAnalysis.AllFieldNamesOn(PivotArea.RowArea),
            columnFields: pivotAnalysis.AllFieldNamesOn(PivotArea.ColumnArea),
            dataFields: pivotAnalysis.AllFieldNamesOn(PivotArea.DataArea),
            aggregates: new[] {aggregate},
            mode: PivotInfo.PivotMode.FlatTable,
            dataFieldColumnName: DATA_FIELD_NAME);
      }
   }
}