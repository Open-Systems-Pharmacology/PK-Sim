using System;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationPersistableUpdater : OSPSuite.Core.Domain.Services.ISimulationPersistableUpdater
   {
      void UpdatePersistableFromSettings(IndividualSimulation individualSimulation);
      void UpdatePersistableFromSettings(PopulationSimulation populationSimulation);
      void ResetPersistable(Simulation simulation);
   }

   public class SimulationPersistableUpdater : OSPSuite.Core.Domain.Services.SimulationPersistableUpdater, ISimulationPersistableUpdater
   {
      public SimulationPersistableUpdater(IEntitiesInContainerRetriever quantitiesRetriever) : base(quantitiesRetriever)
      {
      }

      public void UpdatePersistableFromSettings(IndividualSimulation individualSimulation)
      {
         UpdateSimulationPersistable(individualSimulation);

         var organism = individualSimulation.Model.Root.Container(Constants.ORGANISM);

         individualSimulation.Compounds.Each(compound => addRequiredOutputForSimulation(organism, compound));
      }

      public void UpdatePersistableFromSettings(PopulationSimulation populationSimulation)
      {
         UpdateSimulationPersistable(populationSimulation);
      }

      private void addRequiredOutputForSimulation(IContainer organism, Compound compound)
      {
         //make sure venous blood plasma is always selected so that PK can be calculated as well
         updatePeristable(organism, CoreConstants.Organ.VenousBlood, CoreConstants.Compartment.Plasma,
            compound.Name, CoreConstants.Observer.CONCENTRATION);

         //make sure peripheral venous blood plasma is always selected so that PK can be calculated as well
         updatePeristable(organism, CoreConstants.Organ.PeripheralVenousBlood, compound.Name,
            CoreConstants.Observer.PLASMA_PERIPHERAL_VENOUS_BLOOD);

         //make sure lumen FabsOral is always selected for fabs calculation
         updatePeristable(organism, CoreConstants.Organ.Lumen, compound.Name,
            CoreConstants.Observer.FABS_ORAL);
      }

      private void updatePeristable(IContainer container, params string[] path)
      {
         var observer = container.EntityAt<Observer>(path);
         if (observer == null) return;
         observer.Persistable = true;
      }

      public void ResetPersistable(Simulation simulation)
      {
         SetPersistable(simulation.All<IMoleculeAmount>(), false);
         SetPersistable(simulation.All<IObserver>(), true);

         setApplicationObserversNonPersistable(simulation);
         setUrineFecesAndBileAmountToPersitable(simulation);
      }

      private void setUrineFecesAndBileAmountToPersitable(Simulation simulation)
      {
         setUrineFecesAndBilePersitable(simulation, setMoleculeAmountToPersistableIn);
      }

      private void setUrineFecesAndBileConcentrationToNonPersitable(Simulation simulation)
      {
         setUrineFecesAndBilePersitable(simulation, setConcentrationObserversToNonPersistableIn);
      }

      private void setUrineFecesAndBilePersitable(Simulation simulation, Action<IContainer> updatePersitableInContainerAction)
      {
         var organism = simulation.Model.Root.Container(Constants.ORGANISM);
         if (organism == null)
            return;

         var urine = organism.EntityAt<IContainer>(CoreConstants.Organ.Kidney, CoreConstants.Compartment.URINE);
         updatePersitableInContainerAction(urine);

         var feces = organism.EntityAt<IContainer>(CoreConstants.Organ.Lumen, CoreConstants.Compartment.FECES);
         updatePersitableInContainerAction(feces);

         var gallBladder = organism.EntityAt<IContainer>(CoreConstants.Organ.Gallbladder);
         updatePersitableInContainerAction(gallBladder);
      }

      private void setMoleculeAmountToPersistableIn(IContainer container)
      {
         if (container == null) return;
         SetPersistable(container.GetAllChildren<IMoleculeAmount>(), true);
      }

      private void setConcentrationObserversToNonPersistableIn(IContainer container)
      {
         if (container == null) return;
         SetPersistable(container.GetAllChildren<IObserver>(x => x.NameIsOneOf(CoreConstants.Observer.CONCENTRATION)), false);
      }

      private void setApplicationObserversNonPersistable(Simulation simulation)
      {
         //Set all observers defined in application to persistable false
         var applicationSet = simulation.Model.Root.GetSingleChildByName<IContainer>(Constants.APPLICATIONS);
         if (applicationSet == null) return;

         foreach (var appObserver in applicationSet.GetAllChildren<IObserver>())
         {
            if (applicationObserverShouldBeShown(appObserver))
               continue;

            appObserver.Persistable = false;
         }
      }

      /// <summary>
      ///    Return true if an application observer should not be hidden from user
      /// </summary>
      /// <param name="observer"></param>
      private bool applicationObserverShouldBeShown(IObserver observer)
      {
         string name = observer.Name;

         if (name.StartsWith(CoreConstants.Observer.FRACTION_SOLID_PREFIX))
            return true;

         if (name.StartsWith(CoreConstants.Observer.FRACTION_DISSOLVED_PREFIX))
            return true;

         if (name.StartsWith(CoreConstants.Observer.FRACTION_INSOLUBLE_PREFIX))
            return true;

         return false;
      }
   }
}