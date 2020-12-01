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
         var venousBlood = _organism.Container(CoreConstants.Organ.VENOUS_BLOOD);
         venousBlood.Container(CoreConstants.Compartment.BLOOD_CELLS).ShouldNotBeNull();
         venousBlood.Container(CoreConstants.Compartment.PLASMA).ShouldNotBeNull();
      }

      [Observation]
      public void the_liver_organ_should_contains_the_compartment_blood_cells_plasma_interstitial_and_intracellular()
      {
         var liver = _organism.Container(CoreConstants.Organ.LIVER);
         liver.Container(CoreConstants.Compartment.BLOOD_CELLS).ShouldNotBeNull();
         liver.Container(CoreConstants.Compartment.PLASMA).ShouldNotBeNull();
         liver.Container(CoreConstants.Compartment.INTERSTITIAL).ShouldNotBeNull();
         liver.Container(CoreConstants.Compartment.INTRACELLULAR).ShouldNotBeNull();
      }


      [Observation]
      public void the_spleen_organ_should_contains_the_compartment_blood_cells_plasma_interstitial_and_intracellular()
      {
         var spleen = _organism.Container(CoreConstants.Organ.SPLEEN);
         spleen.Container(CoreConstants.Compartment.BLOOD_CELLS).ShouldNotBeNull();
         spleen.Container(CoreConstants.Compartment.PLASMA).ShouldNotBeNull();
         spleen.Container(CoreConstants.Compartment.INTERSTITIAL).ShouldNotBeNull();
         spleen.Container(CoreConstants.Compartment.INTRACELLULAR).ShouldNotBeNull();
      }
   }
}