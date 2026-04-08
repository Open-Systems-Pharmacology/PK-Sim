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

   public class When_tracking_a_parameter_path_with_object_path : concern_for_SimulationParameterChangeTracker
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
         sut.ChangedPaths.Count.ShouldBeEqualTo(1);
         sut.ChangedPaths[0].PathAsString.ShouldBeEqualTo("Organism|Aspirin|Lipophilicity");
      }
   }

   public class When_tracking_a_parameter_path : concern_for_SimulationParameterChangeTracker
   {
      protected override void Because()
      {
         sut.Track("Organism|Aspirin|Lipophilicity");
      }

      [Observation]
      public void should_have_uncommitted_changes()
      {
         sut.HasUncommittedChanges.ShouldBeTrue();
      }

      [Observation]
      public void should_contain_the_tracked_path()
      {
         sut.IsTracked("Organism|Aspirin|Lipophilicity").ShouldBeTrue();
      }
   }

   public class When_tracking_the_same_path_twice : concern_for_SimulationParameterChangeTracker
   {
      protected override void Because()
      {
         sut.Track("Organism|Aspirin|Lipophilicity");
         sut.Track("Organism|Aspirin|Lipophilicity");
      }

      [Observation]
      public void should_only_contain_the_path_once()
      {
         sut.ChangedPaths.Count.ShouldBeEqualTo(1);
      }
   }

   public class When_untracking_a_parameter_path : concern_for_SimulationParameterChangeTracker
   {
      protected override void Context()
      {
         base.Context();
         sut.Track("Organism|Aspirin|Lipophilicity");
         sut.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         sut.Untrack("Organism|Aspirin|Lipophilicity");
      }

      [Observation]
      public void should_no_longer_contain_the_untracked_path()
      {
         sut.IsTracked("Organism|Aspirin|Lipophilicity").ShouldBeFalse();
      }

      [Observation]
      public void should_still_contain_other_tracked_paths()
      {
         sut.ChangedPaths.Count.ShouldBeEqualTo(1);
         sut.IsTracked("Organism|Aspirin|Permeability").ShouldBeTrue();
      }
   }

   public class When_untracking_a_path_that_was_not_tracked : concern_for_SimulationParameterChangeTracker
   {
      protected override void Because()
      {
         sut.Untrack("Organism|Aspirin|Lipophilicity");
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
         sut.Track("Organism|Aspirin|Lipophilicity");
         sut.Track("Organism|Aspirin|Permeability");
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
         sut.ChangedPaths.Count.ShouldBeEqualTo(0);
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
         sut.ChangedPaths.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_cloning_the_tracker : concern_for_SimulationParameterChangeTracker
   {
      private SimulationParameterChangeTracker _clone;

      protected override void Context()
      {
         base.Context();
         sut.Track("Organism|Aspirin|Lipophilicity");
         sut.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         _clone = sut.Clone();
      }

      [Observation]
      public void should_contain_the_same_paths()
      {
         _clone.ChangedPaths.Count.ShouldBeEqualTo(2);
         _clone.IsTracked("Organism|Aspirin|Lipophilicity").ShouldBeTrue();
         _clone.IsTracked("Organism|Aspirin|Permeability").ShouldBeTrue();
      }

      [Observation]
      public void should_be_independent_from_original()
      {
         _clone.Track("Organism|NewPath");
         sut.ChangedPaths.Count.ShouldBeEqualTo(2);
         _clone.ChangedPaths.Count.ShouldBeEqualTo(3);
      }
   }
}
