using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ProjectMetaDataToProjectMapper : ContextSpecification<IProjectMetaDataToProjectMapper>
   {
      private ISimulationMetaDataToSimulationMapper _simulationMapper;
      protected ICompressedSerializationManager _entitySerializer;
      private ISerializationContextFactory _serializationContextFactory;
      protected IMissingBuildingBlockContentRegistry _missingBuildingBlockContentRegistry;
      protected PKSimProject _project;
      protected ProjectMetaData _projectMetaData;
      protected IndividualSimulationComparisonMetaData _simulationComparisonMetaData;

      protected override void Context()
      {
         _simulationMapper = A.Fake<ISimulationMetaDataToSimulationMapper>();
         _entitySerializer = A.Fake<ICompressedSerializationManager>();
         _serializationContextFactory = A.Fake<ISerializationContextFactory>();
         _missingBuildingBlockContentRegistry = A.Fake<IMissingBuildingBlockContentRegistry>();
         sut = new ProjectMetaDataToProjectMapper(_simulationMapper, _entitySerializer, _serializationContextFactory, _missingBuildingBlockContentRegistry);

         _projectMetaData = new ProjectMetaData();
         _simulationComparisonMetaData = new IndividualSimulationComparisonMetaData {Id = "ComparisonData"};
      }
   }

   public class When_mapping_a_project_meta_data_to_project_with_version_current : concern_for_ProjectMetaDataToProjectMapper
   {
      protected override void Context()
      {
         base.Context();
         //this will trigger an update of the hasChanged flag to true
         _projectMetaData.AddSimulationComparison(_simulationComparisonMetaData);
         _projectMetaData.Version = ProjectVersions.Current;
      }

      protected override void Because()
      {
         _project = sut.MapFrom(_projectMetaData);
      }

      [Observation]
      public void should_have_set_the_has_changed_flag_to_false()
      {
         _project.HasChanged.ShouldBeFalse();
      }
   }

   public class When_mapping_a_project_meta_data_to_project_with_an_older_version : concern_for_ProjectMetaDataToProjectMapper
   {
      protected override void Context()
      {
         base.Context();
         _projectMetaData.AddSimulationComparison(_simulationComparisonMetaData);
         _projectMetaData.Version = ProjectVersions.UNSUPPORTED;
      }

      protected override void Because()
      {
         _project = sut.MapFrom(_projectMetaData);
      }

      [Observation]
      public void should_not_have_reset_the_changed_flag()
      {
         _project.HasChanged.ShouldBeTrue();
      }
   }

   public class When_mapping_a_project_meta_data_containing_building_blocks_whose_content_is_missing : concern_for_ProjectMetaDataToProjectMapper
   {
      private CompoundMetaData _compoundWithContent;
      private CompoundMetaData _compoundWithoutContent;
      private ExpressionProfileMetaData _expressionProfileWithoutContent;
      private IndividualMetaData _individualUsingExpressionProfile;
      private IndividualMetaData _individualNotUsingExpressionProfile;
      private RandomPopulationMetaData _populationUsingExpressionProfile;
      private BuildingBlockWithMissingContent _registeredCompound;
      private BuildingBlockWithMissingContent _registeredExpressionProfile;

      protected override void Context()
      {
         base.Context();
         _compoundWithContent = new CompoundMetaData {Id = "C1", Name = "Compound with content", Content = {Data = new byte[] {1, 2, 3}}};
         _compoundWithoutContent = new CompoundMetaData {Id = "C2", Name = "Compound without content", Description = "Desc", Icon = "Icon", Version = 3, StructureVersion = 2};
         _expressionProfileWithoutContent = new ExpressionProfileMetaData {Id = "EP", Name = "CYP3A4|Human|Healthy"};
         _individualUsingExpressionProfile = new IndividualMetaData {Id = "IND1", Name = "Ind1", ExpressionProfileIds = "OTHER|EP"};
         _individualNotUsingExpressionProfile = new IndividualMetaData {Id = "IND2", Name = "Ind2", ExpressionProfileIds = "OTHER"};
         _populationUsingExpressionProfile = new RandomPopulationMetaData {Id = "POP", Name = "Pop", ExpressionProfileIds = "EP", Properties = {Data = new byte[] {1}}};

         _projectMetaData.AddBuildingBlock(_compoundWithContent);
         _projectMetaData.AddBuildingBlock(_compoundWithoutContent);
         _projectMetaData.AddBuildingBlock(_expressionProfileWithoutContent);
         _projectMetaData.AddBuildingBlock(_individualUsingExpressionProfile);
         _projectMetaData.AddBuildingBlock(_individualNotUsingExpressionProfile);
         _projectMetaData.AddBuildingBlock(_populationUsingExpressionProfile);

         A.CallTo(() => _missingBuildingBlockContentRegistry.Add(A<BuildingBlockWithMissingContent>._))
            .Invokes(x =>
            {
               var buildingBlockWithMissingContent = x.GetArgument<BuildingBlockWithMissingContent>(0);
               if (buildingBlockWithMissingContent.Id == _compoundWithoutContent.Id)
                  _registeredCompound = buildingBlockWithMissingContent;
               else if (buildingBlockWithMissingContent.Id == _expressionProfileWithoutContent.Id)
                  _registeredExpressionProfile = buildingBlockWithMissingContent;
            });
      }

      protected override void Because()
      {
         _project = sut.MapFrom(_projectMetaData);
      }

      [Observation]
      public void should_clear_the_building_blocks_registered_from_a_previous_load_before_mapping()
      {
         A.CallTo(() => _missingBuildingBlockContentRegistry.Clear()).MustHaveHappened()
            .Then(A.CallTo(() => _missingBuildingBlockContentRegistry.Add(A<BuildingBlockWithMissingContent>._)).MustHaveHappened());
      }

      [Observation]
      public void should_deserialize_the_building_blocks_with_content()
      {
         A.CallTo(() => _entitySerializer.Deserialize(A<IPKSimBuildingBlock>.That.Matches(x => x.Id == _compoundWithContent.Id), _compoundWithContent.Content.Data, null)).MustHaveHappened();
         _project.BuildingBlockById<Compound>(_compoundWithContent.Id).ShouldNotBeNull();
      }

      [Observation]
      public void should_not_try_to_deserialize_the_building_blocks_without_content()
      {
         A.CallTo(() => _entitySerializer.Deserialize(A<IPKSimBuildingBlock>._, null, null)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_add_the_building_blocks_without_content_to_the_project()
      {
         _project.BuildingBlockById<Compound>(_compoundWithoutContent.Id).ShouldBeNull();
         _project.BuildingBlockById<ExpressionProfile>(_expressionProfileWithoutContent.Id).ShouldBeNull();
      }

      [Observation]
      public void should_still_add_the_lazy_loaded_building_blocks_to_the_project()
      {
         _project.BuildingBlockById<Individual>(_individualUsingExpressionProfile.Id).ShouldNotBeNull();
         _project.BuildingBlockById<Individual>(_individualNotUsingExpressionProfile.Id).ShouldNotBeNull();
         _project.BuildingBlockById<RandomPopulation>(_populationUsingExpressionProfile.Id).ShouldNotBeNull();
      }

      [Observation]
      public void should_register_the_building_blocks_without_content_with_the_properties_available_in_the_project_file()
      {
         _registeredCompound.ShouldNotBeNull();
         _registeredCompound.Name.ShouldBeEqualTo(_compoundWithoutContent.Name);
         _registeredCompound.Description.ShouldBeEqualTo(_compoundWithoutContent.Description);
         _registeredCompound.Version.ShouldBeEqualTo(_compoundWithoutContent.Version);
         _registeredCompound.StructureVersion.ShouldBeEqualTo(_compoundWithoutContent.StructureVersion);
         _registeredCompound.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Compound);
         _registeredCompound.SimulationSubjectIds.ShouldBeEmpty();

         _registeredExpressionProfile.ShouldNotBeNull();
         _registeredExpressionProfile.Name.ShouldBeEqualTo(_expressionProfileWithoutContent.Name);
         _registeredExpressionProfile.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.ExpressionProfile);
      }

      [Observation]
      public void should_register_the_ids_of_the_simulation_subjects_referencing_an_expression_profile_without_content()
      {
         _registeredExpressionProfile.SimulationSubjectIds.ShouldOnlyContain(_individualUsingExpressionProfile.Id, _populationUsingExpressionProfile.Id);
      }

      [Observation]
      public void should_not_register_the_building_blocks_with_content()
      {
         A.CallTo(() => _missingBuildingBlockContentRegistry.Add(A<BuildingBlockWithMissingContent>.That.Matches(x => x.Id == _compoundWithContent.Id))).MustNotHaveHappened();
      }
   }

   public class When_mapping_a_project_meta_data_in_which_every_building_block_has_content : concern_for_ProjectMetaDataToProjectMapper
   {
      protected override void Context()
      {
         base.Context();
         _projectMetaData.AddBuildingBlock(new CompoundMetaData {Id = "C1", Name = "Compound", Content = {Data = new byte[] {1, 2, 3}}});
      }

      protected override void Because()
      {
         _project = sut.MapFrom(_projectMetaData);
      }

      [Observation]
      public void should_not_register_any_building_block_without_content()
      {
         A.CallTo(() => _missingBuildingBlockContentRegistry.Add(A<BuildingBlockWithMissingContent>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_add_all_building_blocks_to_the_project()
      {
         _project.All<Compound>().Count().ShouldBeEqualTo(1);
      }
   }
}
