using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Core
{
   public abstract class concern_for_MissingBuildingBlockContentRecoveryTask : ContextSpecification<IMissingBuildingBlockContentRecoveryTask>
   {
      protected IMissingBuildingBlockContentRegistry _registry;
      protected IExpressionProfileFactory _expressionProfileFactory;
      protected IExpressionProfileUpdater _expressionProfileUpdater;
      protected ILazyLoadTask _lazyLoadTask;
      protected IRegistrationTask _registrationTask;
      protected IEventPublisher _eventPublisher;
      protected IObjectTypeResolver _objectTypeResolver;
      protected PKSimProject _project;
      protected Individual _individual;
      protected IndividualEnzyme _enzyme;
      protected List<BuildingBlockWithMissingContent> _buildingBlocksWithMissingContent;
      protected MissingBuildingBlockContentRecoveryResult _result;
      protected const string _moleculeName = "CYP3A4";
      protected const string _speciesName = "Human";

      protected override void Context()
      {
         _registry = A.Fake<IMissingBuildingBlockContentRegistry>();
         _expressionProfileFactory = A.Fake<IExpressionProfileFactory>();
         _expressionProfileUpdater = A.Fake<IExpressionProfileUpdater>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _registrationTask = A.Fake<IRegistrationTask>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         sut = new MissingBuildingBlockContentRecoveryTask(_registry, _expressionProfileFactory, _expressionProfileUpdater, _lazyLoadTask, _registrationTask, _eventPublisher, _objectTypeResolver);

         _project = new PKSimProject();
         _individual = DomainHelperForSpecs.CreateIndividual(_speciesName).WithId("IND").WithName("Standard male");
         _enzyme = new IndividualEnzyme().WithName(_moleculeName);
         _individual.AddMolecule(_enzyme);
         _project.AddBuildingBlock(_individual);

         _buildingBlocksWithMissingContent = new List<BuildingBlockWithMissingContent>();
         A.CallTo(() => _registry.All()).Returns(_buildingBlocksWithMissingContent);

         A.CallTo(() => _expressionProfileFactory.Create(typeof(IndividualEnzyme), _individual.Species, _moleculeName))
            .ReturnsLazily(x => DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(_speciesName, _moleculeName));

         A.CallTo(() => _objectTypeResolver.TypeFor(A<IPKSimBuildingBlock>._)).Returns(PKSimConstants.ObjectTypes.ExpressionProfile);
      }

      protected BuildingBlockWithMissingContent expressionProfileWithMissingContent(string individualName = "Standard male", params string[] simulationSubjectIds)
      {
         return new BuildingBlockWithMissingContent
         {
            Id = "EP",
            Name = Constants.ContainerName.ExpressionProfileName(_moleculeName, _speciesName, individualName),
            Description = "Rebuilt description",
            Version = 4,
            StructureVersion = 2,
            BuildingBlockType = PKSimBuildingBlockType.ExpressionProfile,
            SimulationSubjectIds = simulationSubjectIds
         };
      }

      protected override void Because()
      {
         _result = sut.Recover(_project);
      }
   }

   public class When_recovering_a_project_in_which_no_building_block_content_is_missing : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      [Observation]
      public void should_return_an_empty_result()
      {
         _result.Any.ShouldBeFalse();
         _result.RebuiltBuildingBlocks.ShouldBeEmpty();
         _result.RemovedBuildingBlocks.ShouldBeEmpty();
         _result.Message.ShouldBeEmpty();
      }

      [Observation]
      public void should_not_create_any_expression_profile()
      {
         A.CallTo(() => _expressionProfileFactory.Create(A<System.Type>._, A<Species>._, A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_recovering_an_expression_profile_without_content_referenced_by_an_individual_defining_the_molecule : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      private BuildingBlockWithMissingContent _expressionProfileWithMissingContent;
      private ExpressionProfile _rebuiltExpressionProfile;

      protected override void Context()
      {
         base.Context();
         _expressionProfileWithMissingContent = expressionProfileWithMissingContent(simulationSubjectIds: _individual.Id);
         _buildingBlocksWithMissingContent.Add(_expressionProfileWithMissingContent);
      }

      protected override void Because()
      {
         base.Because();
         _rebuiltExpressionProfile = _project.BuildingBlockById<ExpressionProfile>(_expressionProfileWithMissingContent.Id);
      }

      [Observation]
      public void should_clear_the_registry_so_that_the_building_blocks_are_not_recovered_twice()
      {
         A.CallTo(() => _registry.Clear()).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_individual_before_rebuilding_the_expression_profile()
      {
         A.CallTo(() => _lazyLoadTask.Load(_individual)).MustHaveHappened()
            .Then(A.CallTo(() => _expressionProfileFactory.Create(typeof(IndividualEnzyme), _individual.Species, _moleculeName)).MustHaveHappened());
      }

      [Observation]
      public void should_rebuild_the_expression_profile_for_the_molecule_defined_in_the_individual_and_add_it_to_the_project()
      {
         _rebuiltExpressionProfile.ShouldNotBeNull();
         _rebuiltExpressionProfile.MoleculeName.ShouldBeEqualTo(_moleculeName);
      }

      [Observation]
      public void should_synchronize_the_rebuilt_expression_profile_with_the_individual()
      {
         A.CallTo(() => _expressionProfileUpdater.SynchronizeExpressionProfileWithSimulationSubject(_rebuiltExpressionProfile, _individual)).MustHaveHappened();
      }

      [Observation]
      public void should_restore_the_properties_still_available_in_the_project_file()
      {
         _rebuiltExpressionProfile.Id.ShouldBeEqualTo(_expressionProfileWithMissingContent.Id);
         _rebuiltExpressionProfile.Name.ShouldBeEqualTo(_expressionProfileWithMissingContent.Name);
         _rebuiltExpressionProfile.Description.ShouldBeEqualTo(_expressionProfileWithMissingContent.Description);
         _rebuiltExpressionProfile.Version.ShouldBeEqualTo(_expressionProfileWithMissingContent.Version);
         _rebuiltExpressionProfile.StructureVersion.ShouldBeEqualTo(_expressionProfileWithMissingContent.StructureVersion);
      }

      [Observation]
      public void should_flag_the_rebuilt_expression_profile_as_loaded_and_changed_so_that_it_is_persisted_with_the_next_save()
      {
         _rebuiltExpressionProfile.IsLoaded.ShouldBeTrue();
         _rebuiltExpressionProfile.HasChanged.ShouldBeTrue();
      }

      [Observation]
      public void should_add_the_rebuilt_expression_profile_to_the_individual()
      {
         _individual.Uses(_rebuiltExpressionProfile).ShouldBeTrue();
      }

      [Observation]
      public void should_not_update_the_individual_the_profile_was_rebuilt_from()
      {
         A.CallTo(() => _expressionProfileUpdater.SynchronizeAllSimulationSubjectsWithExpressionProfile(A<ExpressionProfile>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_register_the_rebuilt_expression_profile()
      {
         A.CallTo(() => _registrationTask.Register(_rebuiltExpressionProfile)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_that_the_expression_profile_was_added_to_the_project()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<BuildingBlockAddedEvent>.That.Matches(x => x.BuildingBlock == _rebuiltExpressionProfile && x.Project == _project))).MustHaveHappened();
      }

      [Observation]
      public void should_report_the_rebuilt_expression_profile()
      {
         _result.Any.ShouldBeTrue();
         _result.RebuiltBuildingBlocks.ShouldOnlyContain(_rebuiltExpressionProfile);
         _result.RemovedBuildingBlocks.ShouldBeEmpty();
      }

      [Observation]
      public void should_describe_the_recovery_in_the_message()
      {
         _result.Message.ShouldBeEqualTo(PKSimConstants.Warning.BuildingBlocksWithMissingContentRecovered(
            new[] {$"{PKSimConstants.ObjectTypes.ExpressionProfile} '{_expressionProfileWithMissingContent.Name}'"},
            new List<string>()));
      }
   }

   public class When_recovering_an_expression_profile_without_content_that_was_already_recreated_in_the_project : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      private BuildingBlockWithMissingContent _expressionProfileWithMissingContent;
      private ExpressionProfile _existingExpressionProfile;

      protected override void Context()
      {
         base.Context();
         _expressionProfileWithMissingContent = expressionProfileWithMissingContent(simulationSubjectIds: _individual.Id);
         _buildingBlocksWithMissingContent.Add(_expressionProfileWithMissingContent);

         //the project conversion recreated the expression profile under a new id (the previous one was skipped)
         _existingExpressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(_speciesName, _moleculeName).WithId("NEW_EP");
         _existingExpressionProfile.Name = _expressionProfileWithMissingContent.Name;
         _project.AddBuildingBlock(_existingExpressionProfile);
      }

      [Observation]
      public void should_not_create_another_expression_profile()
      {
         A.CallTo(() => _expressionProfileFactory.Create(A<System.Type>._, A<Species>._, A<string>._)).MustNotHaveHappened();
         _project.All<ExpressionProfile>().ShouldOnlyContain(_existingExpressionProfile);
      }

      [Observation]
      public void should_make_sure_that_the_individual_uses_the_existing_expression_profile()
      {
         _individual.Uses(_existingExpressionProfile).ShouldBeTrue();
      }

      [Observation]
      public void should_synchronize_the_individuals_with_the_existing_expression_profile()
      {
         A.CallTo(() => _expressionProfileUpdater.SynchronizeAllSimulationSubjectsWithExpressionProfile(_existingExpressionProfile)).MustHaveHappened();
      }

      [Observation]
      public void should_report_the_existing_expression_profile_as_rebuilt()
      {
         _result.RebuiltBuildingBlocks.ShouldOnlyContain(_existingExpressionProfile);
         _result.RemovedBuildingBlocks.ShouldBeEmpty();
      }
   }

   public class When_recovering_an_expression_profile_without_content_referenced_by_several_individuals : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      private Individual _otherIndividualWithMolecule;
      private Individual _individualWithoutMolecule;
      private ExpressionProfile _rebuiltExpressionProfile;

      protected override void Context()
      {
         base.Context();
         _otherIndividualWithMolecule = DomainHelperForSpecs.CreateIndividual(_speciesName).WithId("IND2").WithName("Other");
         _otherIndividualWithMolecule.AddMolecule(new IndividualEnzyme().WithName(_moleculeName));
         _individualWithoutMolecule = DomainHelperForSpecs.CreateIndividual(_speciesName).WithId("IND3").WithName("No molecule");
         _project.AddBuildingBlock(_otherIndividualWithMolecule);
         _project.AddBuildingBlock(_individualWithoutMolecule);
         _buildingBlocksWithMissingContent.Add(expressionProfileWithMissingContent(simulationSubjectIds: new[] {_individual.Id, _otherIndividualWithMolecule.Id, _individualWithoutMolecule.Id}));
      }

      protected override void Because()
      {
         base.Because();
         _rebuiltExpressionProfile = _project.BuildingBlockById<ExpressionProfile>("EP");
      }

      [Observation]
      public void should_rebuild_the_expression_profile_only_once()
      {
         A.CallTo(() => _expressionProfileFactory.Create(typeof(IndividualEnzyme), A<Species>._, _moleculeName)).MustHaveHappenedOnceExactly();
         _result.RebuiltBuildingBlocks.ShouldOnlyContain(_rebuiltExpressionProfile);
      }

      [Observation]
      public void should_add_the_rebuilt_expression_profile_to_all_individuals_defining_the_molecule()
      {
         _individual.Uses(_rebuiltExpressionProfile).ShouldBeTrue();
         _otherIndividualWithMolecule.Uses(_rebuiltExpressionProfile).ShouldBeTrue();
         _individualWithoutMolecule.Uses(_rebuiltExpressionProfile).ShouldBeFalse();
      }

      [Observation]
      public void should_synchronize_the_other_individuals_with_the_rebuilt_expression_profile()
      {
         A.CallTo(() => _expressionProfileUpdater.SynchronizeAllSimulationSubjectsWithExpressionProfile(_rebuiltExpressionProfile)).MustHaveHappened();
      }
   }

   public class When_recovering_an_expression_profile_without_content_referenced_only_by_a_population : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      private BuildingBlockWithMissingContent _expressionProfileWithMissingContent;

      protected override void Context()
      {
         base.Context();
         var population = new RandomPopulation {Id = "POP", Name = "Pop"};
         _project.AddBuildingBlock(population);
         _expressionProfileWithMissingContent = expressionProfileWithMissingContent("Pop", population.Id);
         _buildingBlocksWithMissingContent.Add(_expressionProfileWithMissingContent);
      }

      [Observation]
      public void should_not_rebuild_the_expression_profile()
      {
         _project.All<ExpressionProfile>().ShouldBeEmpty();
         A.CallTo(() => _expressionProfileFactory.Create(A<System.Type>._, A<Species>._, A<string>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_report_the_expression_profile_as_removed()
      {
         _result.Any.ShouldBeTrue();
         _result.RebuiltBuildingBlocks.ShouldBeEmpty();
         _result.RemovedBuildingBlocks.ShouldOnlyContain(_expressionProfileWithMissingContent);
      }

      [Observation]
      public void should_describe_the_removal_in_the_message()
      {
         _result.Message.ShouldBeEqualTo(PKSimConstants.Warning.BuildingBlocksWithMissingContentRecovered(
            new List<string>(),
            new[] {$"{PKSimConstants.ObjectTypes.ExpressionProfile} '{_expressionProfileWithMissingContent.Name}'"}));
      }
   }

   public class When_recovering_an_expression_profile_without_content_referenced_by_an_individual_that_does_not_define_the_molecule : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      private BuildingBlockWithMissingContent _expressionProfileWithMissingContent;

      protected override void Context()
      {
         base.Context();
         _individual.RemoveChild(_enzyme);
         _expressionProfileWithMissingContent = expressionProfileWithMissingContent(simulationSubjectIds: _individual.Id);
         _buildingBlocksWithMissingContent.Add(_expressionProfileWithMissingContent);
      }

      [Observation]
      public void should_report_the_expression_profile_as_removed()
      {
         _project.All<ExpressionProfile>().ShouldBeEmpty();
         _result.RemovedBuildingBlocks.ShouldOnlyContain(_expressionProfileWithMissingContent);
      }
   }

   public class When_recovering_an_expression_profile_without_content_that_is_not_referenced_by_any_simulation_subject : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      private BuildingBlockWithMissingContent _expressionProfileWithMissingContent;

      protected override void Context()
      {
         base.Context();
         _expressionProfileWithMissingContent = expressionProfileWithMissingContent();
         _buildingBlocksWithMissingContent.Add(_expressionProfileWithMissingContent);
      }

      [Observation]
      public void should_report_the_expression_profile_as_removed()
      {
         _project.All<ExpressionProfile>().ShouldBeEmpty();
         _result.RemovedBuildingBlocks.ShouldOnlyContain(_expressionProfileWithMissingContent);
      }
   }

   public class When_recovering_building_blocks_without_content_that_are_not_expression_profiles : concern_for_MissingBuildingBlockContentRecoveryTask
   {
      private BuildingBlockWithMissingContent _compoundWithMissingContent;
      private BuildingBlockWithMissingContent _protocolWithMissingContent;

      protected override void Context()
      {
         base.Context();
         _compoundWithMissingContent = new BuildingBlockWithMissingContent {Id = "C", Name = "Midazolam", BuildingBlockType = PKSimBuildingBlockType.Compound};
         _protocolWithMissingContent = new BuildingBlockWithMissingContent {Id = "P", Name = "iv", BuildingBlockType = PKSimBuildingBlockType.Protocol};
         _buildingBlocksWithMissingContent.Add(_compoundWithMissingContent);
         _buildingBlocksWithMissingContent.Add(_protocolWithMissingContent);
      }

      [Observation]
      public void should_report_the_building_blocks_as_removed()
      {
         _result.RebuiltBuildingBlocks.ShouldBeEmpty();
         _result.RemovedBuildingBlocks.ShouldOnlyContainInOrder(_compoundWithMissingContent, _protocolWithMissingContent);
      }

      [Observation]
      public void should_describe_the_removed_building_blocks_with_their_type_in_the_message()
      {
         _result.Message.ShouldBeEqualTo(PKSimConstants.Warning.BuildingBlocksWithMissingContentRecovered(
            new List<string>(),
            new[]
            {
               $"{PKSimConstants.ObjectTypes.Compound} '{_compoundWithMissingContent.Name}'",
               $"{PKSimConstants.ObjectTypes.AdministrationProtocol} '{_protocolWithMissingContent.Name}'"
            }));
      }
   }
}
