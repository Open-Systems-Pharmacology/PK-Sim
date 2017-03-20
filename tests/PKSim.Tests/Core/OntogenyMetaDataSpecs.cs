using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_OntogenyMetaData : ContextSpecification<OntogenyMetaData>
   {
      protected override void Context()
      {
         sut = new OntogenyMetaData();
      }
   }

   public class When_calculating_a_random_ontogeny_factor : concern_for_OntogenyMetaData
   {
      private RandomGenerator _randomGenerator;

      protected override void Context()
      {
         base.Context();
         sut.OntogenyFactor = 1;
         sut.Deviation = 1.1;
         _randomGenerator = new RandomGenerator();
      }

      [Observation]
      public void should_use_the_existing_mean_and_deviation_and_return_a_distributed_value()
      {
         Enumerable.Range(0, 100).Each(i =>
            {
               sut.RandomizedFactor(_randomGenerator).ShouldBeGreaterThan(0);
               sut.RandomizedFactor(_randomGenerator).ShouldBeSmallerThan(2);
            });
      }

      [Observation]
      public void should_return_the_ontogeny_factor_if_the_deviation_is_1()
      {
         sut.Deviation = 1;
         sut.RandomizedFactor(_randomGenerator).ShouldBeEqualTo(sut.OntogenyFactor);
      }
   }
}