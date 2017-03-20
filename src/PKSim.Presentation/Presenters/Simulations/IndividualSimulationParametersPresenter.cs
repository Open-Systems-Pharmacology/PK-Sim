using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IIndividualSimulationParametersPresenter : IEditIndividualSimulationItemPresenter, ISimulationParametersPresenter
   {
   }

   public class IndividualSimulationParametersPresenter : SimulationParametersPresenter, IIndividualSimulationParametersPresenter
   {
      public IndividualSimulationParametersPresenter(ISimulationParametersView view, IParameterGroupsPresenter parameterGroupsPresenter)
         : base(view, parameterGroupsPresenter)
      {
      }

      public void EditSimulation(IndividualSimulation simulation)
      {
         _parameterGroupsPresenter.InitializeWith(simulation.Model.Root, simulation.All<IParameter>());
      }
   }
}