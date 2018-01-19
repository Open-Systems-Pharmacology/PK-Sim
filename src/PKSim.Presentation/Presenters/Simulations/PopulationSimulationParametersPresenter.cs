using System.Linq;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPopulationSimulationParametersPresenter : IEditPopulationSimulationItemPresenter, ISimulationParametersPresenter
   {
   }

   public class PopulationSimulationParametersPresenter : SimulationParametersPresenter, IPopulationSimulationParametersPresenter
   {
      public PopulationSimulationParametersPresenter(ISimulationParametersView view, IParameterGroupsPresenter parameterGroupsPresenter)
         : base(view, parameterGroupsPresenter)
      {
      }

      public void EditSimulation(PopulationSimulation simulation)
      {
         var allParameters = simulation.All<IParameter>().Where(parameterIsShownForPopulationSimulation).ToList();
         _parameterGroupsPresenter.InitializeWith(simulation.Model.Root, allParameters);
      }

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
         if (p.IsExpression())
            return false;

         //Hide volume parameters defined as formula (of type Simulation)
         if (p.IsNamed(Constants.Parameters.VOLUME))
            return false;

         return !p.CanBeVariedInPopulation;
      }
   }
}