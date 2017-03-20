using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation
{
   public abstract class concern_for_FixedLimitsGroupingPresenter : ContextSpecification<IFixedLimitsGroupingPresenter>
   {
      private IFixedLimitsGroupingView _view;
      protected IPopulationDataCollector _populationDataCollector;
      protected PopulationAnalysisParameterField _field;
      protected IEnumerable<FixedLimitGroupingDTO> _allDTOs;
      protected IDimension _dimension;
      protected Unit _unit;
      private readonly List<double> _values = new List<double> {1, 2, 3};
      private IColorGenerator _colorGenerator;
      private ISymbolGenerator _symbolGenerator;

      protected override void Context()
      {
         _view = A.Fake<IFixedLimitsGroupingView>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _field = A.Fake<PopulationAnalysisParameterField>();
         _colorGenerator = A.Fake<IColorGenerator>();
         _symbolGenerator = A.Fake<ISymbolGenerator>();
         sut = new FixedLimitsGroupingPresenter(_view, _colorGenerator,_symbolGenerator);

         //standard action for all tests
         _dimension = A.Fake<IDimension>();
         _unit = A.Fake<Unit>();
         _field.Dimension = _dimension;
         _field.DisplayUnit = _unit;
         A.CallTo(() => _view.BindTo(A<IEnumerable<FixedLimitGroupingDTO>>._, A<Unit>._))
            .Invokes(x => _allDTOs = x.GetArgument<NotifyList<FixedLimitGroupingDTO>>(0).DowncastTo<IEnumerable<FixedLimitGroupingDTO>>());

         A.CallTo(() => _field.CanBeUsedForGroupingIn(_populationDataCollector)).Returns(true);
         A.CallTo(() => _field.GetValues(_populationDataCollector)).Returns(_values);
      }
   }

   public class When_starting_the_fixed_limits_grouping_presenter : concern_for_FixedLimitsGroupingPresenter
   {
      private double _minimumInDisplayUnit;
      private double _maximumInDisplayUnit;

      protected override void Context()
      {
         base.Context();
         _minimumInDisplayUnit = 0.1;
         _maximumInDisplayUnit = 0.3;
         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_unit, 1)).Returns(_minimumInDisplayUnit);
         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_unit, 2)).Returns(0.2);
         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_unit, 3)).Returns(_maximumInDisplayUnit);
      }

      protected override void Because()
      {
         sut.InitializeWith(_field, _populationDataCollector);
         sut.StartCreate();
      }

      [Observation]
      public void should_add_two_entries_to_the_view_that_cannot_be_deleted()
      {
         _allDTOs.Count().ShouldBeEqualTo(2);
         _allDTOs.Each(dto => dto.CanDelete.ShouldBeFalse());
      }

      [Observation]
      public void the_first_entry_should_contain_the_minimum_of_the_original_field_in_display_unit()
      {
         var minDTO = _allDTOs.ElementAt(0);
         minDTO.MaximumEditable.ShouldBeTrue();
         minDTO.Maximum.ShouldBeNull();
         minDTO.Minimum.ShouldBeEqualTo(_minimumInDisplayUnit);
      }

      [Observation]
      public void the_second_entry_should_contain_the_maximum_of_the_original_field_in_display_unit()
      {
         var maxDTO = _allDTOs.ElementAt(1);
         maxDTO.MaximumEditable.ShouldBeFalse();
         maxDTO.Minimum.ShouldBeNull();
         maxDTO.Maximum.ShouldBeEqualTo(_maximumInDisplayUnit);
      }
   }

   public class When_adding_a_new_fixed_limit_after_a_specific_limit : concern_for_FixedLimitsGroupingPresenter
   {
      private FixedLimitGroupingDTO _firstItem;
      private FixedLimitGroupingDTO _lastItem;

      protected override void Context()
      {
         base.Context();
         sut.InitializeWith(_field, _populationDataCollector);
         sut.StartCreate();
         _firstItem = _allDTOs.First();
         _lastItem = _allDTOs.Last();
         _firstItem.Maximum = 15;
      }

      protected override void Because()
      {
         sut.AddFixedLimitAfter(_firstItem);
      }

      [Observation]
      public void should_add_a_new_line_item_after_the_selected_one()
      {
         _allDTOs.Count().ShouldBeEqualTo(3);
         _allDTOs.First().ShouldBeEqualTo(_firstItem);
         _allDTOs.Last().ShouldBeEqualTo(_lastItem);

         //new item is then in the middle
      }

      [Observation]
      public void should_have_set_the_minimum_of_the_new_item_to_the_maximum_of_the_selected_item()
      {
         _allDTOs.ElementAt(1).Minimum.ShouldBeEqualTo(_firstItem.Maximum);
      }
   }

   public class When_removing_an_item_from_the_specified_limits : concern_for_FixedLimitsGroupingPresenter
   {
      private FixedLimitGroupingDTO _firstItem;
      private FixedLimitGroupingDTO _lastItem;
      private FixedLimitGroupingDTO _secondItem;

      protected override void Context()
      {
         base.Context();
         sut.InitializeWith(_field, _populationDataCollector);
         sut.StartCreate();

         _firstItem = _allDTOs.First();
         _lastItem = _allDTOs.Last();
         //add items
         sut.AddFixedLimitAfter(_firstItem);
         _secondItem = _allDTOs.ElementAt(1);

         _firstItem.Maximum = 15;
         _secondItem.Minimum = 15;
         _secondItem.Maximum = 20;
         _lastItem.Minimum = 20;
         _lastItem.Maximum = 30;
      }

      protected override void Because()
      {
         sut.RemoveFixedLimit(_secondItem);
      }

      [Observation]
      public void should_actually_remove_the_item()
      {
         _allDTOs.ShouldOnlyContainInOrder(_firstItem, _lastItem);
      }

      [Observation]
      public void should_have_set_the_minimum_of_the_next_item_to_the_maximum_of_the_previous_item()
      {
         _lastItem.Minimum.ShouldBeEqualTo(_firstItem.Maximum);
      }
   }

   public class When_retrieving_the_fixed_limit_grouping_definition_bassed_on_the_user_input : concern_for_FixedLimitsGroupingPresenter
   {
      private FixedLimitGroupingDTO _firstItem;
      private FixedLimitGroupingDTO _lastItem;
      private FixedLimitGroupingDTO _secondItem;
      private FixedLimitsGroupingDefinition _result;
      private double _maximum1;
      private double _maximum2;

      protected override void Context()
      {
         base.Context();
         sut.InitializeWith(_field, _populationDataCollector);
         sut.StartCreate();
         _firstItem = _allDTOs.First();
         _lastItem = _allDTOs.Last();
         //add items
         sut.AddFixedLimitAfter(_firstItem);
         _secondItem = _allDTOs.ElementAt(1);

         _firstItem.Maximum = 15;
         _firstItem.Label = "Label1";
         _secondItem.Maximum = 20;
         _secondItem.Label = "Label2";
         _lastItem.Maximum = 30;
         _lastItem.Label = "Label3";

         _maximum1 = 1;
         _maximum2 = 2;

         A.CallTo(() => _dimension.UnitValueToBaseUnitValue(_unit, _firstItem.Maximum.Value)).Returns(_maximum1);
         A.CallTo(() => _dimension.UnitValueToBaseUnitValue(_unit, _secondItem.Maximum.Value)).Returns(_maximum2);
      }

      protected override void Because()
      {
         _result = sut.GroupingDefinition as FixedLimitsGroupingDefinition;
      }

      [Observation]
      public void should_return_a_grouping_definition_with_the_accurate_limits_in_core_unit()
      {
         _result.ShouldNotBeNull();
         _result.Limits.ShouldOnlyContainInOrder(_maximum1, _maximum2);
      }

      [Observation]
      public void should_return_a_grouping_definition_with_the_accurate_labels()
      {
         _result.Labels.ShouldOnlyContainInOrder("Label1", "Label2", "Label3");
      }
   }

   public class When_editing_a_fixed_limit_grouping : concern_for_FixedLimitsGroupingPresenter
   {
      private FixedLimitsGroupingDefinition _groupingDefinition;

      protected override void Context()
      {
         base.Context();
         _groupingDefinition = new FixedLimitsGroupingDefinition(_field.Name);
         _groupingDefinition.SetLimits(new[] {1d, 2d}.OrderBy(x => x));
         _groupingDefinition.AddItem(new GroupingItem{Label = "Label1"});
         _groupingDefinition.AddItem(new GroupingItem{Label = "Label2"});
         _groupingDefinition.AddItem(new GroupingItem{Label = "Label3"});

         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_unit, 1d)).Returns(10);
         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_unit, 2d)).Returns(20);

         sut.InitializeWith(_field, _populationDataCollector);
      }

      protected override void Because()
      {
         sut.Edit(_groupingDefinition);
      }

      [Observation]
      public void should_add_the_expected_items_to_be_edited_in_the_view()
      {
         _allDTOs.Count().ShouldBeEqualTo(3);
         _allDTOs.ElementAt(0).Maximum.ShouldBeEqualTo(10);
         _allDTOs.ElementAt(0).Label.ShouldBeEqualTo("Label1");
         _allDTOs.ElementAt(1).Maximum.ShouldBeEqualTo(20);
         _allDTOs.ElementAt(1).Label.ShouldBeEqualTo("Label2");
         _allDTOs.ElementAt(2).Label.ShouldBeEqualTo("Label3");
         _allDTOs.ElementAt(2).Minimum.ShouldBeEqualTo(20);
      }
   }
}