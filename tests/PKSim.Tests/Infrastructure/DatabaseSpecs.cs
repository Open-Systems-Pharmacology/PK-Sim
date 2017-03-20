using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ORM.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_Database : ContextSpecification<IDatabase>
   {
      protected override void Context()
      {
         sut = new TemplateDatabase();
      }
   }

   public class When_connecting_to_a_database_that_does_not_exist : concern_for_Database
   {
      [Observation]
      public void should_throw_an_sbsuite_exception()
      {
         The.Action(() => sut.Connect("A file that does not exist")).ShouldThrowAn<OSPSuiteException>();
      }
   }
}