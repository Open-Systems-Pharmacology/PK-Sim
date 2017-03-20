using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation
{
   public abstract class concern_for_NumberOfBinsGroupingPresenter : ContextSpecification<INumberOfBinsGroupingPresenter>
   {
      protected INumberOfBinsGroupingView _view;
      protected IPopulationDataCollector _populationDataCollector;
      protected PopulationAnalysisParameterField _field;
      private IDimension _dimension;
      private Unit _unit;
      protected IGroupingLabelGenerator _groupingLabelGenerator;
      protected BinSizeGroupingDTO _dto;
      private PopulationAnalysis _populationAnalysis;
      private readonly List<double> _values = new List<double> {1, 2, 3};
      protected IColorGradientGenerator _colorGradientGenerator;

      protected override void Context()
      {
         _view = A.Fake<INumberOfBinsGroupingView>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _populationAnalysis = new PopulationPivotAnalysis();
         _field = A.Fake<PopulationAnalysisParameterField>().WithName("Field");
         A.CallTo(() => _field.CanBeUsedForGroupingIn(_populationDataCollector)).Returns(true);
         A.CallTo(() => _field.GetValues(_populationDataCollector)).Returns(_values);
         _populationAnalysis.Add(_field);
         _groupingLabelGenerator = A.Fake<IGroupingLabelGenerator>();
         _colorGradientGenerator = A.Fake<IColorGradientGenerator>();
         sut = new NumberOfBinsGroupingPresenter(_view, _groupingLabelGenerator, _colorGradientGenerator);

         //standard action for all tests
         _dimension = A.Fake<IDimension>();
         _unit = A.Fake<Unit>();
         _field.Dimension = _dimension;
         _field.DisplayUnit = _unit;

         A.CallTo(() => _view.BindTo(A<BinSizeGroupingDTO>._))
            .Invokes(x => _dto = x.GetArgument<BinSizeGroupingDTO>(0));
      }
   }

   public class When_initializing_the_bin_size_grouping_presenter : concern_for_NumberOfBinsGroupingPresenter
   {
      private readonly List<string> _labels = new List<string> {"Label1", "Label2"};
      private readonly List<Color> _colors = new List<Color> {Color.Blue, Color.Red};

      protected override void Context()
      {
         base.Context();
         A.CallTo(_groupingLabelGenerator).WithReturnType<IReadOnlyList<string>>().Returns(_labels);
         A.CallTo(_colorGradientGenerator).WithReturnType<IReadOnlyList<Color>>().Returns(_colors);
      }

      protected override void Because()
      {
         sut.InitializeWith(_field, _populationDataCollector);
         sut.StartCreate();
      }

      [Observation]
      public void should_have_generated_the_default_labels()
      {
         _dto.Labels.Select(x => x.Label).ShouldOnlyContain("Label1", "Label2");
      }

      [Observation]
      public void should_have_generated_the_default_colors()
      {
         _dto.Labels.Select(x => x.Color).ShouldOnlyContain(Color.Blue, Color.Red);
      }

      [Observation]
      public void should_have_updated_the_view_with_the_created_labels()
      {
         A.CallTo(() => _view.RefreshLabels()).MustHaveHappened();
      }
   }

   public class When_asked_for_the_available_pattern_generation_strategies : concern_for_NumberOfBinsGroupingPresenter
   {
      [Observation]
      public void should_return_the_available_strategies()
      {
         sut.AvailableStrategies.ShouldOnlyContain(LabelGenerationStrategies.Alpha, LabelGenerationStrategies.Numeric, LabelGenerationStrategies.Roman);
      }
   }

   public class When_retrieving_the_number_of_bins_grouping_definition_bassed_on_the_user_input : concern_for_NumberOfBinsGroupingPresenter
   {
      private NumberOfBinsGroupingDefinition _result;

      protected override void Context()
      {
         base.Context();
         sut.InitializeWith(_field, _populationDataCollector);
         sut.StartCreate();
         _dto.NumberOfBins = 3;
         _dto.Labels.Add(new GroupingItemDTO {Label = "Label1"});
         _dto.Labels.Add(new GroupingItemDTO { Label = "Label2" });
         _dto.Labels.Add(new GroupingItemDTO { Label = "Label3" });
         _dto.Strategy=LabelGenerationStrategies.Roman;
      }

      protected override void Because()
      {
         _result = sut.GroupingDefinition as NumberOfBinsGroupingDefinition;
      }

      [Observation]
      public void should_return_a_grouping_definition_with_the_accurate_number_of_bins()
      {
         _result.ShouldNotBeNull();
         _result.NumberOfBins.ShouldBeEqualTo(_dto.NumberOfBins);
      }

      [Observation]
      public void should_return_a_grouping_definition_with_the_accurate_labels()
      {
         _result.Labels.ShouldOnlyContainInOrder("Label1", "Label2", "Label3");
      }

      [Observation]
      public void should_have_saved_the_strategy()
      {
         _result.Strategy.ShouldBeEqualTo(_dto.Strategy);
      }
   }

   public class When_initializing_the_number_of_bins_grouping_presenter_with_a_field_having_undefined_values : concern_for_NumberOfBinsGroupingPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _field.CanBeUsedForGroupingIn(_populationDataCollector)).Returns(false);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(()=>sut.InitializeWith(_field, _populationDataCollector)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_editing_a_number_of_bins_grouping_definiton : concern_for_NumberOfBinsGroupingPresenter
   {
      private NumberOfBinsGroupingDefinition _groupingDefinition;

      protected override void Context()
      {
         base.Context();
         _groupingDefinition =new NumberOfBinsGroupingDefinition(_field.Name);
         _groupingDefinition.StartColor = Color.Red;
         _groupingDefinition.EndColor = Color.PowderBlue;
         _groupingDefinition.NamingPattern = "Tralala";
         _groupingDefinition.NumberOfBins = 2;
         _groupingDefinition.Strategy = LabelGenerationStrategies.Alpha;
         _groupingDefinition.AddItem(new GroupingItem{Label = "Label1"});
         _groupingDefinition.AddItem(new GroupingItem{Label = "Label2"});
         sut.InitializeWith(_field, _populationDataCollector);
      }

      protected override void Because()
      {
         sut.Edit(_groupingDefinition);
      }

      [Observation]
      public void should_display_the_expected_labels_in_the_view()
      {
         _dto.Labels[0].Label.ShouldBeEqualTo("Label1");
         _dto.Labels[1].Label.ShouldBeEqualTo("Label2");
         _dto.StartColor.ShouldBeEqualTo(_groupingDefinition.StartColor);
         _dto.EndColor.ShouldBeEqualTo(_groupingDefinition.EndColor);
         _dto.NamingPattern.ShouldBeEqualTo(_groupingDefinition.NamingPattern);
         _dto.Strategy.ShouldBeEqualTo(_groupingDefinition.Strategy);
      }
   }
}