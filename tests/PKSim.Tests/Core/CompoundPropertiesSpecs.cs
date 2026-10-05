using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundProperties : ContextSpecification<CompoundProperties>
   {
      protected IndividualSimulation _simulation;
      protected Compound _compound;

      protected override void Context()
      {
         _compound = new Compound().WithName("Midazolam");
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("midazolamTemplate", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound, Name = _compound.Name});
         sut = new CompoundProperties {Compound = _compound};
         _simulation.Properties.AddCompoundProperties(sut);
      }
   }

   public class When_retrieving_the_overwrite_parameter_set_name_of_a_compound_in_a_loaded_simulation : concern_for_CompoundProperties
   {
      protected override void Context()
      {
         base.Context();
         sut.OverwriteParameterSetName = "Saved set";
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, new OverwriteParameterSet {Name = "Renal impairment"});
         _simulation.IsLoaded = true;
      }

      [Observation]
      public void should_return_the_name_of_the_set_selected_in_the_simulation()
      {
         sut.OverwriteParameterSetName.ShouldBeEqualTo("Renal impairment");
      }
   }

   public class When_retrieving_the_overwrite_parameter_set_name_of_a_compound_in_a_loaded_simulation_selecting_no_set : concern_for_CompoundProperties
   {
      protected override void Context()
      {
         base.Context();
         sut.OverwriteParameterSetName = "Saved set";
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, null);
         _simulation.IsLoaded = true;
      }

      [Observation]
      public void should_return_no_name()
      {
         sut.OverwriteParameterSetName.ShouldBeNull();
      }
   }

   public class When_retrieving_the_overwrite_parameter_set_name_of_a_compound_in_a_simulation_that_is_not_loaded : concern_for_CompoundProperties
   {
      protected override void Context()
      {
         base.Context();
         sut.OverwriteParameterSetName = "Renal impairment";
      }

      [Observation]
      public void should_return_the_saved_name()
      {
         sut.OverwriteParameterSetName.ShouldBeEqualTo("Renal impairment");
      }
   }

   public class When_cloning_compound_properties_with_an_overwrite_parameter_set_name : concern_for_CompoundProperties
   {
      private CompoundProperties _clone;

      protected override void Context()
      {
         base.Context();
         sut.OverwriteParameterSetName = "Renal impairment";
      }

      protected override void Because()
      {
         _clone = sut.Clone(A.Fake<ICloneManager>());
      }

      [Observation]
      public void should_keep_the_name_of_the_overwrite_parameter_set()
      {
         _clone.OverwriteParameterSetName.ShouldBeEqualTo("Renal impairment");
      }
   }
}
