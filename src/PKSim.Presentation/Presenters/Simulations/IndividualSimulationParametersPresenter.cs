using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IIndividualSimulationParametersPresenter : IEditIndividualSimulationItemPresenter, ISimulationParametersPresenter
   {
   }

   public class IndividualSimulationParametersPresenter : SimulationParametersPresenter<IndividualSimulation>, IIndividualSimulationParametersPresenter
   {
      public IndividualSimulationParametersPresenter(ISimulationParametersView view, IParameterGroupsPresenter parameterGroupsPresenter)
         : base(view, parameterGroupsPresenter)
      {
      }

      protected override IEnumerable<IParameter> AllSimulationParametersToShow(IndividualSimulation simulation) => simulation.All<IParameter>();
   }
}