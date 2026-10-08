using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_changing_a_simulation_parameter_overwritten_by_the_applied_overwrite_parameter_set : ContextForSimulationIntegration<IExecutionContext>
   {
      private Compound _compound;
      private string _parameterPath;
      private IParameter _overwrittenParameter;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IExecutionContext>();

         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(templateIndividual, _compound, protocol).DowncastTo<IndividualSimulation>();
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var compoundParameter = firstValidCompoundParameter();
         _parameterPath = compoundParameter.Key;

         var overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet" };
         overwriteParameterSet.Add(new ParameterValue { Path = _parameterPath.ToObjectPath(), Value = compoundParameter.Value.Value + 1.0 });
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, overwriteParameterSet);

         //rebuild the model so the selected overwrite parameter set is applied during construction
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         _overwrittenParameter = parameterCache()[_parameterPath];
      }

      private KeyValuePair<string, IParameter> firstValidCompoundParameter()
      {
         return parameterCache().KeyValues
            .First(kv => kv.Value.BuildingBlockType == PKSimBuildingBlockType.Simulation &&
                         _simulation.CompoundNameForParameterPath(kv.Key) == _compound.Name &&
                         !double.IsNaN(kv.Value.Value));
      }

      private PathCache<IParameter> parameterCache() => IoC.Resolve<IContainerTask>().CacheAllChildren<IParameter>(_simulation.Model.Root);

      protected override void Because()
      {
         new SetParameterValueCommand(_overwrittenParameter, _overwrittenParameter.Value + 1.0).Run(sut);
      }

      [Observation]
      public void should_track_the_change_as_an_uncommitted_change_of_the_compound()
      {
         _simulation.ParameterChangeTracker.IsTracked(_parameterPath).ShouldBeTrue();
      }

      [Observation]
      public void should_not_flag_the_compound_used_in_the_simulation_as_altered()
      {
         _simulation.UsedBuildingBlockByTemplateId(_compound.Id).Altered.ShouldBeFalse();
      }
   }

   public class When_applying_an_overwrite_parameter_set_supplying_a_parameter_changed_in_the_simulation : ContextForSimulationIntegration<IExecutionContext>
   {
      private Compound _compound;
      private string _parameterPath;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IExecutionContext>();

         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(templateIndividual, _compound, protocol).DowncastTo<IndividualSimulation>();
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var compoundParameter = firstValidCompoundParameter();
         _parameterPath = compoundParameter.Key;

         //the user changes the parameter in the simulation without committing it
         new SetParameterValueCommand(compoundParameter.Value, compoundParameter.Value.Value + 1.0).Run(sut);

         var overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet" };
         overwriteParameterSet.Add(new ParameterValue { Path = _parameterPath.ToObjectPath(), Value = compoundParameter.Value.Value });
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, overwriteParameterSet);
      }

      private KeyValuePair<string, IParameter> firstValidCompoundParameter()
      {
         return parameterCache().KeyValues
            .First(kv => kv.Value.BuildingBlockType == PKSimBuildingBlockType.Simulation &&
                         _simulation.CompoundNameForParameterPath(kv.Key) == _compound.Name &&
                         !double.IsNaN(kv.Value.Value));
      }

      private PathCache<IParameter> parameterCache() => IoC.Resolve<IContainerTask>().CacheAllChildren<IParameter>(_simulation.Model.Root);

      protected override void Because()
      {
         //configuring the simulation rebuilds the model and applies the selected overwrite parameter set
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_not_report_the_change_as_uncommitted_anymore()
      {
         _simulation.ParameterChangeTracker.IsTracked(_parameterPath).ShouldBeFalse();
         _simulation.HasUncommittedChanges.ShouldBeFalse();
      }
   }

   public abstract class concern_for_committing_simulation_parameters_to_a_new_overwrite_parameter_set : ContextForSimulationIntegration<IExecutionContext>
   {
      protected Compound _compound;
      protected Compound _simulationCompound;
      protected UsedBuildingBlock _usedCompound;
      protected ICommitSimulationParametersTask _commitTask;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected string _changedPath;
      protected string _resetPath;
      protected string _untouchedPath;
      protected double _changedValue;
      protected double _calculatedValueOfResetParameter;
      protected double _untouchedValue;
      private ICoreWorkspace _workspace;
      private PKSimProject _oldProject;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IExecutionContext>();
         _commitTask = IoC.Resolve<ICommitSimulationParametersTask>();
         _buildingBlockInProjectManager = IoC.Resolve<IBuildingBlockInProjectManager>();

         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(templateIndividual, _compound, protocol).DowncastTo<IndividualSimulation>();
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var compoundParameters = compoundDependentParameters().Take(3).ToList();
         _changedPath = compoundParameters[0].Key;
         _resetPath = compoundParameters[1].Key;
         _untouchedPath = compoundParameters[2].Key;

         var selectedSet = new OverwriteParameterSet { Name = "Selected" };
         compoundParameters.Each(x => selectedSet.Add(new ParameterValue { Path = x.Key.ToObjectPath(), Value = x.Value.Value + 1.0 }));
         _compound.AddOverwriteParameterSet(selectedSet);

         _usedCompound = _simulation.UsedBuildingBlockByTemplateId(_compound.Id);
         _simulationCompound = _usedCompound.BuildingBlock.DowncastTo<Compound>();
         var selectedSetInSimulation = IoC.Resolve<ICloner>().Clone(selectedSet);
         _simulationCompound.AddOverwriteParameterSet(selectedSetInSimulation);
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, selectedSetInSimulation);

         //rebuild the model so the selected overwrite parameter set is applied during construction
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var project = new PKSimProject();
         project.AddBuildingBlock(templateIndividual);
         project.AddBuildingBlock(_compound);
         project.AddBuildingBlock(protocol);
         project.AddBuildingBlock(_simulation);
         _workspace = IoC.Resolve<ICoreWorkspace>();
         _oldProject = _workspace.Project;
         _workspace.Project = project;

         var changedParameter = parameterCache()[_changedPath];
         new SetParameterValueCommand(changedParameter, changedParameter.Value + 2.0).Run(sut);
         new ResetParameterCommand(parameterCache()[_resetPath]).Run(sut);

         _changedValue = valueOf(_changedPath);
         _calculatedValueOfResetParameter = valueOf(_resetPath);
         _untouchedValue = valueOf(_untouchedPath);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         _workspace.Project = _oldProject;
      }

      protected void commitToNewSet(params string[] parameterPaths) => _commitTask.CommitParametersToCompound(_simulation, new CompoundCommitInfo
      {
         TemplateCompoundId = _compound.Id,
         ParameterPaths = parameterPaths,
         ParameterPathsToRemove = new[] { _resetPath },
         OverwriteParameterSetName = "New",
         ShouldCreateNew = true
      });

      protected void commitEverythingToNewSet() => commitToNewSet(_changedPath, _untouchedPath);

      protected double valueOf(string path) => parameterCache()[path].Value;

      protected void valuesShouldBeUnchanged()
      {
         valueOf(_changedPath).ShouldBeEqualTo(_changedValue);
         valueOf(_resetPath).ShouldBeEqualTo(_calculatedValueOfResetParameter);
         valueOf(_untouchedPath).ShouldBeEqualTo(_untouchedValue);
      }

      private IEnumerable<KeyValuePair<string, IParameter>> compoundDependentParameters() => parameterCache().KeyValues
         .Where(kv => kv.Value.BuildingBlockType == PKSimBuildingBlockType.Simulation &&
                      _simulation.CompoundNameForParameterPath(kv.Key) == _compound.Name &&
                      !double.IsNaN(kv.Value.Value));

      private PathCache<IParameter> parameterCache() => IoC.Resolve<IContainerTask>().CacheAllChildren<IParameter>(_simulation.Model.Root);
   }

   public class When_committing_simulation_parameters_to_a_new_overwrite_parameter_set : concern_for_committing_simulation_parameters_to_a_new_overwrite_parameter_set
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         commitEverythingToNewSet();
      }

      [Observation]
      public void should_add_the_new_set_to_the_project_compound()
      {
         _compound.OverwriteParameterSets.FindByName("New").ParameterValues.Select(x => x.Path.PathAsString).ShouldOnlyContain(_changedPath, _untouchedPath);
      }

      [Observation]
      public void should_add_a_copy_of_the_new_set_to_the_compound_used_in_the_simulation()
      {
         var setInSimulation = _simulationCompound.OverwriteParameterSets.FindByName("New");
         setInSimulation.ShouldNotBeNull();
         setInSimulation.ShouldNotBeEqualTo(_compound.OverwriteParameterSets.FindByName("New"));
         setInSimulation.ParameterValueByPath(_changedPath).Value.ShouldBeEqualTo(_changedValue);
      }

      [Observation]
      public void should_select_the_new_set_of_the_compound_used_in_the_simulation()
      {
         var selectedSet = _simulation.OverwriteParameterSetSelections.SelectedSetFor(_compound.Name);
         selectedSet.ShouldNotBeNull();
         selectedSet.ShouldBeEqualTo(_simulationCompound.OverwriteParameterSets.FindByName("New"));
      }

      [Observation]
      public void should_not_report_uncommitted_changes_anymore()
      {
         _simulation.HasUncommittedChanges.ShouldBeFalse();
      }

      [Observation]
      public void should_keep_the_compound_used_in_the_simulation_in_sync_with_the_project_compound()
      {
         _buildingBlockInProjectManager.StatusFor(_usedCompound).ShouldBeEqualTo(BuildingBlockStatus.Green);
      }

      [Observation]
      public void should_not_change_the_values_of_the_simulation()
      {
         valuesShouldBeUnchanged();
      }
   }

   public class When_committing_only_some_simulation_parameters_to_a_new_overwrite_parameter_set : concern_for_committing_simulation_parameters_to_a_new_overwrite_parameter_set
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         commitToNewSet(_changedPath);
      }

      [Observation]
      public void should_track_the_untouched_entry_left_out_of_the_new_set()
      {
         _simulation.ParameterChangeTracker.ChangedPaths.Select(x => x.PathAsString).ShouldOnlyContain(_untouchedPath);
      }

      [Observation]
      public void should_still_select_the_new_set()
      {
         _simulation.OverwriteParameterSetSelections.SelectedSetFor(_compound.Name).ShouldBeEqualTo(_simulationCompound.OverwriteParameterSets.FindByName("New"));
      }

      [Observation]
      public void should_keep_the_compound_used_in_the_simulation_in_sync_with_the_project_compound()
      {
         _buildingBlockInProjectManager.StatusFor(_usedCompound).ShouldBeEqualTo(BuildingBlockStatus.Green);
      }
   }

   public class When_rebuilding_a_simulation_after_committing_its_parameters_to_a_new_overwrite_parameter_set : concern_for_committing_simulation_parameters_to_a_new_overwrite_parameter_set
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         commitEverythingToNewSet();
      }

      protected override void Because()
      {
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_reproduce_the_values_the_simulation_had_before_the_rebuild()
      {
         valuesShouldBeUnchanged();
      }
   }
}
