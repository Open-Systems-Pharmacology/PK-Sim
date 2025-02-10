using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_CompoundCalculationMethodRetriever : ContextSpecification<CompoundCalculationMethodRepository>
   {
      protected ICompoundCalculationMethodCategoryRepository _compoundCalculationMethodCategoryRepository;

      protected override void Context()
      {
         _compoundCalculationMethodCategoryRepository = A.Fake<ICompoundCalculationMethodCategoryRepository>();

         sut = new CompoundCalculationMethodRepository(_compoundCalculationMethodCategoryRepository);
      }
   }

   public class When_retrieving_all_the_calculation_methods : concern_for_CompoundCalculationMethodRetriever
   {
      protected override void Because()
      {
         sut.All();
      }

      [Observation]
      public void results_in_a_call_to_the_repository()
      {
         A.CallTo(() => _compoundCalculationMethodCategoryRepository.All()).MustHaveHappened();
      }
   }
}
