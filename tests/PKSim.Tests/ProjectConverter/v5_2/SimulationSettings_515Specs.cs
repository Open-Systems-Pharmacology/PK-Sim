using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public abstract class concern_for_SimulationSettings_515 : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimulationSettings_515");
      }
   }

   public class When_converting_the_project_SimulationSettings_515 : concern_for_SimulationSettings_515
   {
      private Simulation _simulation;
      private SimpleProtocol _protocol;
      private Compound _compound;

      protected override void Context()
      {
         _simulation = First<Simulation>();
         _protocol = First<SimpleProtocol>();
         _compound = First<Compound>();
      }

      [Observation]
      public void should_have_converted_the_simulation_settings_to_contain_some_selected_quantities()
      {
         _simulation.OutputSelections.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_have_converted_the_charts_available_in_the_simulation()
      {
         _simulation.Analyses.Count().ShouldBeEqualTo(3);
      }


      [Observation]
      public void should_have_remove_the_name_of_the_simulation_at_the_beginning_of_each_selected_path()
      {
         foreach (var selectedQuantity in _simulation.OutputSelections)
         {
            selectedQuantity.Path.StartsWith(_simulation.Name).ShouldBeFalse();
         }
      }

      [Observation]
      public void should_have_converted_the_end_time_parameter_to_use_the_new_nomenclature_in_the_simulation_output()
      {
         foreach (var simulationInterval in _simulation.OutputSchema.Intervals)
         {
            simulationInterval.EndTime.ShouldNotBeNull();
         }
      }

      [Observation]
      public void should_have_updated_the_resolution_parameter()
      {
         foreach (var simulationInterval in _simulation.OutputSchema.Intervals)
         {
            simulationInterval.Resolution.ValueInDisplayUnit.ShouldBeGreaterThan(0);
         }
      }

      [Observation]
      public void should_have_converted_the_end_time_parameter_to_use_the_new_nomenclature_in_the_protocol()
      {
         _protocol.EndTimeParameter.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_the_mol_weight_of_the_compound_in_the_building_block_and_in_the_simulation()
      {
         _compound.Parameter(Constants.Parameters.MOL_WEIGHT).ValueInDisplayUnit.ShouldBeEqualTo(250, 1e-2);
         var simCompound = _simulation.BuildingBlock<Compound>();
         simCompound.Parameter(Constants.Parameters.MOL_WEIGHT).ValueInDisplayUnit.ShouldBeEqualTo(250, 1e-2);
         var modelComp = _simulation.Model.Root.Container(_compound.Name);
         modelComp.Parameter(Constants.Parameters.MOL_WEIGHT).ValueInDisplayUnit.ShouldBeEqualTo(250, 1e-2);
      }
   }
}