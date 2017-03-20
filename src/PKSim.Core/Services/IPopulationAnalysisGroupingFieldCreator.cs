using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public interface IPopulationAnalysisGroupingFieldCreator
   {
      /// <summary>
      /// Creates a grouping field based on the <paramref name="populationAnalysisField"/> given as parameter.
      /// </summary>
      /// <param name="populationAnalysisField">Field for which a grouping should be created</param>
      /// <param name="populationDataCollector">Population simulaation for which the field should be defined</param>
      /// <returns>The created grouping field or null, if the action was cancelled</returns>
      PopulationAnalysisGroupingField CreateGroupingFieldFor(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector);

      /// <summary>
      /// Edit the <paramref name="derivedField"/> given as parameter. Returns true if the field as indeed edited. 
      /// Returns false if the user canceled the action
      /// </summary>
      /// <param name="derivedField">Field to edit</param>
      /// <param name="populationDataCollector">Simulation that will be analyzed using the field</param>
      bool EditDerivedField(PopulationAnalysisDerivedField derivedField, IPopulationDataCollector populationDataCollector);
   }
}