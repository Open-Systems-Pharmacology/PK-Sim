using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_PKSimProjectRetriever : ContextSpecification<IPKSimProjectRetriever>
   {
      protected ICoreWorkspace _workspace;

      protected override void Context()
      {
         _workspace= A.Fake<ICoreWorkspace>();  
         sut = new PKSimProjectRetriever(_workspace);
      }
   }

   public class When_retrieving_the_project_name_when_the_project_is_not_defined : concern_for_PKSimProjectRetriever
   {
      protected override void Because()
      {
         _workspace.Project = null;
      }

      [Observation]
      public void should_return_an_empty_string()
      {
         sut.ProjectName.ShouldBeEqualTo(string.Empty);
      }
   }
}	