using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationSimulationComparisonDistributionPresenter : IAdvancedParameterDistributionPresenter, IPopulationSimulationComparisonItemPresenter
   {
   }

   public class PopulationSimulationComparisonDistributionPresenter : AdvancedParameterDistributionPresenter, IPopulationSimulationComparisonDistributionPresenter
   {
      public PopulationSimulationComparisonDistributionPresenter(IAdvancedParameterDistributionView view, IPopulationParameterGroupsPresenter parametersPresenter, IRepresentationInfoRepository representationInfoRepository, IEntityPathResolver entityPathResolver, IPopulationDistributionPresenter populationParameterDistributionPresenter, IProjectChangedNotifier projectChangedNotifier) : base(view, parametersPresenter, representationInfoRepository, entityPathResolver, populationParameterDistributionPresenter,projectChangedNotifier)
      {
      }

      public void EditComparison(PopulationSimulationComparison comparison)
      {
         EditParameterDistributionFor(comparison);
      }
   }
}