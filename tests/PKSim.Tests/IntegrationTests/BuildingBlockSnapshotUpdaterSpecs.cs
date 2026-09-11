using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_BuildingBlockSnapshotUpdater : ContextForIntegration<IBuildingBlockSnapshotUpdater>
   {
      protected IJsonSerializer _jsonSerializer;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _jsonSerializer = IoC.Resolve<IJsonSerializer>();
      }
   }

   public class When_adding_a_snapshot_to_an_individual_building_block : concern_for_BuildingBlockSnapshotUpdater
   {
      private Individual _individual;
      private IndividualBuildingBlock _individualBuildingBlock;
      private SnapshotIndividual _snapshot;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(_individual);
         _individualBuildingBlock = IoC.Resolve<IIndividualToIndividualBuildingBlockMapper>().MapFrom(_individual);
      }

      protected override void Because()
      {
         sut.AddSnapshotTo(_individualBuildingBlock, _individual);
         _snapshot = _jsonSerializer.DeserializeFromBase64String<SnapshotIndividual>(_individualBuildingBlock.Snapshot).Result;
      }

      [Observation]
      public void the_snapshot_should_describe_the_individual()
      {
         _snapshot.Name.ShouldBeEqualTo(_individual.Name);
         _snapshot.OriginData.ShouldNotBeNull();
      }

      [Observation]
      public void the_snapshot_should_not_reference_the_expression_profiles_of_the_individual()
      {
         _snapshot.ExpressionProfiles.ShouldBeNull();
      }

      [Observation]
      public void the_snapshot_should_recreate_the_individual_building_block()
      {
         var recreated = IoC.Resolve<IIndividualSnapshotToIndividualBuildingBlockMapper>().MapFrom(_individualBuildingBlock.Snapshot);
         recreated.Name.ShouldBeEqualTo(_individualBuildingBlock.Name);
         recreated.Count().ShouldBeEqualTo(_individualBuildingBlock.Count());
      }
   }

   public class When_adding_a_snapshot_to_an_expression_profile_building_block : concern_for_BuildingBlockSnapshotUpdater
   {
      private ExpressionProfile _expressionProfile;
      private ExpressionProfileBuildingBlock _expressionProfileBuildingBlock;
      private ExpressionProfileBuildingBlock _recreated;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _expressionProfileBuildingBlock = IoC.Resolve<IExpressionProfileToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfile);
      }

      protected override void Because()
      {
         sut.AddSnapshotTo(_expressionProfileBuildingBlock, _expressionProfile);
         _recreated = IoC.Resolve<IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfileBuildingBlock.Snapshot);
      }

      [Observation]
      public void the_snapshot_should_recreate_the_expression_profile_building_block()
      {
         _recreated.Name.ShouldBeEqualTo(_expressionProfileBuildingBlock.Name);
         _recreated.MoleculeName.ShouldBeEqualTo(_expressionProfileBuildingBlock.MoleculeName);
         _recreated.Species.ShouldBeEqualTo(_expressionProfileBuildingBlock.Species);
         _recreated.Type.ShouldBeEqualTo(_expressionProfileBuildingBlock.Type);
      }
   }

   public class When_updating_the_snapshot_of_an_expression_profile_building_block_from_a_database_query : concern_for_BuildingBlockSnapshotUpdater
   {
      private ExpressionProfileBuildingBlock _expressionProfileBuildingBlock;
      private ExpressionProfileBuildingBlock _recreated;
      private ExpressionProfile _expressionProfile;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _expressionProfileBuildingBlock = IoC.Resolve<IExpressionProfileToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfile);
         sut.AddSnapshotTo(_expressionProfileBuildingBlock, _expressionProfile);
      }

      protected override void Because()
      {
         var queryResults = new QueryExpressionResults(new[]
         {
            new ExpressionResult {ContainerName = CoreConstants.Compartment.PERIPORTAL, RelativeExpression = 1},
            new ExpressionResult {ContainerName = CoreConstants.Organ.KIDNEY, RelativeExpression = 0.5},
         });

         sut.UpdateSnapshotFromQuery(_expressionProfileBuildingBlock, queryResults);
         _recreated = IoC.Resolve<IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfileBuildingBlock.Snapshot);
      }

      [Observation]
      public void the_snapshot_should_contain_the_queried_relative_expressions()
      {
         relativeExpressionFor(CoreConstants.Compartment.PERIPORTAL).ShouldBeEqualTo(1);
         relativeExpressionFor(CoreConstants.Organ.KIDNEY).ShouldBeEqualTo(0.5);
      }

      private double relativeExpressionFor(string containerName) =>
         _recreated.ExpressionParameters.Single(x => x.HasExpressionName() && x.ContainerNameForRelativeExpressionParameter() == containerName).Value.Value;
   }

   public class When_updating_the_snapshot_of_an_expression_profile_building_block_without_snapshot : concern_for_BuildingBlockSnapshotUpdater
   {
      private ExpressionProfileBuildingBlock _expressionProfileBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _expressionProfileBuildingBlock = new ExpressionProfileBuildingBlock();
      }

      protected override void Because()
      {
         sut.UpdateSnapshotFromQuery(_expressionProfileBuildingBlock, new QueryExpressionResults(new ExpressionResult[] { }));
      }

      [Observation]
      public void the_building_block_should_still_have_no_snapshot()
      {
         _expressionProfileBuildingBlock.HasSnapshot.ShouldBeFalse();
      }
   }
}
