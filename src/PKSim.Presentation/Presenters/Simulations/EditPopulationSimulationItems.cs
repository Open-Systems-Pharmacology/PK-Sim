using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Simulations
{
   public static class EditPopulationSimulationItems
   {
      public static EditPopulationSimulationItem<IPopulationSimulationParametersPresenter> Parameters = new EditPopulationSimulationItem<IPopulationSimulationParametersPresenter>();
      public static EditPopulationSimulationItem<IPopulationSimulationAdvancedParametersPresenter> AdvancedParameters = new EditPopulationSimulationItem<IPopulationSimulationAdvancedParametersPresenter>();
      public static EditPopulationSimulationItem<ISimulationAdvancedParameterDistributionPresenter> ParameterDistribution = new EditPopulationSimulationItem<ISimulationAdvancedParameterDistributionPresenter>();
      public static EditPopulationSimulationItem<IReactionDiagramPresenter> ReactionDiagram = new EditPopulationSimulationItem<IReactionDiagramPresenter>();
      public static EditPopulationSimulationItem<IPKSimSimulationOutputMappingPresenter> Outputs = new EditPopulationSimulationItem<IPKSimSimulationOutputMappingPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Parameters, AdvancedParameters, ParameterDistribution, ReactionDiagram, Outputs};
   }

   public class EditPopulationSimulationItem<TSimulationItemPresenter> : SubPresenterItem<TSimulationItemPresenter> where TSimulationItemPresenter : IEditPopulationSimulationItemPresenter
   {
   }
}