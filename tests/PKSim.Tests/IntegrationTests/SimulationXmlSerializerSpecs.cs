using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NUnit.Framework;
using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_serializing_an_individual_simulation : ContextForSerialization<IndividualSimulation>
   {
      private IndividualSimulation _simulation;
      private IndividualSimulation _deserializedSimulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulation();
         // _simulation.CompoundPKFor("COMP1").BioAvailability = 10;
         // _simulation.CompoundPKFor("COMP1").CmaxDDI = 20;
         // _simulation.CompoundPKFor("COMP2").BioAvailability = 30;
         // _simulation.CompoundPKFor("COMP2").AucDDI = 40;
         _deserializedSimulation = SerializeAndDeserialize(_simulation);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_simulation()
      {
         _deserializedSimulation.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_deserialized_the_reaction_building_block()
      {
         _deserializedSimulation.Reactions.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_deserialized_the_simulation_settings()
      {
         _deserializedSimulation.SimulationSettings.ShouldNotBeNull();
      }


      [Observation]
      public void should_have_deserialized_the_solver_settings()
      {
         _deserializedSimulation.Solver.ShouldNotBeNull();
      }


      [Observation]
      public void should_have_deserialized_the_output_selection()
      {
         _deserializedSimulation.OutputSelections.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_deserialized_the_schema_selection()
      {
         _deserializedSimulation.OutputSelections.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_deserialized_references_to_compound_in_compound_properties()
      {
         foreach (var compound in _deserializedSimulation.Compounds)
         {
            var cp = _deserializedSimulation.CompoundPropertiesFor(compound);
            Assert.AreSame(cp.Compound,compound);
         }
      }

      [Observation]
      public void should_have_deserialized_references_to_protocol_in_protocol_properties()
      {
         var protocol = _deserializedSimulation.BuildingBlock<Protocol>();
         foreach (var compoundProperty in _deserializedSimulation.Compounds.Select(_deserializedSimulation.CompoundPropertiesFor))
         {
            Assert.AreSame(compoundProperty.ProtocolProperties.Protocol, protocol);
         }
      }

      [Observation]
      public void should_have_deserialized_the_auc_iv_values()
      {
         // _deserializedSimulation.AucIVFor("COMP1").ShouldBeEqualTo(10);
         // _deserializedSimulation.CmaxDDIFor("COMP1").ShouldBeEqualTo(20);
         // _deserializedSimulation.AucIVFor("COMP2").ShouldBeEqualTo(30);
         // _deserializedSimulation.AucDDIFor("COMP2").ShouldBeEqualTo(40);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         Unregister(_simulation);
         Unregister(_deserializedSimulation);
      }
   }
}