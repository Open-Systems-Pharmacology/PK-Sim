using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationParameterChangeTracker : ContextSpecification<SimulationParameterChangeTracker>
   {
      protected override void Context()
      {
         sut = new SimulationParameterChangeTracker();
      }
   }

   public class When_tracking_a_parameter_path : concern_for_SimulationParameterChangeTracker
   {
      protected override void Because()
      {
         sut.Track("Organism|Aspirin|Lipophilicity".ToObjectPath());
      }

      [Observation]
      public void should_have_uncommitted_changes()
      {
         sut.HasUncommittedChanges.ShouldBeTrue();
      }

      [Observation]
      public void should_contain_the_tracked_path()
      {
         sut.GetChangedPaths().Count.ShouldBeEqualTo(1);
         sut.GetChangedPaths()[0].PathAsString.ShouldBeEqualTo("Organism|Aspirin|Lipophilicity");
      }
   }

   public class When_tracking_the_same_path_twice : concern_for_SimulationParameterChangeTracker
   {
      protected override void Because()
      {
         sut.Track("Organism|Aspirin|Lipophilicity".ToObjectPath());
         sut.Track("Organism|Aspirin|Lipophilicity".ToObjectPath());
      }

      [Observation]
      public void should_only_contain_the_path_once()
      {
         sut.GetChangedPaths().Count.ShouldBeEqualTo(1);
      }
   }

   public class When_untracking_a_parameter_path : concern_for_SimulationParameterChangeTracker
   {
      protected override void Context()
      {
         base.Context();
         sut.Track("Organism|Aspirin|Lipophilicity".ToObjectPath());
         sut.Track("Organism|Aspirin|Permeability".ToObjectPath());
      }

      protected override void Because()
      {
         sut.Untrack("Organism|Aspirin|Lipophilicity".ToObjectPath());
      }

      [Observation]
      public void should_no_longer_contain_the_untracked_path()
      {
         sut.GetChangedPaths().Any(p => p.PathAsString == "Organism|Aspirin|Lipophilicity").ShouldBeFalse();
      }

      [Observation]
      public void should_still_contain_other_tracked_paths()
      {
         sut.GetChangedPaths().Count.ShouldBeEqualTo(1);
         sut.GetChangedPaths()[0].PathAsString.ShouldBeEqualTo("Organism|Aspirin|Permeability");
      }
   }

   public class When_untracking_a_path_that_was_not_tracked : concern_for_SimulationParameterChangeTracker
   {
      protected override void Because()
      {
         sut.Untrack("Organism|Aspirin|Lipophilicity".ToObjectPath());
      }

      [Observation]
      public void should_not_throw_and_remain_empty()
      {
         sut.HasUncommittedChanges.ShouldBeFalse();
      }
   }

   public class When_clearing_all_tracked_paths : concern_for_SimulationParameterChangeTracker
   {
      protected override void Context()
      {
         base.Context();
         sut.Track("Organism|Aspirin|Lipophilicity".ToObjectPath());
         sut.Track("Organism|Aspirin|Permeability".ToObjectPath());
      }

      protected override void Because()
      {
         sut.Clear();
      }

      [Observation]
      public void should_have_no_uncommitted_changes()
      {
         sut.HasUncommittedChanges.ShouldBeFalse();
      }

      [Observation]
      public void should_return_empty_changed_paths()
      {
         sut.GetChangedPaths().Count.ShouldBeEqualTo(0);
      }
   }

   public class When_no_paths_have_been_tracked : concern_for_SimulationParameterChangeTracker
   {
      [Observation]
      public void should_not_have_uncommitted_changes()
      {
         sut.HasUncommittedChanges.ShouldBeFalse();
      }

      [Observation]
      public void should_return_empty_changed_paths()
      {
         sut.GetChangedPaths().Count.ShouldBeEqualTo(0);
      }
   }
}
