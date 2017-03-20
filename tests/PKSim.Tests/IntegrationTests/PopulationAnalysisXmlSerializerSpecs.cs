using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_PopulationAnalysisXmlSerializer : ContextForSerialization<PopulationAnalysis>
   {
      protected IDimensionRepository _dimensionRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _dimensionRepository = IoC.Resolve<IDimensionRepository>();
      }
   }

   public class When_serializing_a_population_analysis : concern_for_PopulationAnalysisXmlSerializer
   {
      private PopulationAnalysis _populationAnalysis;
      private PopulationAnalysis _deserialized;
      private PopulationAnalysisPKParameterField _pkParameterField;
      private PopulationAnalysisParameterField _parameterField;
      private PopulationAnalysisGroupingField _numberOfBinsDerivedField;
      private PopulationAnalysisGroupingField _fixedLimitsGroupingDefinitionField;
      private PopulationAnalysisExpressionField _derivedFieldWithFixedExpression;
      private PopulationAnalysisCovariateField _genderField;
      private PopulationAnalysisOutputField _outputField;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _populationAnalysis = new PopulationPivotAnalysis();

         _pkParameterField = new PopulationAnalysisPKParameterField().WithName("PKParam");
         _pkParameterField.PKParameter = "AUC";
         _pkParameterField.QuantityPath = "A|B|C";
         _pkParameterField.QuantityType = QuantityType.Drug;
         _pkParameterField.Dimension = _dimensionRepository.MassConcentration;

         _parameterField = new PopulationAnalysisParameterField().WithName("Param");
         _parameterField.ParameterPath = "A|B|C|D";
         _parameterField.Dimension = _dimensionRepository.Auc;

         _outputField = new PopulationAnalysisOutputField().WithName("Output");
         _outputField.QuantityPath = "A|B|C";
         _outputField.Dimension = _dimensionRepository.Auc;
         _outputField.Color = Color.Aquamarine;

         _genderField = new PopulationAnalysisCovariateField {Covariate = "Gender"}.WithName("Gender");
         var numberOfBinsGroupingDefinition = new NumberOfBinsGroupingDefinition(_parameterField.Name) {NumberOfBins = 2};
         numberOfBinsGroupingDefinition.NamingPattern = "AA";
         numberOfBinsGroupingDefinition.StartColor = Color.AliceBlue;
         numberOfBinsGroupingDefinition.EndColor = Color.RoyalBlue;
         numberOfBinsGroupingDefinition.Strategy = LabelGenerationStrategies.Roman;
         numberOfBinsGroupingDefinition.AddItems(PopulationAnalysisHelperForSpecs.BMIGroups);
         _numberOfBinsDerivedField = new PopulationAnalysisGroupingField(numberOfBinsGroupingDefinition).WithName("NumberOfBins");

         var fixedLimitsGroupingDefinition = new FixedLimitsGroupingDefinition(_pkParameterField.Name);
         fixedLimitsGroupingDefinition.AddItems(PopulationAnalysisHelperForSpecs.AgeGroups);

         _fixedLimitsGroupingDefinitionField = new PopulationAnalysisGroupingField(fixedLimitsGroupingDefinition)
            .WithName("FixedLimits");

         fixedLimitsGroupingDefinition.SetLimits(new[] {1d, 2d}.OrderBy(x => x));

         _derivedFieldWithFixedExpression = new PopulationAnalysisExpressionField("A+B")
            .WithName("FixedExpression");


         var valueMappingGroupingDefinition = new ValueMappingGroupingDefinition(_genderField.Name);
         valueMappingGroupingDefinition.AddValueLabel("Male", PopulationAnalysisHelperForSpecs.GroupingItemMale);
         valueMappingGroupingDefinition.AddValueLabel("Female", PopulationAnalysisHelperForSpecs.GroupingItemFemale);
         var valueMappingGroupingDefinitionField = new PopulationAnalysisGroupingField(valueMappingGroupingDefinition).WithName("ValueMapping");

         _populationAnalysis.Add(_pkParameterField);
         _populationAnalysis.Add(_parameterField);
         _populationAnalysis.Add(_genderField);
         _populationAnalysis.Add(_numberOfBinsDerivedField);
         _populationAnalysis.Add(_fixedLimitsGroupingDefinitionField);
         _populationAnalysis.Add(_derivedFieldWithFixedExpression);
         _populationAnalysis.Add(valueMappingGroupingDefinitionField);
         _populationAnalysis.Add(_outputField);
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_populationAnalysis);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_population_analyses()
      {
         _deserialized.ShouldNotBeNull();
         _deserialized.AllFields.Count.ShouldBeEqualTo(_populationAnalysis.AllFields.Count);
      }

      [Observation]
      public void should_be_able_to_deserialize_a_population_pk_parameter_field()
      {
         var field = _deserialized.FieldByName(_pkParameterField.Name).DowncastTo<PopulationAnalysisPKParameterField>();
         field.Name.ShouldBeEqualTo(_pkParameterField.Name);
         field.PKParameter.ShouldBeEqualTo(_pkParameterField.PKParameter);
         field.QuantityPath.ShouldBeEqualTo(_pkParameterField.QuantityPath);
         field.QuantityType.ShouldBeEqualTo(_pkParameterField.QuantityType);
         field.Dimension.ShouldBeEqualTo(_pkParameterField.Dimension);
         field.DisplayUnit.ShouldBeEqualTo(_pkParameterField.DisplayUnit);
      }

      [Observation]
      public void should_be_able_to_deserialize_a_population_parameter_field()
      {
         var field = _deserialized.FieldByName(_parameterField.Name).DowncastTo<PopulationAnalysisParameterField>();
         field.Name.ShouldBeEqualTo(_parameterField.Name);
         field.ParameterPath.ShouldBeEqualTo(_parameterField.ParameterPath);
         field.Dimension.ShouldBeEqualTo(_parameterField.Dimension);
         field.DisplayUnit.ShouldBeEqualTo(_parameterField.DisplayUnit);
      }

      [Observation]
      public void should_be_able_to_deserialize_an_output_parameter_field()
      {
         var field = _deserialized.FieldByName(_outputField.Name).DowncastTo<PopulationAnalysisOutputField>();
         field.Name.ShouldBeEqualTo(_outputField.Name);
         field.QuantityPath.ShouldBeEqualTo(_outputField.QuantityPath);
         field.Dimension.ShouldBeEqualTo(_outputField.Dimension);
         field.DisplayUnit.ShouldBeEqualTo(_outputField.DisplayUnit);
         field.Color.ShouldBeEqualTo(_outputField.Color);
      }

      [Observation]
      public void should_be_able_to_deserialize_a_covariate_field()
      {
         var field = _deserialized.FieldByName(_genderField.Name).DowncastTo<PopulationAnalysisCovariateField>();
         field.Name.ShouldBeEqualTo(_genderField.Name);
         field.Covariate.ShouldBeEqualTo(_genderField.Covariate);
         field.GroupingItems.Count.ShouldBeEqualTo(_genderField.GroupingItems.Count);
         for (int i = 0; i < field.GroupingItems.Count; i++)
         {
            field.GroupingItems[i].Color.ShouldBeEqualTo(_genderField.GroupingItems[i].Color);
            field.GroupingItems[i].Label.ShouldBeEqualTo(_genderField.GroupingItems[i].Label);
            field.GroupingItems[i].Symbol.ShouldBeEqualTo(_genderField.GroupingItems[i].Symbol);
         }
      }

      [Observation]
      public void should_be_able_to_deserialize_a_derived_parameter_with_a_number_of_bins_grouping_definition()
      {
         var field = _deserialized.FieldByName(_numberOfBinsDerivedField.Name).DowncastTo<PopulationAnalysisGroupingField>();
         field.Name.ShouldBeEqualTo(_numberOfBinsDerivedField.Name);
         field.GroupingDefinition.FieldName.ShouldBeEqualTo(_parameterField.Name);

         var numberOfBinsGrouping = _numberOfBinsDerivedField.GroupingDefinition.DowncastTo<NumberOfBinsGroupingDefinition>();
         var deserializednumberOfBinsGrouping = field.GroupingDefinition.DowncastTo<NumberOfBinsGroupingDefinition>();
         numberOfBinsGrouping.Labels.ShouldBeEqualTo(deserializednumberOfBinsGrouping.Labels);
         numberOfBinsGrouping.StartColor.ShouldBeEqualTo(deserializednumberOfBinsGrouping.StartColor);
         numberOfBinsGrouping.EndColor.ShouldBeEqualTo(deserializednumberOfBinsGrouping.EndColor);
         numberOfBinsGrouping.NamingPattern.ShouldBeEqualTo(deserializednumberOfBinsGrouping.NamingPattern);
         numberOfBinsGrouping.Strategy.ShouldBeEqualTo(deserializednumberOfBinsGrouping.Strategy);
         numberOfBinsGrouping.NumberOfBins.ShouldBeEqualTo(deserializednumberOfBinsGrouping.NumberOfBins);
         numberOfBinsGrouping.Items.Select(x => x.Color).ShouldBeEqualTo(deserializednumberOfBinsGrouping.Items.Select(x => x.Color));
         numberOfBinsGrouping.Items.Select(x => x.Symbol).ShouldBeEqualTo(deserializednumberOfBinsGrouping.Items.Select(x => x.Symbol));
      }

      [Observation]
      public void should_be_able_to_deserialize_a_derived_parameter_with_a_fixed_limits_grouping_definition()
      {
         var field = _deserialized.FieldByName(_fixedLimitsGroupingDefinitionField.Name).DowncastTo<PopulationAnalysisGroupingField>();
         field.Name.ShouldBeEqualTo(_fixedLimitsGroupingDefinitionField.Name);
         field.GroupingDefinition.FieldName.ShouldBeEqualTo(_pkParameterField.Name);

         var fixedLimitsGroupingDefinition = _fixedLimitsGroupingDefinitionField.GroupingDefinition.DowncastTo<FixedLimitsGroupingDefinition>();
         var deserializedFixedLimitsGroupingDefinition = field.GroupingDefinition.DowncastTo<FixedLimitsGroupingDefinition>();
         fixedLimitsGroupingDefinition.Labels.ShouldBeEqualTo(deserializedFixedLimitsGroupingDefinition.Labels);
         fixedLimitsGroupingDefinition.Limits.ShouldBeEqualTo(deserializedFixedLimitsGroupingDefinition.Limits);
      }

      [Observation]
      public void should_be_able_to_deserialize_a_derived_parameter_with_a_fixed_expression()
      {
         var field = _deserialized.FieldByName(_derivedFieldWithFixedExpression.Name).DowncastTo<PopulationAnalysisExpressionField>();
         field.Name.ShouldBeEqualTo(_derivedFieldWithFixedExpression.Name);
         field.Expression.ShouldBeEqualTo(_derivedFieldWithFixedExpression.Expression);
      }
   }
}