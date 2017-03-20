using System.Linq;
using OSPSuite.Utility.Events;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IPopulationAdvancedParametersPresenter : IAdvancedParametersPresenter, IPopulationItemPresenter
   {
      /// <summary>
      ///    Edit the given random population to enable the definition of advanced parameters
      /// </summary>
      /// <param name="population">Population being edited</param>
      void EditPopulation(PKSim.Core.Model.Population population);
   }

   public class PopulationAdvancedParametersPresenter : AdvancedParametersPresenter, IPopulationAdvancedParametersPresenter
   {
      public PopulationAdvancedParametersPresenter(IAdvancedParametersView view, IEntityPathResolver entityPathResolver,
                                                   IPopulationParameterGroupsPresenter constantParameterGroupsPresenter, IPopulationParameterGroupsPresenter advancedParameterGroupsPresenter,
                                                   IAdvancedParameterPresenter advancedParameterPresenter, IAdvancedParametersTask advancedParametersTask, IEventPublisher eventPublisher)
         : base(view, entityPathResolver, constantParameterGroupsPresenter, advancedParameterGroupsPresenter, advancedParameterPresenter, advancedParametersTask, eventPublisher)
      {
      }

      public void EditPopulation(PKSim.Core.Model.Population population)
      {
         EditAdvancedParametersFor(population, population.AllIndividualParameters());
      }
   }
}