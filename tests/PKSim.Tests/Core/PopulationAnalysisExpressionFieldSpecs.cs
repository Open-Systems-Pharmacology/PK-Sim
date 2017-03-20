using System.Collections;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisExpressionField : ContextSpecification<PopulationAnalysisExpressionField>
   {
      protected const string _testExpression = "[Field1] + [Field2]";
      protected string[] _labels = new[] { "Children", "Adolescents", "Adults" };

      protected override void Context()
      {
         sut = new PopulationAnalysisExpressionField(_testExpression);
      }
   }

   public class When_creating_a_population_analysis_expression_field : concern_for_PopulationAnalysisExpressionField
   {
      [Observation]
      public void the_expression_should_be_set()
      {
         sut = new PopulationAnalysisExpressionField(_testExpression);
         sut.Expression.ShouldBeEqualTo(_testExpression);
      }
   }

   public class When_checking_if_an_expression_field_is_a_derived_type_for_a_given_type : concern_for_PopulationAnalysisExpressionField
   {
      [Observation]
      public void should_always_return_false()
      {
         sut.IsDerivedTypeFor<PopulationAnalysisParameterField>().ShouldBeFalse();
         sut.IsDerivedTypeFor<PopulationAnalysisPKParameterField>().ShouldBeFalse();
      }
   }

   public class When_sorting_expression_fields : concern_for_PopulationAnalysisExpressionField
   {
   
      [Observation]
      public void should_sort_values_by_values_if_value_mapping_is_used()
      {
         sut.Compare(_labels[0], _labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(_labels[0], _labels[1]));
         sut.Compare(_labels[0], _labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(_labels[0], _labels[2]));
         sut.Compare(_labels[1], _labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(_labels[1], _labels[0]));
         sut.Compare(_labels[1], _labels[2]).ShouldBeEqualTo(Comparer.Default.Compare(_labels[1], _labels[2]));
         sut.Compare(_labels[2], _labels[0]).ShouldBeEqualTo(Comparer.Default.Compare(_labels[2], _labels[0]));
         sut.Compare(_labels[2], _labels[1]).ShouldBeEqualTo(Comparer.Default.Compare(_labels[2], _labels[1]));
      }
   }
}