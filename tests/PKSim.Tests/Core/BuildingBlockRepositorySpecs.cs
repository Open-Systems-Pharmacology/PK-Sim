using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockRepository : ContextSpecification<IBuildingBlockRepository>
   {
      private IPKSimProjectRetriever _projectRetriever;
      private PKSimProject _project;
      protected Individual _individual1;
      protected Individual _individual2;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         sut = new BuildingBlockRepository(_projectRetriever);

         _project = new PKSimProject();
         A.CallTo(() => _projectRetriever.Current).Returns(_project);
         
         _individual1 = new Individual().WithName("IND1");
         _individual2 = new Individual().WithName("IND2");
         _project.AddBuildingBlock(_individual1);
         _project.AddBuildingBlock(_individual2);
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