using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationAdvancedParameterDistributionPresenter : IAdvancedParameterDistributionPresenter, IEditPopulationSimulationItemPresenter
   {
   }

   public class SimulationAdvancedParameterDistributionPresenter : AdvancedParameterDistributionPresenter, ISimulationAdvancedParameterDistributionPresenter
   {
      public SimulationAdvancedParameterDistributionPresenter(IAdvancedParameterDistributionView view, IPopulationParameterGroupsPresenter parametersPresenter,
         IRepresentationInfoRepository representationInfoRepository, IEntityPathResolver entityPathResolver,
         IPopulationDistributionPresenter populationParameterDistributionPresenter, IProjectChangedNotifier projectChangedNotifier)
         : base(view, parametersPresenter, representationInfoRepository, entityPathResolver, populationParameterDistributionPresenter, projectChangedNotifier)
      {
      }

      public void EditSimulation(PopulationSimulation simulation)
      {
         EditParameterDistributionFor(simulation);
      }
   }
}