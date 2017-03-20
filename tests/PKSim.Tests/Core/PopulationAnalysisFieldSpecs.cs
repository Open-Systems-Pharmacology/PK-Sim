using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisField : ContextSpecification<IPopulationAnalysisField>
   {
      protected PopulationAnalysis _populationAnalysis;
      protected IPopulationAnalysisField _anotherField;

      protected override void Context()
      {
         _populationAnalysis = new PopulationPivotAnalysis();
         _anotherField = new PopulationAnalysisParameterField().WithName("NAME");
         sut = new PopulationAnalysisParameterField();
         _populationAnalysis.Add(_anotherField);
      }
   }

   public class When_validating_a_population_analysis_field : concern_for_PopulationAnalysisField
   {
      [Observation]
      public void should_be_invalid_if_the_name_is_not_defined()
      {
         sut.Name = string.Empty;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_valid_if_the_name_is_defined_and_the_field_does_not_belong_to_any_analysis()
      {
         sut.Name = "TOTO";
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_be_valid_if_the_name_is_defined_and_unique_in_the_parent_analysis()
      {
         sut.Name = "TOTO";
         _populationAnalysis.Add(sut);
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_be_invalid_if_the_name_is_not_unique_in_the_parent_analysis()
      {
         sut.Name = _anotherField.Name;
         _populationAnalysis.Add(sut);
         sut.IsValid().ShouldBeFalse();
      }
   }
}