using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockRepository : ContextSpecification<IBuildingBlockRepository>
   {
      private IProjectRetriever _projectRetriever;
      private PKSimProject _project;
      protected Individual _individual1;
      protected Individual _individual2;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IProjectRetriever>();
         _project = A.Fake<PKSimProject>();
         _individual1 = new Individual();
         _individual2 = new Individual();
         A.CallTo(() => _projectRetriever.CurrentProject).Returns(_project);
         A.CallTo(() => _project.All<Individual>()).Returns(new[] {_individual1, _individual2});
         sut = new BuildingBlockRepository(_projectRetriever);
      }
   }

   public class When_the_building_block_repository_is_returning_all_the_building_block_for_a_given_type : concern_for_BuildingBlockRepository
   {
      [Observation]
      public void should_return_the_building_block_defined_for_that_type()
      {
         sut.All<Individual>().ShouldOnlyContain(_individual1, _individual2);
      }
   }
}