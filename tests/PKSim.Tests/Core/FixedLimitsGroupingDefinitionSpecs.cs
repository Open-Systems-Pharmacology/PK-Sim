using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core
{
   public abstract class concern_for_FixedLimitsGroupingDefinition : ContextSpecification<FixedLimitsGroupingDefinition>
   {
      protected string _expectedResults;

      protected override void Context()
      {
         sut = new FixedLimitsGroupingDefinition("Field");


         sut.SetLimits((new[] {14.5, 18.5}).OrderBy(x => x));
         sut.AddItems(PopulationAnalysisHelperForSpecs.AgeGroups);

         _expectedResults = string.Format("iif([Field] < 14.5, 'Children', iif([Field] < 18.5, 'Adolescents', 'Adults'))");
      }
   }

   public class When_creating_a_FixedLimitsGroupingDefinition : concern_for_FixedLimitsGroupingDefinition
   {
      [Observation]
      public void the_expression_should_be_generated()
      {
         sut.GetExpression().ShouldBeEqualTo(_expectedResults);
      }

      [Observation]
      public void can_be_used_for_numeric_types()
      {
         sut.CanBeUsedFor(typeof (double)).ShouldBeTrue();
      }

      [Observation]
      public void cannot_be_used_for_non_numeric_types()
      {
         sut.CanBeUsedFor(typeof (string)).ShouldBeFalse();
      }
   }

 
}