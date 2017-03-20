using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_population_pivot_analysis : ContextForSerialization<PopulationPivotAnalysis>
   {
      private PopulationPivotAnalysis _populationPivotAnalysis;
      private PopulationPivotAnalysis _deserialized;
      private PopulationAnalysisPKParameterField _pkParameterField;
      private PopulationAnalysisParameterField _parameterField;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _populationPivotAnalysis = new PopulationPivotAnalysis();
         _pkParameterField = new PopulationAnalysisPKParameterField().WithName("PKParam");
         _pkParameterField.PKParameter = "AUC";
         _pkParameterField.QuantityPath = "A|B|C";

         _parameterField = new PopulationAnalysisParameterField().WithName("Param");
         _parameterField.ParameterPath = "A|B|C|D";
         _populationPivotAnalysis.Add(_pkParameterField);
         _populationPivotAnalysis.Add(_parameterField);
         _populationPivotAnalysis.SetPosition(_parameterField, PivotArea.DataArea, 1);
         _populationPivotAnalysis.SetPosition(_pkParameterField, PivotArea.RowArea, 2);
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_populationPivotAnalysis);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_area_where_the_fields_were_defined()
      {
         _deserialized.ShouldNotBeNull();
         _deserialized.AllFields.Count.ShouldBeEqualTo(_populationPivotAnalysis.AllFields.Count);
         _deserialized.GetArea(_parameterField.Name).ShouldBeEqualTo(PivotArea.DataArea);
         _deserialized.GetAreaIndex(_parameterField.Name).ShouldBeEqualTo(1);
         _deserialized.GetArea(_pkParameterField.Name).ShouldBeEqualTo(PivotArea.RowArea);
         _deserialized.GetAreaIndex(_pkParameterField.Name).ShouldBeEqualTo(2);
      }
   }
}