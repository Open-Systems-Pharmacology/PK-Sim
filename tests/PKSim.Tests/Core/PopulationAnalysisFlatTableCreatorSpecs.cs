using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisFlatTableCreator : ContextSpecification<IPopulationAnalysisFlatTableCreator>
   {
      protected PopulationSimulationComparison _populationDataCollector;
      protected PopulationAnalysis _populationAnalysis;
      protected DataTable _dataTable;
      protected PopulationAnalysisParameterField _ageField;
      protected List<double> _ageValues;
      protected string[] _ageClassValues;
      protected PopulationAnalysisDerivedField _ageClassField;

      protected override void Context()
      {
         sut = new PopulationAnalysisFlatTableCreator();
         _populationDataCollector = A.Fake<PopulationSimulationComparison>();
         _populationAnalysis = new PopulationPivotAnalysis();

         _ageField = new PopulationAnalysisParameterField {ParameterPath = "Path", Name = "Age"};
         _populationAnalysis.Add(_ageField);

         //add derived field age class
         var groupingDefinition = new FixedLimitsGroupingDefinition("Age");
         groupingDefinition.SetLimits(new[] {14d}.OrderBy(x => x));
         groupingDefinition.AddItems(new[]
         {
            new GroupingItem {Label = "Children", Color = Color.Blue},
            new GroupingItem {Label = "Adults", Color = Color.Red}
         });
         _ageClassField = new PopulationAnalysisGroupingField(groupingDefinition) {Name = "AgeClass"};
         _populationAnalysis.Add(_ageClassField);

         _ageValues = new List<double> {8d, 14d, 21d};
         _ageClassValues = new[] {"Children", "Adults", "Adults"};
      }

      protected override void Because()
      {
         _dataTable = sut.Create(_populationDataCollector, _populationAnalysis);
      }
   }

   public class When_creating_a_data_table_for_a_basic_analysis_containing_two_fields : concern_for_PopulationAnalysisFlatTableCreator
   {
      protected override void Context()
      {
         base.Context();
         _populationDataCollector.ReferenceSimulation = null;
         _populationDataCollector.ReferenceGroupingItem = null;

         A.CallTo(() => _populationDataCollector.NumberOfItems).Returns(_ageValues.Count);
         A.CallTo(() => _populationDataCollector.AllSimulationNames).Returns(Enumerable.Repeat("Sim", _ageValues.Count).ToArray());
         A.CallTo(() => _populationDataCollector.AllValuesFor(_ageField.ParameterPath)).Returns(_ageValues);
      }

      [Observation]
      public void should_return_a_table_with_one_column_for_each_field()
      {
         _dataTable.Columns.Contains(_ageField.Name).ShouldBeTrue();
         _dataTable.Columns.Contains(_ageClassField.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_return_a_table_with_one_column_filled_for_each_field()
      {
         _dataTable.AllValuesInColumn<double>(_ageField.Name).ShouldOnlyContainInOrder(_ageValues.ToArray());
         _dataTable.AllValuesInColumn<string>(_ageClassField.Name).ShouldOnlyContainInOrder(_ageClassValues);
      }
   }

   public class When_creating_a_data_table_for_a_reference_analysis_containing_two_fields : concern_for_PopulationAnalysisFlatTableCreator
   {
      private GroupingItem _referenceGroupingItem;

      protected override void Context()
      {
         base.Context();

         var referenceSimulation = A.Fake<PopulationSimulation>().WithName("Sim");
         _referenceGroupingItem = new GroupingItem {Label = "Reference", Color = Color.Black, Symbol = Symbols.Circle};
         _populationDataCollector.ReferenceSimulation = referenceSimulation;
         _populationDataCollector.ReferenceGroupingItem = _referenceGroupingItem;

         A.CallTo(() => _populationDataCollector.NumberOfItems).Returns(_ageValues.Count);
         A.CallTo(() => _populationDataCollector.AllSimulationNames).Returns(Enumerable.Repeat(referenceSimulation.Name, _ageValues.Count).ToArray());
         A.CallTo(() => _populationDataCollector.AllValuesFor(_ageField.ParameterPath)).Returns(_ageValues);
      }

      [Observation]
      public void should_return_a_table_with_one_column_for_each_field()
      {
         _dataTable.Columns.Contains(_ageField.Name).ShouldBeTrue();
         _dataTable.Columns.Contains(_ageClassField.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_return_a_table_with_one_column_filled_for_each_field()
      {
         _dataTable.AllValuesInColumn<double>(_ageField.Name).ShouldOnlyContainInOrder(_ageValues.ToArray());
         _dataTable.AllValuesInColumn<string>(_ageClassField.Name).ShouldOnlyContainInOrder(
            Enumerable.Repeat(_referenceGroupingItem.Label, _ageValues.Count).ToArray());
      }

      [Observation]
      public void ageclass_field_should_know_reference_grouping_item()
      {
         var field = _ageClassField.DowncastTo<PopulationAnalysisGroupingField>();
         field.ShouldNotBeNull();
         field.GroupingItems.Contains(_referenceGroupingItem).ShouldBeTrue();
         field.GroupingByName(_referenceGroupingItem.Label).ShouldBeEqualTo(_referenceGroupingItem);
      }
   }

   /// <summary>
   ///    This tests a scenario where 2 sims are involved and the first sim is the reference sim.
   /// </summary>
   /// <remarks>
   ///    The expectation is that for the reference sim only reference is calculated and for the other sim the age
   ///    classes.
   /// </remarks>
   public class When_creating_a_data_table_for_a_reference_analysis_containing_two_fields_and_two_sims : concern_for_PopulationAnalysisFlatTableCreator
   {
      private GroupingItem _referenceGroupingItem;

      protected override void Context()
      {
         base.Context();

         //adding a character that should be escaped
         var referenceSimulation = A.Fake<PopulationSimulation>().WithName("Sim's");
         _referenceGroupingItem = new GroupingItem {Label = "Reference", Color = Color.Black, Symbol = Symbols.Circle};
         _populationDataCollector.ReferenceSimulation = referenceSimulation;
         _populationDataCollector.ReferenceGroupingItem = _referenceGroupingItem;


         A.CallTo(() => _populationDataCollector.NumberOfItems).Returns(_ageValues.Count * 2);
         A.CallTo(() => _populationDataCollector.AllSimulationNames).Returns(
            Enumerable.Repeat(referenceSimulation.Name, _ageValues.Count).Concat(
               Enumerable.Repeat("Sim2", _ageValues.Count)).ToArray());
         A.CallTo(() => _populationDataCollector.AllValuesFor(_ageField.ParameterPath)).Returns(_ageValues.Concat(_ageValues).ToArray());
      }

      [Observation]
      public void should_return_a_table_with_one_column_for_each_field()
      {
         _dataTable.Columns.Contains(_ageField.Name).ShouldBeTrue();
         _dataTable.Columns.Contains(_ageClassField.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_return_a_table_with_one_column_filled_for_each_field()
      {
         _dataTable.AllValuesInColumn<double>(_ageField.Name).ShouldOnlyContainInOrder(_ageValues.Concat(_ageValues).ToArray());
         _dataTable.AllValuesInColumn<string>(_ageClassField.Name).ShouldOnlyContainInOrder(
            Enumerable.Repeat(_referenceGroupingItem.Label, _ageValues.Count).Concat(
               _ageClassValues).ToArray());
      }

      [Observation]
      public void ageclass_field_should_know_reference_grouping_item()
      {
         var field = _ageClassField.DowncastTo<PopulationAnalysisGroupingField>();
         field.ShouldNotBeNull();
         field.GroupingItems.Contains(_referenceGroupingItem).ShouldBeTrue();
         field.GroupingByName(_referenceGroupingItem.Label).ShouldBeEqualTo(_referenceGroupingItem);
      }
   }

   /// <summary>
   ///    This tests a scenario where 2 sims are involved and the first sim is the reference sim.
   /// </summary>
   /// <remarks>
   ///    The expectation is that for the reference sim only reference is calculated and for the other sim the age
   ///    classes.
   /// </remarks>
   public class When_creating_a_data_table_for_a_reference_analysis_containing_covariates_and_two_sims : concern_for_PopulationAnalysisFlatTableCreator
   {
      private GroupingItem _referenceGroupingItem;
      private PopulationAnalysisCovariateField _covariateField;
      private string[] _genderValues;

      protected override void Context()
      {
         base.Context();

         var populationFieldFactory = A.Fake<PopulationAnalysisFieldFactory>();
         _covariateField = populationFieldFactory.CreateFor("Gender", _populationDataCollector);
         _populationAnalysis.Add(_covariateField);
         _genderValues = new string[] {"Male", "Female"};
         _covariateField.AddGroupingItem(new GroupingItem {Label = "Male", Color = PKSimColors.Male, Symbol = Symbols.Circle});
         _covariateField.AddGroupingItem(new GroupingItem {Label = "Female", Color = PKSimColors.Female, Symbol = Symbols.Diamond});

         var referenceSimulation = A.Fake<PopulationSimulation>().WithName("Sim");
         _referenceGroupingItem = new GroupingItem {Label = "Reference", Color = Color.Black, Symbol = Symbols.Circle};

         _populationDataCollector.ReferenceSimulation = referenceSimulation;
         _populationDataCollector.ReferenceGroupingItem = _referenceGroupingItem;

         A.CallTo(() => _populationDataCollector.NumberOfItems).Returns(_ageValues.Count * 2);
         A.CallTo(() => _populationDataCollector.AllSimulationNames).Returns(
            Enumerable.Repeat(referenceSimulation.Name, _ageValues.Count).Concat(
               Enumerable.Repeat("Sim2", _ageValues.Count)).ToArray());
         A.CallTo(() => _populationDataCollector.AllValuesFor(_ageField.ParameterPath)).Returns(_ageValues.Concat(_ageValues).ToArray());
         A.CallTo(() => _populationDataCollector.AllCovariateValuesFor(_covariateField.Covariate)).Returns(_genderValues.Concat(_genderValues).Concat(_genderValues).ToArray());
      }

      [Observation]
      public void should_return_a_table_with_one_column_for_each_field()
      {
         _dataTable.Columns.Contains(_ageField.Name).ShouldBeTrue();
         _dataTable.Columns.Contains(_ageClassField.Name).ShouldBeTrue();
         _dataTable.Columns.Contains(_covariateField.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_return_a_table_with_one_column_filled_for_each_field()
      {
         _dataTable.AllValuesInColumn<double>(_ageField.Name).ShouldOnlyContainInOrder(_ageValues.Concat(_ageValues).ToArray());
         _dataTable.AllValuesInColumn<string>(_ageClassField.Name).ShouldOnlyContainInOrder(
            Enumerable.Repeat(_referenceGroupingItem.Label, _ageValues.Count).Concat(
               _ageClassValues).ToArray());

         _dataTable.AllValuesInColumn<string>(_covariateField.Name).ShouldOnlyContainInOrder(
            Enumerable.Repeat(_referenceGroupingItem.Label, _ageValues.Count).Concat(new[] {_genderValues[1]}).Concat(
               _genderValues).ToArray());
      }

      [Observation]
      public void covariate_field_should_know_reference_grouping_item()
      {
         _covariateField.GroupingItems.Contains(_referenceGroupingItem).ShouldBeTrue();
         _covariateField.GroupingByName(_referenceGroupingItem.Label).ShouldBeEqualTo(_referenceGroupingItem);
      }
   }
}