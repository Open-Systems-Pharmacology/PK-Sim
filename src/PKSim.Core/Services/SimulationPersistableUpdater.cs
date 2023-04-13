using System;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationPersistableUpdater : OSPSuite.Core.Domain.Services.ISimulationPersistableUpdater
   {
      void UpdatePersistableFromSettings(Simulation populationSimulation);
      void ResetPersistable(Simulation simulation);
   }

   public class SimulationPersistableUpdater : OSPSuite.Core.Domain.Services.SimulationPersistableUpdater, ISimulationPersistableUpdater
   {
      public SimulationPersistableUpdater(IEntitiesInContainerRetriever quantitiesRetriever) : base(quantitiesRetriever)
      {
      }

      public void UpdatePersistableFromSettings(Simulation individualSimulation)
      {
         UpdateSimulationPersistable(individualSimulation);

         var organism = individualSimulation.Model.Root.Container(Constants.ORGANISM);

         individualSimulation.Compounds.Each(compound => addRequiredOutputForSimulation(organism, compound));
      }

      private void addRequiredOutputForSimulation(IContainer organism, Compound compound)
      {
         //make sure venous blood plasma is always selected so that PK can be calculated as well
         updatePersistable(organism, CoreConstants.Organ.VENOUS_BLOOD, CoreConstants.Compartment.PLASMA,
            compound.Name, CoreConstants.Observer.CONCENTRATION_IN_CONTAINER);

         //make sure peripheral venous blood plasma is always selected so that PK can be calculated as well
         updatePersistable(organism, CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD, compound.Name,
            CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD);

         //make sure lumen FabsOral is always selected for fabs calculation
         updatePersistable(organism, CoreConstants.Organ.LUMEN, compound.Name,
            CoreConstants.Observer.FABS_ORAL);
      }

      private void updatePersistable(IContainer container, params string[] path)
      {
         var observer = container.EntityAt<Observer>(path);
         if (observer == null) return;
         observer.Persistable = true;
      }

      public void ResetPersistable(Simulation simulation)
      {
         SetPersistable(simulation.All<MoleculeAmount>(), false);
         SetPersistable(simulation.All<Observer>(), true);

         setApplicationObserversNonPersistable(simulation);
         setUrineFecesAndBileAmountToPersitable(simulation);
      }

      private void setUrineFecesAndBileAmountToPersitable(Simulation simulation)
      {
         setUrineFecesAndBilePersitable(simulation, setMoleculeAmountToPersistableIn);
      }

      private void setUrineFecesAndBilePersitable(Simulation simulation, Action<IContainer> updatePersitableInContainerAction)
      {
         var organism = simulation.Model.Root.Container(Constants.ORGANISM);
         if (organism == null)
            return;

         var urine = organism.EntityAt<IContainer>(CoreConstants.Organ.KIDNEY, CoreConstants.Compartment.URINE);
         updatePersitableInContainerAction(urine);

         var feces = organism.EntityAt<IContainer>(CoreConstants.Organ.LUMEN, CoreConstants.Compartment.FECES);
         updatePersitableInContainerAction(feces);

         var gallBladder = organism.EntityAt<IContainer>(CoreConstants.Organ.GALLBLADDER);
         updatePersitableInContainerAction(gallBladder);
      }

      private void setMoleculeAmountToPersistableIn(IContainer container)
      {
         if (container == null) return;
         SetPersistable(container.GetAllChildren<MoleculeAmount>(), true);
      }

      private void setApplicationObserversNonPersistable(Simulation simulation)
      {
         //Set all observers defined in application to persistable false
         var applications = simulation.Model.Root.Container(Constants.APPLICATIONS);

         applications?.GetAllChildren<Observer>(applicationObserverShouldBeHidden)
            .Each(x => x.Persistable = false);
      }

      /// <summary>
      ///    Return true if an application observer should be hidden from user
      /// </summary>
      /// <param name="observer"></param>
      private bool applicationObserverShouldBeHidden(Observer observer) => observer.Name == CoreConstants.Observer.CONCENTRATION_IN_CONTAINER;
   }
}