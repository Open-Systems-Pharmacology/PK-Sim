using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
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

         _expectedResults = "iif([Field] < 14.5, \'Children\', iif([Field] < 18.5, \'Adolescents\', \'Adults\'))";
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
         sut.CanBeUsedFor(typeof(double)).ShouldBeTrue();
      }

      [Observation]
      public void cannot_be_used_for_non_numeric_types()
      {
         sut.CanBeUsedFor(typeof(string)).ShouldBeFalse();
      }
   }

   public class When_updating_a_fixed_limit_grouping_definition_from_another_grouping_definition : concern_for_FixedLimitsGroupingDefinition
   {
      private FixedLimitsGroupingDefinition _sourceFixedLimitDefinition;
      private ICloneManager _cloneManager;

      protected override void Context()
      {
         base.Context();
         _cloneManager= A.Fake<ICloneManager>(); 
         _sourceFixedLimitDefinition = new FixedLimitsGroupingDefinition("Field");
         _sourceFixedLimitDefinition.SetLimits(new []{1.5, 2.5}.OrderBy(x=>x));
         _sourceFixedLimitDefinition.Dimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _sourceFixedLimitDefinition.DisplayUnit = _sourceFixedLimitDefinition.Dimension.Unit("h");
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_sourceFixedLimitDefinition, _cloneManager);
      }

      [Observation]
      public void should_update_all_propeties_from_the_source_object()
      {
         sut.Dimension.ShouldBeEqualTo(_sourceFixedLimitDefinition.Dimension);
         sut.DisplayUnit.ShouldBeEqualTo(_sourceFixedLimitDefinition.DisplayUnit);
         sut.Limits.ShouldOnlyContainInOrder(_sourceFixedLimitDefinition.Limits);
      }
   }
}