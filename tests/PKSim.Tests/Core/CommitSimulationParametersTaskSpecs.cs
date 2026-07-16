using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_CommitSimulationParametersTask : ContextSpecification<CommitSimulationParametersTask>
   {
      protected IExecutionContext _executionContext;
      protected IContainerTask _containerTask;
      protected PKSim.Core.Repositories.IBuildingBlockRepository _buildingBlockRepository;
      protected IndividualSimulation _simulation;
      protected Compound _simulationCompound;
      protected Compound _templateCompound;
      protected IParameter _lipophilicityParam;
      protected IParameter _permeabilityParam;
      protected PathCache<IParameter> _parameterCache;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _containerTask = A.Fake<IContainerTask>();
         _buildingBlockRepository = A.Fake<PKSim.Core.Repositories.IBuildingBlockRepository>();

         _simulationCompound = new Compound { Name = "Aspirin", Id = "SimCompId" };
         _templateCompound = new Compound { Name = "Aspirin", Id = "TemplateCompId" };

         _lipophilicityParam = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         _permeabilityParam = DomainHelperForSpecs.ConstantParameterWithValue(7.2).WithName("Permeability");

         var root = new Container { Name = "Sim" };
         root.Add(_lipophilicityParam);
         root.Add(_permeabilityParam);

         _simulation = new IndividualSimulation
         {
            Id = "SimId",
            Model = new OSPSuite.Core.Domain.Model { Root = root }
         };

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateCompId", PKSimBuildingBlockType.Compound) { BuildingBlock = _simulationCompound });
         A.CallTo(() => _buildingBlockRepository.ById<Compound>("TemplateCompId")).Returns(_templateCompound);

         _parameterCache = new PathCacheForSpecs<IParameter>();
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", _lipophilicityParam);
         _parameterCache.Add("Organism|Aspirin|Permeability", _permeabilityParam);

         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);
         A.CallTo(() => _executionContext.TypeFor(_simulationCompound)).Returns("Compound");

         sut = new CommitSimulationParametersTask(_executionContext, _containerTask, _buildingBlockRepository);
      }
   }

   public class When_committing_parameters_to_a_new_overwrite_parameter_set : concern_for_CommitSimulationParametersTask
   {
      private ICommand _result;

      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         _result = sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity", "Organism|Aspirin|Permeability" },
            OverwriteParameterSetName = "MyNewSet",
            ShouldCreateNew = true
         });
      }

      [Observation]
      public void should_clear_committed_paths_from_tracker()
      {
         _simulation.ParameterChangeTracker.HasUncommittedChanges.ShouldBeFalse();
      }

      [Observation]
      public void should_add_the_overwrite_parameter_set_to_the_simulation_compound()
      {
         _simulationCompound.OverwriteParameterSets.Count.ShouldBeEqualTo(1);
         _simulationCompound.OverwriteParameterSets[0].Name.ShouldBeEqualTo("MyNewSet");
      }

      [Observation]
      public void should_add_the_overwrite_parameter_set_to_the_template_compound()
      {
         _templateCompound.OverwriteParameterSets.Count.ShouldBeEqualTo(1);
         _templateCompound.OverwriteParameterSets[0].Name.ShouldBeEqualTo("MyNewSet");
      }

      [Observation]
      public void should_set_building_block_properties_on_the_command()
      {
         A.CallTo(() => _executionContext.UpdateBuildingBlockPropertiesInCommand(A<IOSPSuiteCommand>._, _simulationCompound)).MustHaveHappened();
      }

      [Observation]
      public void should_publish_a_simulation_status_changed_event_for_the_simulation()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationStatusChangedEvent>.That.Matches(e => e.Simulation == _simulation))).MustHaveHappened();
      }
   }

   public class When_committing_parameters_to_an_existing_overwrite_parameter_set : concern_for_CommitSimulationParametersTask
   {
      private ICommand _result;
      private OverwriteParameterSet _simulationExistingSet;
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         _simulationExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId1" };
         _simulationExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|OldParam".ToObjectPath(), Value = 1.0 });
         _simulationCompound.AddOverwriteParameterSet(_simulationExistingSet);

         _templateExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId2" };
         _templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|OldParam".ToObjectPath(), Value = 1.0 });
         _templateCompound.AddOverwriteParameterSet(_templateExistingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         _result = sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity" },
            OverwriteParameterSetName = "ExistingSet",
            ShouldCreateNew = false
         });
      }

      [Observation]
      public void should_clear_committed_paths_from_tracker()
      {
         _simulation.ParameterChangeTracker.IsTracked("Organism|Aspirin|Lipophilicity").ShouldBeFalse();
      }

      [Observation]
      public void should_update_the_simulation_existing_set_with_new_parameter_value()
      {
         _simulationExistingSet.ParameterValues.Any(pv => pv.Path.PathAsString == "Organism|Aspirin|Lipophilicity").ShouldBeTrue();
      }

      [Observation]
      public void should_update_the_template_existing_set_with_new_parameter_value()
      {
         _templateExistingSet.ParameterValues.Any(pv => pv.Path.PathAsString == "Organism|Aspirin|Lipophilicity").ShouldBeTrue();
      }

      [Observation]
      public void should_preserve_existing_entries_for_parameters_no_longer_present_in_the_simulation()
      {
         _simulationExistingSet.ParameterValues.Any(pv => pv.Path.PathAsString == "Organism|Aspirin|OldParam").ShouldBeTrue();
         _templateExistingSet.ParameterValues.Any(pv => pv.Path.PathAsString == "Organism|Aspirin|OldParam").ShouldBeTrue();
      }
   }

   public class When_committing_a_tracked_parameter_to_an_existing_set_with_other_untouched_entries : concern_for_CommitSimulationParametersTask
   {
      private OverwriteParameterSet _simulationExistingSet;
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         //Permeability is in the existing set with the same value the simulation parameter currently holds (7.2),
         //modelling a parameter that was previously committed and has not been touched since.
         _simulationExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId1" };
         _simulationExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 7.2 });
         _simulationCompound.AddOverwriteParameterSet(_simulationExistingSet);

         _templateExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId2" };
         _templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 7.2 });
         _templateCompound.AddOverwriteParameterSet(_templateExistingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity" },
            OverwriteParameterSetName = "ExistingSet",
            ShouldCreateNew = false
         });
      }

      [Observation]
      public void should_preserve_the_existing_untouched_entry_in_the_simulation_set()
      {
         _simulationExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldNotBeNull();
      }

      [Observation]
      public void should_preserve_the_existing_untouched_entry_in_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldNotBeNull();
      }

      [Observation]
      public void should_add_the_newly_committed_entry_to_the_simulation_set()
      {
         _simulationExistingSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").ShouldNotBeNull();
      }
   }

   public class When_committing_to_an_existing_set_after_user_has_reset_a_parameter_in_that_set : concern_for_CommitSimulationParametersTask
   {
      private OverwriteParameterSet _simulationExistingSet;
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         //Permeability is in the existing set with stored value 99.0. The simulation's Permeability parameter is
         //at 7.2 — different from the stored value and not tracked — which models a parameter the user has reset
         //since the previous commit.
         _simulationExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId1" };
         _simulationExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 99.0 });
         _simulationCompound.AddOverwriteParameterSet(_simulationExistingSet);

         _templateExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId2" };
         _templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 99.0 });
         _templateCompound.AddOverwriteParameterSet(_templateExistingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity" },
            OverwriteParameterSetName = "ExistingSet",
            ShouldCreateNew = false
         });
      }

      [Observation]
      public void should_remove_the_reset_entry_from_the_simulation_set()
      {
         _simulationExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldBeNull();
      }

      [Observation]
      public void should_remove_the_reset_entry_from_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldBeNull();
      }

      [Observation]
      public void should_add_the_newly_committed_entry_to_the_simulation_set()
      {
         _simulationExistingSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").ShouldNotBeNull();
      }
   }

   public class When_committing_to_an_existing_set_with_an_unchecked_tracked_parameter : concern_for_CommitSimulationParametersTask
   {
      private OverwriteParameterSet _simulationExistingSet;

      protected override void Context()
      {
         base.Context();
         //Permeability is tracked AND in the existing set with the same stored value, but the user unchecks it in
         //the commit dialog (it is not in ParameterPaths). The entry should be preserved as-is rather than removed.
         _simulationExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId1" };
         _simulationExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 7.2 });
         _simulationCompound.AddOverwriteParameterSet(_simulationExistingSet);

         var templateExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId2" };
         templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 7.2 });
         _templateCompound.AddOverwriteParameterSet(templateExistingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity" },
            OverwriteParameterSetName = "ExistingSet",
            ShouldCreateNew = false
         });
      }

      [Observation]
      public void should_preserve_the_unchecked_tracked_entry()
      {
         _simulationExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldNotBeNull();
      }

      [Observation]
      public void should_keep_the_unchecked_path_tracked()
      {
         _simulation.ParameterChangeTracker.IsTracked("Organism|Aspirin|Permeability").ShouldBeTrue();
      }
   }

   public class When_committing_and_a_parameter_cannot_be_resolved : concern_for_CommitSimulationParametersTask
   {
      private ICommand _result;

      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|NonExistent");
      }

      protected override void Because()
      {
         _result = sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|NonExistent" },
            OverwriteParameterSetName = "Set",
            ShouldCreateNew = true
         });
      }

      [Observation]
      public void should_not_untrack_the_unresolved_path()
      {
         _simulation.ParameterChangeTracker.IsTracked("Organism|Aspirin|NonExistent").ShouldBeTrue();
      }
   }
}