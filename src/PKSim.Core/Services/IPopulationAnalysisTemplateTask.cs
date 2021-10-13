using System.Threading.Tasks;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public interface IPopulationAnalysisTemplateTask
   {
      /// <summary>
      /// Loads a new derived field for the data field <paramref name="populationAnalysisDataField"/> belonging to the <paramref name="populationAnalysis"/>. 
      /// The derived field  is not added to the population. However the name of the derived field will be changed if required to ensure
      /// unicity in the <paramref name="populationAnalysis"/>
      /// </summary>
      Task<PopulationAnalysisDerivedField> LoadDerivedFieldFor(PopulationAnalysis populationAnalysis, PopulationAnalysisDataField populationAnalysisDataField);
      void SaveDerivedField(PopulationAnalysisDerivedField derivedField);
      Task<TPopulationAnalysis> LoadPopulationAnalysisFor<TPopulationAnalysis>(IPopulationDataCollector populationDataCollector) where TPopulationAnalysis : PopulationAnalysis, new();
      void SavePopulationAnalysis<TPopulationAnalysis>(TPopulationAnalysis populationAnalysis) where TPopulationAnalysis : PopulationAnalysis;
      Task LoadPopulationAnalysisWorkflowInto(IPopulationDataCollector populationDataCollector);
      void SavePopulationAnalysisWorkflowFrom(IPopulationDataCollector populationDataCollector);
   }
}