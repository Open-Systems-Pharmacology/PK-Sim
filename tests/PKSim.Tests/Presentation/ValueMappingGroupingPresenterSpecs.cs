using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;



namespace PKSim.Presentation
{
   public abstract class concern_for_ValueMappingGroupingPresenter : ContextSpecification<IValueMappingGroupingPresenter>
   {
      private IValueMappingGroupingView _view;
      protected List<GroupingLabelDTO> _allLabels;
      private IColorGenerator _colorGenerator;
      protected PopulationAnalysisCovariateField _covariateField;
      protected IPopulationDataCollector _populationDataCollector;
      private ISymbolGenerator _symbolGenerator;

      protected override void Context()
      {
         _view= A.Fake<IValueMappingGroupingView>();
         _colorGenerator= A.Fake<IColorGenerator>();
         _symbolGenerator= A.Fake<ISymbolGenerator>();
         sut = new ValueMappingGroupingPresenter(_view, _colorGenerator, _symbolGenerator);
         _populationDataCollector = A.Fake<IPopulationDataCollector>();

         _covariateField = A.Fake<PopulationAnalysisCovariateField>();
         A.CallTo(() => _covariateField.GetValues(_populationDataCollector)).Returns(new[] { "Male", "Male", "Female", "Female" });

         A.CallTo(() => _view.BindTo(A<IEnumerable<GroupingLabelDTO>>._))
            .Invokes(x => _allLabels = x.GetArgument<IEnumerable<GroupingLabelDTO>>(0).ToList());

      }
   }

   public class When_creating_a_value_mapping_grouping_for_a_covariate_field : concern_for_ValueMappingGroupingPresenter
   {
      private ValueMappingGroupingDefinition _grouping;

      protected override void Context()
      {
         base.Context();
         sut.InitializeWith(_covariateField, _populationDataCollector);
         sut.StartCreate();

         _allLabels[0].Label = "Label1";
         _allLabels[0].Sequence = 2;
         _allLabels[1].Label = "Label2";
         _allLabels[1].Sequence = 1;
      }

      protected override void Because()
      {
         _grouping = sut.GroupingDefinition.DowncastTo<ValueMappingGroupingDefinition>();
      }

      [Observation]
      public void should_add_all_disctinct_values_to_the_mapping_for_the_given_covariate()
      {
         _allLabels.Select(x => x.Value).ShouldOnlyContainInOrder("Male", "Female");
      }

      [Observation]
      public void should_return_a_value_mapping_definition_with_the_value_entered_by_the_user()
      {
         _grouping.Mapping["Male"].Label.ShouldBeEqualTo("Label1");
         _grouping.Mapping["Female"].Label.ShouldBeEqualTo("Label2");
      }


      [Observation]
      public void should_return_a_value_mapping_definition_with_the_sequence_defined_by_the_user()
      {
         _grouping.Mapping.ElementAt(0).Label.ShouldBeEqualTo("Label2");
         _grouping.Mapping.ElementAt(1).Label.ShouldBeEqualTo("Label1");
      }
   }

   public class When_editing_a_grouping_definition : concern_for_ValueMappingGroupingPresenter
   {
      private ValueMappingGroupingDefinition _groupingDefinition;

      protected override void Context()
      {
         base.Context();
         _groupingDefinition = new ValueMappingGroupingDefinition(_covariateField.Name);
         _groupingDefinition.AddValueLabel("Male",new GroupingItem{Label = "Label1"});
         _groupingDefinition.AddValueLabel("Female",new GroupingItem{Label = "Label2"});
         sut.InitializeWith(_covariateField, _populationDataCollector);
      }

      protected override void Because()
      {
         sut.Edit(_groupingDefinition);
      }

      [Observation]
      public void should_edited_the_expected_value()
      {
         _allLabels.Count.ShouldBeEqualTo(2);
         _allLabels.Select(x=>x.Label).ShouldOnlyContain("Label1","Label2");
         _allLabels.Single(x=>x.Label=="Label1").Sequence.ShouldBeEqualTo((uint)1);
         _allLabels.Single(x => x.Label == "Label2").Sequence.ShouldBeEqualTo((uint)2);
      }
   }
}	