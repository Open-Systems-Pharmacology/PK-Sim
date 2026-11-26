using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPopulationSimulationAdvancedParametersPresenter : IAdvancedParametersPresenter, IEditPopulationSimulationItemPresenter
   {
   }

   public class PopulationSimulationAdvancedParametersPresenter : AdvancedParametersPresenter, IPopulationSimulationAdvancedParametersPresenter
   {
      public PopulationSimulationAdvancedParametersPresenter(IAdvancedParametersView view, IEntityPathResolver entityPathResolver,
                                                             IPopulationParameterGroupsPresenter constantParameterGroupsPresenter, IPopulationParameterGroupsPresenter advancedParameterGroupsPresenter,
                                                             IAdvancedParameterPresenter advancedParameterPresenter, IAdvancedParametersTask advancedParametersTask, IEventPublisher eventPublisher,
                                                             ICoreUserSettings userSettings) :
                                                                base(
                                                                view, entityPathResolver, constantParameterGroupsPresenter, advancedParameterGroupsPresenter, advancedParameterPresenter,
                                                                advancedParametersTask, eventPublisher, userSettings)
      {
      }

      public void EditSimulation(PopulationSimulation simulation)
      {
         EditAdvancedParametersFor(simulation, simulation.AllPotentialAdvancedParameters);
      }
   }
}