using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IGroupingDefinitionPresenter : IPresenter
   {
      /// <summary>
      /// Returns the grouping definiton defined by the user
      /// </summary>
      GroupingDefinition GroupingDefinition { get; }

      /// <summary>
      /// Initializes the grouping definition presenter allowing the user to setup a new <see cref="IGroupingDefinition"/>
      /// </summary>
      /// <param name="populationAnalysisField">Field for which the grouping definition should be defined</param>
      /// <param name="populationDataCollector">Population simulation used in the analyse</param>
      void InitializeWith(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector);

      /// <summary>
      /// Edit the grouping definition <paramref name="groupingDefinition"/>
      /// </summary>
      void Edit(GroupingDefinition groupingDefinition);

      /// <summary>
      /// This should be called when editing is over to save the changes back in the grouping definition
      /// </summary>
      void UpdateGroupingDefinition();

      /// <summary>
      /// starts the creation process
      /// </summary>
      void StartCreate();
   }
}