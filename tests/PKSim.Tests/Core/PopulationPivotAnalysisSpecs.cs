using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Infrastructure.Services;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationPivotAnalysis : ContextSpecification<PopulationPivotAnalysis>
   {
      protected ICloneManager _cloneManager;
      private IModelFinalizer _modelFinalizer;

      protected override void Context()
      {
         sut = new PopulationPivotAnalysis();
         _modelFinalizer= A.Fake<IModelFinalizer>();  
         _cloneManager = new CloneManagerForModel(new PopulationAnalysisObjectBaseFactoryForSpecs(), new DataRepositoryTask(),_modelFinalizer);
      }
   }

   public class When_updating_a_population_pivot_analysis_from_an_existing_pivot_analysis : concern_for_PopulationPivotAnalysis
   {
      private PopulationPivotAnalysis _sourcePivotAnalysis;
      private PopulationAnalysisPKParameterField _pkParameterField;
      private PopulationAnalysisParameterField _parameterField;

      protected override void Context()
      {
         base.Context();
         _sourcePivotAnalysis = new PopulationPivotAnalysis();
         _pkParameterField = new PopulationAnalysisPKParameterField().WithName("PKParam");
         _pkParameterField.PKParameter = "AUC";
         _pkParameterField.QuantityPath = "A|B|C";

         _parameterField = new PopulationAnalysisParameterField().WithName("Param");
         _parameterField.ParameterPath = "A|B|C|D";
         _sourcePivotAnalysis.Add(_pkParameterField);
         _sourcePivotAnalysis.Add(_parameterField);
         _sourcePivotAnalysis.SetPosition(_parameterField, PivotArea.DataArea, 1);
         _sourcePivotAnalysis.SetPosition(_pkParameterField, PivotArea.RowArea, 2);
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_sourcePivotAnalysis, _cloneManager);
      }

      [Observation]
      public void should_return_an_analysis_containing_the_same_fields_at_the_same_position()
      {
         sut.AllFields.Count.ShouldBeEqualTo(2);

         sut.GetPosition(_parameterField.Name).Area.ShouldBeEqualTo(PivotArea.DataArea);
         sut.GetPosition(_parameterField.Name).Index.ShouldBeEqualTo(1);

         sut.GetPosition(_pkParameterField.Name).Area.ShouldBeEqualTo(PivotArea.RowArea);
         sut.GetPosition(_pkParameterField.Name).Index.ShouldBeEqualTo(2);
      }
   }

   public class When_setting_the_field_defined_for_color_grouping : concern_for_PopulationPivotAnalysis
   {
      private PopulationPivotAnalysis _pivotAnalysis;
      private PopulationAnalysisCovariateField _field;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis = new PopulationPivotAnalysis();
         _field = new PopulationAnalysisCovariateField().WithName("Param");
         _pivotAnalysis.Add(_field);
      }

      protected override void Because()
      {
         _pivotAnalysis.ColorField = _field;
      }

      [Observation]
      public void should_set_the_field_at_the_index_0_in_the_color_area()
      {
         var position = _pivotAnalysis.GetPosition(_field);
         position.Area.ShouldBeEqualTo(PivotArea.ColorArea);
         position.Index.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_set_the_field_as_the_color_field()
      {
         _pivotAnalysis.ColorField.ShouldBeEqualTo(_field);
      }
   }

   public class When_setting_the_field_defined_for_symbol_grouping : concern_for_PopulationPivotAnalysis
   {
      private PopulationPivotAnalysis _pivotAnalysis;
      private PopulationAnalysisCovariateField _field;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis = new PopulationPivotAnalysis();
         _field = new PopulationAnalysisCovariateField().WithName("Param");
         _pivotAnalysis.Add(_field);
      }

      protected override void Because()
      {
         _pivotAnalysis.SymbolField = _field;
      }

      [Observation]
      public void should_set_the_field_at_the_index_0_in_the_symbol_area()
      {
         var position = _pivotAnalysis.GetPosition(_field);
         position.Area.ShouldBeEqualTo(PivotArea.SymbolArea);
         position.Index.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_set_the_field_as_the_symbol_field()
      {
         _pivotAnalysis.SymbolField.ShouldBeEqualTo(_field);
      }
   }

   public class When_setting_a_field_in_the_color_grouping_and_another_field_is_already_there : concern_for_PopulationPivotAnalysis
   {
      private PopulationPivotAnalysis _pivotAnalysis;
      private PopulationAnalysisCovariateField _parameterField;
      private PopulationAnalysisCovariateField _existingColorField;
      private PopulationAnalysisParameterField _filterArea;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis = new PopulationPivotAnalysis();
         _parameterField = new PopulationAnalysisCovariateField().WithName("Param");
         _existingColorField = new PopulationAnalysisCovariateField().WithName("ColorField");
         _filterArea = new PopulationAnalysisParameterField().WithName("OnFilter");
         _pivotAnalysis.Add(_parameterField);
         _pivotAnalysis.Add(_existingColorField);
         _pivotAnalysis.Add(_filterArea);
         _pivotAnalysis.SetPosition(_filterArea, PivotArea.FilterArea, 0);
         _pivotAnalysis.ColorField = _existingColorField;
      }

      protected override void Because()
      {
         _pivotAnalysis.ColorField = _parameterField;
      }

      [Observation]
      public void should_move_the_existing_field_at_the_end_of_the_filter()
      {
         var position = _pivotAnalysis.GetPosition(_existingColorField);
         position.Area.ShouldBeEqualTo(PivotArea.FilterArea);
      }

      [Observation]
      public void should_set_the_given_field_as_new_color_field()
      {
         var position = _pivotAnalysis.GetPosition(_parameterField);
         position.Area.ShouldBeEqualTo(PivotArea.ColorArea);
         position.Index.ShouldBeEqualTo(0);
      }
   }

   public class When_setting_the_area_of_a_field_without_index : concern_for_PopulationPivotAnalysis
   {
      private PopulationPivotAnalysis _pivotAnalysis;
      private PopulationAnalysisCovariateField _parameterField;
      private PopulationAnalysisParameterField _dataArea;

      protected override void Context()
      {
         base.Context();
         _pivotAnalysis = new PopulationPivotAnalysis();
         _parameterField = new PopulationAnalysisCovariateField().WithName("Param");
         _dataArea = new PopulationAnalysisParameterField().WithName("OnFilter");
         _pivotAnalysis.Add(_parameterField);
         _pivotAnalysis.Add(_dataArea);
         _pivotAnalysis.SetPosition(_dataArea, PivotArea.DataArea, 0);
      }

      protected override void Because()
      {
         _pivotAnalysis.SetPosition(_parameterField, PivotArea.DataArea);
      }

      [Observation]
      public void should_set_an_automatic_index_as_being_the_last_with_the_given_area()
      {
         _pivotAnalysis.GetAreaIndex(_parameterField).ShouldBeEqualTo(1);
      }
   }
}