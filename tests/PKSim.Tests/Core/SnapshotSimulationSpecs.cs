using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Snapshots;

namespace PKSim.Core
{
   public abstract class concern_for_SnapshotSimulation : ContextSpecification<Simulation>
   {
      protected LocalizedParameter _parameter1 = new LocalizedParameter {Path = "LOCALIZED_PARAMETER_1", Value = 1};
      protected LocalizedParameter _parameter2 = new LocalizedParameter {Path = "LOCALIZED_PARAMETER_2"};

      protected override void Context()
      {
         sut = new Simulation();
      }
   }

   public class When_adding_a_parameter_that_does_not_exist_by_path : concern_for_SnapshotSimulation
   {
      protected override void Because()
      {
         sut.AddOrUpdate(_parameter1);
      }

      [Observation]
      public void should_add_the_parameter_to_the_list_of_simulation_parameters()
      {
         sut.Parameters.ShouldContain(_parameter1);
      }
   }

   public class When_adding_a_parameter_that_already_exists_by_path : concern_for_SnapshotSimulation
   {
      private LocalizedParameter _otherParameter;

      protected override void Context()
      {
         base.Context();
         sut.AddOrUpdate(_parameter1);
         _otherParameter = new LocalizedParameter {Path = _parameter1.Path, Value = 2};
      }
      `
      protected override void Because()
      {
         sut.AddOrUpdate(_otherParameter);
      }

      [Observation]
      public void should_have_swapped_the_parameter_by_path()
      {
         sut.Parameters.ShouldContain(_otherParameter);
         sut.Parameters.ShouldNotContain(_parameter1);
      }
   }
}