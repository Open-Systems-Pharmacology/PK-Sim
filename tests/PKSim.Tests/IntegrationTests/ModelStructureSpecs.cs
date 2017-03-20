using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ModelStructure : ContextForSimulationIntegration<Simulation>
   {
      protected IContainer _organism;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulation();
         _organism = _simulation.Model.Root.Container(Constants.ORGANISM);
      }
   }

   public class When_creating_a_simulation : concern_for_ModelStructure
   {
      [Observation]
      public void the_venous_blood_organ_should_contains_the_compartment_blood_cells_and_plasma()
      {
         var venousBlood = _organism.Container(CoreConstants.Organ.VenousBlood);
         venousBlood.Container(CoreConstants.Compartment.BloodCells).ShouldNotBeNull();
         venousBlood.Container(CoreConstants.Compartment.Plasma).ShouldNotBeNull();
      }

      [Observation]
      public void the_liver_organ_should_contains_the_compartment_blood_cells_plasma_interstitial_and_intracellular()
      {
         var liver = _organism.Container(CoreConstants.Organ.Liver);
         liver.Container(CoreConstants.Compartment.BloodCells).ShouldNotBeNull();
         liver.Container(CoreConstants.Compartment.Plasma).ShouldNotBeNull();
         liver.Container(CoreConstants.Compartment.Interstitial).ShouldNotBeNull();
         liver.Container(CoreConstants.Compartment.Intracellular).ShouldNotBeNull();
      }


      [Observation]
      public void the_spleen_organ_should_contains_the_compartment_blood_cells_plasma_interstitial_and_intracellular()
      {
         var spleen = _organism.Container(CoreConstants.Organ.Spleen);
         spleen.Container(CoreConstants.Compartment.BloodCells).ShouldNotBeNull();
         spleen.Container(CoreConstants.Compartment.Plasma).ShouldNotBeNull();
         spleen.Container(CoreConstants.Compartment.Interstitial).ShouldNotBeNull();
         spleen.Container(CoreConstants.Compartment.Intracellular).ShouldNotBeNull();
      }
   }
}