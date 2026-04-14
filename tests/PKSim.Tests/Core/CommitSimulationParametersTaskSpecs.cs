using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_CommitSimulationParametersTask : ContextSpecification<CommitSimulationParametersTask>
   {
      protected IExecutionContext _executionContext;
      protected IContainerTask _containerTask;
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

         _parameterCache = new PathCacheForSpecs<IParameter>();
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", _lipophilicityParam);
         _parameterCache.Add("Organism|Aspirin|Permeability", _permeabilityParam);

         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);
         A.CallTo(() => _executionContext.TypeFor(_simulationCompound)).Returns("Compound");

         sut = new CommitSimulationParametersTask(_executionContext, _containerTask);
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
            SimulationCompound = _simulationCompound,
            TemplateCompound = _templateCompound,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity", "Organism|Aspirin|Permeability" },
            NewOverwriteParameterSetName = "MyNewSet"
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
            SimulationCompound = _simulationCompound,
            TemplateCompound = _templateCompound,
            ParameterPaths = new[] { "Organism|Aspirin|Lipophilicity" },
            ExistingSimulationOverwriteParameterSet = _simulationExistingSet,
            ExistingTemplateOverwriteParameterSet = _templateExistingSet
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
            SimulationCompound = _simulationCompound,
            TemplateCompound = _templateCompound,
            ParameterPaths = new[] { "Organism|Aspirin|NonExistent" },
            NewOverwriteParameterSetName = "Set"
         });
      }

      [Observation]
      public void should_not_untrack_the_unresolved_path()
      {
         _simulation.ParameterChangeTracker.IsTracked("Organism|Aspirin|NonExistent").ShouldBeTrue();
      }
   }
}