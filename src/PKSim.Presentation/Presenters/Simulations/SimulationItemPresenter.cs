using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IConfigurationPresenter : ISubPresenter
   {
      /// <summary>
      ///    Save the configuration defined in the presenter
      /// </summary>
      void SaveConfiguration();
   }

   public interface IEditSimulationPresenter : ISubPresenter
   {
      void EditSimulation(Simulation simulation, CreationMode creationMode);
   }

   public interface IEditSimulationCompoundPresenter : IConfigurationPresenter
   {
      void EditSimulation(Simulation simulation, Compound compound);
   }

   public interface ISimulationItemPresenter : IEditSimulationPresenter, IConfigurationPresenter
   {
   }

   public interface IEditSimulationItemPresenter<TSimulation> : ISubPresenter where TSimulation : Simulation
   {
      void EditSimulation(TSimulation simulation);
   }

   public interface IEditIndividualSimulationItemPresenter : IEditSimulationItemPresenter<IndividualSimulation>
   {
   }

   public interface IEditPopulationSimulationItemPresenter : IEditSimulationItemPresenter<PopulationSimulation>
   {
   }
}