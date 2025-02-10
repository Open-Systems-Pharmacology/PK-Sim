using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;


namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockFinalizer : ContextSpecification<IBuildingBlockFinalizer>
   {
      protected IReferencesResolver _referenceResolver;
      protected IKeywordReplacerTask _keywordReplacerTask;
      protected INeighborhoodFinalizer _neighborhoodFinalizer;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected IIndividualPathWithRootExpander _individualPathWithRootExpander;
      protected IFormulaTask _formulaTask;

      protected override void Context()
      {
         _referenceResolver = A.Fake<IReferencesResolver>();
         _keywordReplacerTask = A.Fake<IKeywordReplacerTask>();
         _neighborhoodFinalizer = A.Fake<INeighborhoodFinalizer>();
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _individualPathWithRootExpander = A.Fake<IIndividualPathWithRootExpander>();
         _formulaTask= A.Fake<IFormulaTask>();
         sut = new BuildingBlockFinalizer(_referenceResolver, _keywordReplacerTask, _neighborhoodFinalizer, _buildingBlockInProjectManager, _individualPathWithRootExpander, _formulaTask);
      }
   }

   public class When_finalizing_an_building_block_that_is_not_an_individual : concern_for_BuildingBlockFinalizer
   {
      private IPKSimBuildingBlock _buildingBlock;

      protected override void Context()
      {
         base.Context();
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
      }

      protected override void Because()
      {
         sut.Finalize(_buildingBlock);
      }

      [Observation]
      public void should_not_resolve_the_references_in_the_building_block()
      {
         A.CallTo(() => _referenceResolver.ResolveReferencesIn(_buildingBlock)).MustNotHaveHappened();
      }
   }

   public class When_finalizing_an_building_block_that_is_an_individual : concern_for_BuildingBlockFinalizer
   {
      private Individual _individual;
      private Neighborhood _neighborhood1;
      private Neighborhood _neighborhood2;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual();
         var neighborhoods = A.Fake<IContainer>().WithName(Constants.NEIGHBORHOODS);
         _neighborhood1 = A.Fake<Neighborhood>();
         _neighborhood2 = A.Fake<Neighborhood>();
         A.CallTo(() => neighborhoods.GetChildren<Neighborhood>()).Returns(new[] {_neighborhood1, _neighborhood2});
         _individual.Add(neighborhoods);
      }

      protected override void Because()
      {
         sut.Finalize(_individual);
      }

      [Observation]
      public void should_resolve_the_references_in_the_building_block()
      {
         A.CallTo(() => _referenceResolver.ResolveReferencesIn(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_replace_the_keyword_used_in_the_individuals()
      {
         A.CallTo(() => _keywordReplacerTask.ReplaceInSpatialStructure(_individual, A<ReplacementContext>._)).MustHaveHappened();
      }

      [Observation]
      public void should_expand_all_dynamic_formulas_defined_in_the_individual()
      {
         A.CallTo(() => _formulaTask.ExpandDynamicFormulaIn(_individual)).MustHaveHappened();
      }
   }

   public class When_finalizing_an_building_block_that_is_a_simulation : concern_for_BuildingBlockFinalizer
   {
      private Simulation _simulation;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulation.Individual).Returns(_individual);
         _simulation.Model = A.Fake<IModel>();
      }

      protected override void Because()
      {
         sut.Finalize(_simulation);
      }

      [Observation]
      public void should_resolve_the_references_in_the_model()
      {
         A.CallTo(() => _referenceResolver.ResolveReferencesIn(_simulation.Model)).MustHaveHappened();
      }

      [Observation]
      public void should_finalize_the_individual_in_this_simulation()
      {
         A.CallTo(() => _neighborhoodFinalizer.SetNeighborsIn(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_name_of_used_building_block_in_the_simulation()
      {
         A.CallTo(() => _buildingBlockInProjectManager.UpdateBuildingBlockNamesUsedIn(_simulation)).MustHaveHappened();
      }
   }
}