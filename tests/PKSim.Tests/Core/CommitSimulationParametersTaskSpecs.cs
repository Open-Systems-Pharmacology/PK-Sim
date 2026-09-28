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
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_CommitSimulationParametersTask : ContextSpecification<CommitSimulationParametersTask>
   {
      protected IExecutionContext _executionContext;
      protected IContainerTask _containerTask;
      protected PKSim.Core.Repositories.IBuildingBlockRepository _buildingBlockRepository;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected ICloneManager _cloneManager;
      protected IndividualSimulation _simulation;
      protected Compound _simulationCompound;
      protected Compound _templateCompound;
      protected UsedBuildingBlock _usedCompound;
      protected IParameter _lipophilicityParam;
      protected IParameter _permeabilityParam;
      protected PathCache<IParameter> _parameterCache;
      protected IObjectBaseFactory _objectBaseFactory;
      private int _createdSetCount;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _containerTask = A.Fake<IContainerTask>();
         _buildingBlockRepository = A.Fake<PKSim.Core.Repositories.IBuildingBlockRepository>();
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _cloneManager = A.Fake<ICloneManager>();
         A.CallTo(() => _objectBaseFactory.Create<OverwriteParameterSet>()).ReturnsLazily(() => new OverwriteParameterSet { Id = $"NewSetId{_createdSetCount++}" });
         A.CallTo(() => _executionContext.CloneManager).Returns(_cloneManager);
         A.CallTo(() => _cloneManager.Clone(A<OverwriteParameterSet>._)).ReturnsLazily(x => cloneOf(x.GetArgument<OverwriteParameterSet>(0)));
         A.CallTo(() => _cloneManager.Clone(A<ParameterValue>._)).ReturnsLazily(x => cloneOf(x.GetArgument<ParameterValue>(0)));

         _simulationCompound = new Compound { Name = "Aspirin", Id = "SimCompId" };
         _templateCompound = new Compound { Name = "Aspirin", Id = "TemplateCompId", Version = 7 };

         _lipophilicityParam = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         _permeabilityParam = DomainHelperForSpecs.ConstantParameterWithValue(7.2).WithName("Permeability");
         _permeabilityParam.DisplayUnit = _permeabilityParam.Dimension.Unit("cm");
         _permeabilityParam.MinValue = 0;
         _permeabilityParam.MinIsAllowed = true;
         _permeabilityParam.ValueOrigin.Id = 42;
         _permeabilityParam.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;
         _permeabilityParam.ValueOrigin.Description = "Fitted to data";

         var root = new Container { Name = "Sim" };
         root.Add(_lipophilicityParam);
         root.Add(_permeabilityParam);

         _simulation = new IndividualSimulation
         {
            Id = "SimId",
            Model = new OSPSuite.Core.Domain.Model { Root = root }
         };

         _usedCompound = new UsedBuildingBlock("TemplateCompId", PKSimBuildingBlockType.Compound) { BuildingBlock = _simulationCompound, Version = 3 };
         _simulation.AddUsedBuildingBlock(_usedCompound);
         A.CallTo(() => _buildingBlockRepository.ById<Compound>("TemplateCompId")).Returns(_templateCompound);
         A.CallTo(() => _buildingBlockInProjectManager.StatusFor(_usedCompound)).Returns(BuildingBlockStatus.Green);

         _parameterCache = new PathCacheForSpecs<IParameter>();
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", _lipophilicityParam);
         _parameterCache.Add("Organism|Aspirin|Permeability", _permeabilityParam);

         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);
         A.CallTo(() => _executionContext.TypeFor(_templateCompound)).Returns("Compound");

         sut = new CommitSimulationParametersTask(_executionContext, _containerTask, _buildingBlockRepository, _objectBaseFactory, _buildingBlockInProjectManager);
      }

      private OverwriteParameterSet cloneOf(OverwriteParameterSet overwriteParameterSet)
      {
         var clone = new OverwriteParameterSet { Id = $"CloneOf{overwriteParameterSet.Id}" };
         clone.UpdatePropertiesFrom(overwriteParameterSet, _cloneManager);
         return clone;
      }

      private static ParameterValue cloneOf(ParameterValue parameterValue) =>
         new() { Path = parameterValue.Path, Value = parameterValue.Value, Dimension = parameterValue.Dimension, DisplayUnit = parameterValue.DisplayUnit };

      protected OverwriteParameterSet setInSimulationCompound(string name) => _simulationCompound.OverwriteParameterSets.FindByName(name);

      protected OverwriteParameterSet setInTemplateCompound(string name) => _templateCompound.OverwriteParameterSets.FindByName(name);

      protected OverwriteParameterSet selectedSet => _simulation.OverwriteParameterSetSelections.SelectedSetFor(_templateCompound.Name);
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
      public void should_add_the_overwrite_parameter_set_to_the_template_compound()
      {
         _templateCompound.OverwriteParameterSets.Count.ShouldBeEqualTo(1);
         _templateCompound.OverwriteParameterSets[0].Name.ShouldBeEqualTo("MyNewSet");
      }

      [Observation]
      public void should_add_a_copy_of_the_overwrite_parameter_set_to_the_compound_used_in_the_simulation()
      {
         var setInSimulation = setInSimulationCompound("MyNewSet");
         setInSimulation.ShouldNotBeNull();
         setInSimulation.ShouldNotBeEqualTo(setInTemplateCompound("MyNewSet"));
         setInSimulation.ParameterValueByPath("Organism|Aspirin|Permeability").Value.ShouldBeEqualTo(7.2);
      }

      [Observation]
      public void should_keep_the_compound_used_in_the_simulation_in_sync_with_the_template_compound()
      {
         _usedCompound.Version.ShouldBeEqualTo(_templateCompound.Version);
      }

      [Observation]
      public void should_create_the_overwrite_parameter_set_with_its_own_id()
      {
         string.IsNullOrEmpty(_templateCompound.OverwriteParameterSets[0].Id).ShouldBeFalse();
      }

      [Observation]
      public void should_set_building_block_properties_on_the_command()
      {
         A.CallTo(() => _executionContext.UpdateBuildingBlockPropertiesInCommand(A<IOSPSuiteCommand>._, _templateCompound)).MustHaveHappened();
      }

      [Observation]
      public void should_publish_a_simulation_status_changed_event_for_the_simulation()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationStatusChangedEvent>.That.Matches(e => e.Simulation == _simulation))).MustHaveHappened();
      }

      [Observation]
      public void should_only_show_the_change_to_the_template_compound_in_the_history()
      {
         var visibleSubCommands = _result.DowncastTo<IPKSimMacroCommand>().All().Where(x => x.Visible).ToList();
         visibleSubCommands.Count.ShouldBeEqualTo(1);
         visibleSubCommands[0].DowncastTo<IBuildingBlockChangeCommand>().BuildingBlockId.ShouldBeEqualTo(_templateCompound.Id);
      }

      [Observation]
      public void should_describe_the_commit_on_the_macro_command()
      {
         _result.Description.ShouldBeEqualTo(PKSimConstants.Command.CommitSimulationParametersToCompound("MyNewSet", _templateCompound.Name));
         _result.CommandType.ShouldBeEqualTo(PKSimConstants.Command.CommandTypeAdd);
      }

      [Observation]
      public void should_take_over_dimension_display_unit_and_allowed_range_of_the_committed_simulation_parameter()
      {
         var parameterValue = _templateCompound.OverwriteParameterSets[0].ParameterValueByPath("Organism|Aspirin|Permeability");
         parameterValue.Dimension.ShouldBeEqualTo(_permeabilityParam.Dimension);
         parameterValue.DisplayUnit.ShouldBeEqualTo(_permeabilityParam.DisplayUnit);
         parameterValue.Info.MinValue.ShouldBeEqualTo(_permeabilityParam.MinValue);
         parameterValue.Info.MinIsAllowed.ShouldBeEqualTo(_permeabilityParam.MinIsAllowed);
      }

      [Observation]
      public void should_take_over_the_value_origin_of_the_committed_simulation_parameter()
      {
         var valueOrigin = _templateCompound.OverwriteParameterSets[0].ParameterValueByPath("Organism|Aspirin|Permeability").ValueOrigin;
         valueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.ParameterIdentification);
         valueOrigin.Description.ShouldBeEqualTo("Fitted to data");
         //the id keeps the link to the value origin database entry, so it is not written to the snapshot again
         valueOrigin.Id.ShouldBeEqualTo(42);
      }
   }

   public class When_committing_parameters_to_a_new_set_while_the_compound_used_in_the_simulation_is_out_of_sync : concern_for_CommitSimulationParametersTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _buildingBlockInProjectManager.StatusFor(_usedCompound)).Returns(BuildingBlockStatus.Red);
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
      }

      protected override void Because()
      {
         sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity" },
            OverwriteParameterSetName = "MyNewSet",
            ShouldCreateNew = true
         });
      }

      [Observation]
      public void should_leave_the_compound_used_in_the_simulation_out_of_sync()
      {
         _usedCompound.Version.ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_still_add_the_set_to_the_compound_used_in_the_simulation()
      {
         setInSimulationCompound("MyNewSet").ShouldNotBeNull();
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
         _simulation.AddOverwriteParameterSetSelection("Aspirin", _simulationExistingSet);

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
      public void should_update_the_template_existing_set_with_new_parameter_value()
      {
         _templateExistingSet.ParameterValues.Any(pv => pv.Path.PathAsString == "Organism|Aspirin|Lipophilicity").ShouldBeTrue();
      }

      [Observation]
      public void should_update_the_set_in_the_compound_used_in_the_simulation_as_well()
      {
         _simulationExistingSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").Value.ShouldBeEqualTo(3.5);
      }

      [Observation]
      public void should_keep_the_selection_on_the_set_of_the_compound_used_in_the_simulation()
      {
         selectedSet.ShouldBeEqualTo(_simulationExistingSet);
      }

      [Observation]
      public void should_keep_the_compound_used_in_the_simulation_in_sync_with_the_template_compound()
      {
         _usedCompound.Version.ShouldBeEqualTo(_templateCompound.Version);
      }

      [Observation]
      public void should_preserve_existing_entries_for_parameters_no_longer_present_in_the_simulation()
      {
         _templateExistingSet.ParameterValues.Any(pv => pv.Path.PathAsString == "Organism|Aspirin|OldParam").ShouldBeTrue();
      }

      [Observation]
      public void should_only_show_the_change_to_the_template_compound_in_the_history()
      {
         var visibleSubCommands = _result.DowncastTo<IPKSimMacroCommand>().All().Where(x => x.Visible).ToList();
         visibleSubCommands.Count.ShouldBeEqualTo(1);
         visibleSubCommands[0].DowncastTo<IBuildingBlockChangeCommand>().BuildingBlockId.ShouldBeEqualTo(_templateCompound.Id);
      }
   }

   public class When_committing_a_tracked_parameter_to_an_existing_set_with_other_untouched_entries : concern_for_CommitSimulationParametersTask
   {
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         //Permeability is in the existing set with the same value the simulation parameter currently holds (7.2),
         //modelling a parameter that was previously committed and has not been touched since.
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
      public void should_preserve_the_existing_untouched_entry_in_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldNotBeNull();
      }

      [Observation]
      public void should_add_the_newly_committed_entry_to_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").ShouldNotBeNull();
      }
   }

   public class When_committing_the_removal_of_a_reset_parameter_together_with_a_changed_parameter_to_an_existing_set : concern_for_CommitSimulationParametersTask
   {
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         _templateExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId2" };
         _templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 99.0 });
         _templateCompound.AddOverwriteParameterSet(_templateExistingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity" },
            ParameterPathsToRemove = new[] { "Organism|Aspirin|Permeability" },
            OverwriteParameterSetName = "ExistingSet",
            ShouldCreateNew = false
         });
      }

      [Observation]
      public void should_remove_the_reset_entry_from_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldBeNull();
      }

      [Observation]
      public void should_add_the_newly_committed_entry_to_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").ShouldNotBeNull();
      }

      [Observation]
      public void should_clear_the_removed_path_from_the_tracker()
      {
         _simulation.ParameterChangeTracker.HasUncommittedChanges.ShouldBeFalse();
      }
   }

   public class When_committing_only_the_removal_of_a_reset_parameter_to_an_existing_set : concern_for_CommitSimulationParametersTask
   {
      private ICommand _result;
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         _templateExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId2" };
         _templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 99.0 });
         _templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Lipophilicity".ToObjectPath(), Value = 2.0 });
         _templateCompound.AddOverwriteParameterSet(_templateExistingSet);

         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Permeability");
      }

      protected override void Because()
      {
         _result = sut.CommitParametersToCompound(_simulation, new CompoundCommitInfo
         {
            TemplateCompoundId = _templateCompound.Id,
            ParameterPaths = new string[0],
            ParameterPathsToRemove = new[] { "Organism|Aspirin|Permeability" },
            OverwriteParameterSetName = "ExistingSet",
            ShouldCreateNew = false
         });
      }

      [Observation]
      public void should_remove_the_reset_entry_from_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldBeNull();
      }

      [Observation]
      public void should_preserve_the_other_entries_of_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Lipophilicity").ShouldNotBeNull();
      }

      [Observation]
      public void should_clear_the_removed_path_from_the_tracker()
      {
         _simulation.ParameterChangeTracker.HasUncommittedChanges.ShouldBeFalse();
      }

      [Observation]
      public void should_describe_the_commit_as_an_update_of_the_set()
      {
         _result.CommandType.ShouldBeEqualTo(PKSimConstants.Command.CommandTypeUpdate);
         _result.Description.ShouldBeEqualTo(PKSimConstants.Command.CommitSimulationParametersToCompound("ExistingSet", _templateCompound.Name));
      }
   }

   public class When_committing_to_an_existing_set_holding_an_untracked_entry_whose_value_differs_from_the_simulation : concern_for_CommitSimulationParametersTask
   {
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         //Permeability is in the existing set with a value that differs from the simulation parameter but is not
         //tracked: the set was edited in the compound after being applied. The commit must not infer a reset from that.
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
      public void should_preserve_the_untracked_entry_in_the_template_set()
      {
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").Value.ShouldBeEqualTo(99.0);
      }
   }

   public class When_committing_to_an_existing_set_with_an_unchecked_tracked_parameter : concern_for_CommitSimulationParametersTask
   {
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         //Permeability is tracked AND in the existing set with the same stored value, but the user unchecks it in
         //the commit dialog (it is not in ParameterPaths). The entry should be preserved as-is rather than removed.
         _templateExistingSet = new OverwriteParameterSet { Name = "ExistingSet", Id = "SetId2" };
         _templateExistingSet.Add(new ParameterValue { Path = "Organism|Aspirin|Permeability".ToObjectPath(), Value = 7.2 });
         _templateCompound.AddOverwriteParameterSet(_templateExistingSet);

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
         _templateExistingSet.ParameterValueByPath("Organism|Aspirin|Permeability").ShouldNotBeNull();
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
