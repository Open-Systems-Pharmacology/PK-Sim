using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IPopulationAdvancedParameterDistributionPresenter : IAdvancedParameterDistributionPresenter, IPopulationItemPresenter
   {
      /// <summary>
      ///    Edit the distributed parameters for the given population
      /// </summary>
      void EditPopulation(Population population);
   }

   public class PopulationAdvancedParameterDistributionPresenter : AdvancedParameterDistributionPresenter, IPopulationAdvancedParameterDistributionPresenter
   {
      public PopulationAdvancedParameterDistributionPresenter(IAdvancedParameterDistributionView view, IPopulationParameterGroupsPresenter parametersPresenter, IRepresentationInfoRepository representationInfoRepository, IEntityPathResolver entityPathResolver, IPopulationDistributionPresenter populationParameterDistributionPresenter, IProjectChangedNotifier projectChangedNotifier)
         : base(view, parametersPresenter, representationInfoRepository, entityPathResolver, populationParameterDistributionPresenter, projectChangedNotifier)
      {
      }

      public void EditPopulation(Population population)
      {
         EditParameterDistributionFor(population);
      }
   }
}