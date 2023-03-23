using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationSimulationAnalysisSynchronizer : ContextSpecification<IPopulationSimulationAnalysisSynchronizer>
   {
      protected override void Context()
      {
         sut = new PopulationSimulationAnalysisSynchronizer();
      }
   }

   public class When_synchronizing_the_population_analysis_defined_in_a_population_simulation : concern_for_PopulationSimulationAnalysisSynchronizer
   {
      private PopulationSimulation _populationSimulation;
      private TimeProfileAnalysisChart _timeProfileAnalysis;
      private PopulationAnalysisOutputField _outputField1;
      private PopulationAnalysisOutputField _outputField2;
      private PopulationAnalysisPKParameterField _pkField1;
      private PopulationAnalysisPKParameterField _pkField2;
      private PopulationAnalysisDerivedField _derivedField1;
      private PopulationAnalysisDerivedField _derivedField2;
      private PopulationAnalysisDerivedField _pkDerivedField1;
      private PopulationAnalysisDerivedField _pkDerivedField2;
      private PopulationStatisticalAnalysis _populationStatisticalAnalysis;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = new PopulationSimulation { Settings = new SimulationSettings {OutputSelections = new OutputSelections()}};

         var liverCellsDrugConc = "Liver|Cells|Drug|Conc";
         _populationSimulation.OutputSelections.AddOutput(new QuantitySelection(liverCellsDrugConc, QuantityType.Drug));
         _populationSimulation.OutputSelections.AddOutput(new QuantitySelection("Liver|Plasma|Drug|Conc", QuantityType.Drug));

         _populationStatisticalAnalysis = new PopulationStatisticalAnalysis();
         _timeProfileAnalysis = new TimeProfileAnalysisChart {PopulationAnalysis = _populationStatisticalAnalysis};

         _outputField1 = new PopulationAnalysisOutputField {QuantityPath = "Kidney|Cell|Drug|Conc", Name = "outputField1"};
         _derivedField1 = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition(_outputField1.Name)) {Name = "derivedField1"};

         _outputField2 = new PopulationAnalysisOutputField {QuantityPath = liverCellsDrugConc, Name = "outputField2"};
         _derivedField2 = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition(_outputField2.Name)) {Name = "derivedField2"};

         _pkField1 = new PopulationAnalysisPKParameterField {QuantityPath = "Kidney|Cell|Drug|Conc", Name = "pkField1"};
         _pkDerivedField1 = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition(_pkField1.Name)) {Name = "pkDerivedField1"};

         _pkField2 = new PopulationAnalysisPKParameterField {QuantityPath = liverCellsDrugConc, Name = "pkField2"};
         _pkDerivedField2 = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition(_pkField2.Name)) {Name = "pkDerivedField2"};

         _populationStatisticalAnalysis.Add(_outputField1);
         _populationStatisticalAnalysis.Add(_outputField2);
         _populationStatisticalAnalysis.Add(_derivedField1);
         _populationStatisticalAnalysis.Add(_derivedField2);
         _populationStatisticalAnalysis.Add(_pkField1);
         _populationStatisticalAnalysis.Add(_pkField2);
         _populationStatisticalAnalysis.Add(_pkDerivedField1);
         _populationStatisticalAnalysis.Add(_pkDerivedField2);
         _populationSimulation.AddAnalysis(_timeProfileAnalysis);
      }

      protected override void Because()
      {
         sut.UpdateAnalysesDefinedIn(_populationSimulation);
      }

      [Observation]
      public void should_remove_all_output_field_and_derived_field_that_are_referencing_an_output_that_does_not_exist_anymore()
      {
         _populationStatisticalAnalysis.AllFields.ShouldNotContain(_outputField1, _derivedField1);
      }

      [Observation]
      public void should_remove_all_pk_field_and_derived_field_that_are_referencing_an_output_that_does_not_exist_anymore()
      {
         _populationStatisticalAnalysis.AllFields.ShouldNotContain(_pkField1, _pkDerivedField1);
      }

      [Observation]
      public void should_keep_all_other_fields()
      {
         _populationStatisticalAnalysis.AllFields.ShouldContain(_outputField2, _derivedField2, _pkField2, _pkDerivedField2);
      }
   }
}