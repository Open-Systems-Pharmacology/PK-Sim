using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPopulationSimulationParametersPresenter : IEditPopulationSimulationItemPresenter, ISimulationParametersPresenter
   {
   }

   public class PopulationSimulationParametersPresenter : SimulationParametersPresenter<PopulationSimulation>, IPopulationSimulationParametersPresenter
   {
      public PopulationSimulationParametersPresenter(ISimulationParametersView view, IParameterGroupsPresenter parameterGroupsPresenter)
         : base(view, parameterGroupsPresenter)
      {
      }

      protected override IEnumerable<IParameter> AllSimulationParametersToShow(PopulationSimulation simulation) =>
         simulation.All<IParameter>().Where(parameterIsShownForPopulationSimulation);

      private bool parameterIsShownForPopulationSimulation(IParameter p)
      {
         //Parameter is one of the core building block. Always display
         if (p.IsOfType(PKSimBuildingBlockType.Compound |
                        PKSimBuildingBlockType.Formulation |
                        PKSimBuildingBlockType.Event |
                        PKSimBuildingBlockType.Protocol))
            return true;

         //now only displays simulation parameters (i.e. Individual and Population parameters are excluded) 
         if (!p.IsOfType(PKSimBuildingBlockType.Simulation))
            return false;

         //Hide expressions parameters
         if (p.HasExpressionName())
            return false;

         //Hide volume parameters defined as formula (of type Simulation)
         if (p.IsNamed(Constants.Parameters.VOLUME))
            return false;

         return !p.CanBeVariedInPopulation;
      }
   }
}