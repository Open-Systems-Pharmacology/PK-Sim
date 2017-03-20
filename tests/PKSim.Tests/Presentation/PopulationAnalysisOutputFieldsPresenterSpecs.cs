using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisOutputFieldsPresenter : ContextSpecification<IPopulationAnalysisOutputFieldsPresenter>
   {
      protected IPopulationAnalysisFieldsView _view;
      protected IPopulationAnalysesContextMenuFactory _contextMenuFactory;
      protected IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;
      protected IEventPublisher _eventPublisher;
      protected IPopulationAnalysisGroupingFieldCreator _populationAnalysisGroupingFieldCreator;
      protected IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;
      protected IDialogCreator _dialogCreator;
      protected IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper _fieldDTOMapper;
      private IPopulationDataCollector _populationDataCollector;
      protected PopulationPivotAnalysis _populationAnalysis;
      protected IDimension _dimension1;
      protected IDimension _dimension2;

      protected override void Context()
      {
         _view = A.Fake<IPopulationAnalysisFieldsView>();
         _contextMenuFactory = A.Fake<IPopulationAnalysesContextMenuFactory>();
         _populationAnalysisFieldFactory = A.Fake<IPopulationAnalysisFieldFactory>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _populationAnalysisGroupingFieldCreator = A.Fake<IPopulationAnalysisGroupingFieldCreator>();
         _populationAnalysisTemplateTask = A.Fake<IPopulationAnalysisTemplateTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _fieldDTOMapper = A.Fake<IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper>();
         sut = new PopulationAnalysisOutputFieldsPresenter(_view, _contextMenuFactory, _populationAnalysisFieldFactory, _eventPublisher, _populationAnalysisGroupingFieldCreator, _populationAnalysisTemplateTask, _dialogCreator, _fieldDTOMapper);

         A.CallTo(() => _view.SelectedField).Returns(null);
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _populationAnalysis = new PopulationPivotAnalysis();
         sut.StartAnalysis(_populationDataCollector, _populationAnalysis);

         _dimension1 = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _dimension2 = DomainHelperForSpecs.LengthDimensionForSpecs();

         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(A<IQuantity>._, A<string>._))
            .ReturnsLazily(s => new PopulationAnalysisOutputField { Dimension = s.Arguments[0].DowncastTo<IQuantity>().Dimension, Name = s.Arguments[1].ToString() });
      }
   }

   public class When_adding_an_output_field_to_the_selection_that_has_no_field_selected_so_far : concern_for_PopulationAnalysisOutputFieldsPresenter
   {
      private IQuantity _output;

      protected override void Context()
      {
         base.Context();
         _output = A.Fake<IQuantity>().WithName("Q1").WithDimension(_dimension1);
      }

      protected override void Because()
      {
         sut.AddOutput(_output, _output.Name);
      }

      [Observation]
      public void should_add_the_field_to_the_analysis()
      {
         _populationAnalysis.FieldByName(_output.Name).ShouldNotBeNull();
      }

      [Observation]
      public void should_add_the_field_to_the_data_area()
      {
         _populationAnalysis.GetArea(_output.Name).ShouldBeEqualTo(PivotArea.DataArea);
      }
   }

   public class When_adding_an_output_field_to_the_selection_that_has_the_same_dimension_as_existing_fields : concern_for_PopulationAnalysisOutputFieldsPresenter
   {
      private IQuantity _output1;
      private IQuantity _output2;

      protected override void Context()
      {
         base.Context();
         _output1 = A.Fake<IQuantity>().WithName("Q1").WithDimension(_dimension1);
         _output2 = A.Fake<IQuantity>().WithName("Q2").WithDimension(_dimension1);
         sut.AddOutput(_output1, _output1.Name);
      }

      protected override void Because()
      {
         sut.AddOutput(_output2, _output2.Name);
      }

      [Observation]
      public void should_add_the_field_to_the_analysis()
      {
         _populationAnalysis.FieldByName(_output1.Name).ShouldNotBeNull();
         _populationAnalysis.FieldByName(_output2.Name).ShouldNotBeNull();
      }
   }

   public class When_adding_an_output_field_to_the_selection_that_does_not_have_the_same_dimension_as_existing_fields : concern_for_PopulationAnalysisOutputFieldsPresenter
   {
      private IQuantity _output1;
      private IQuantity _output2;

      protected override void Context()
      {
         base.Context();
         _output1 = A.Fake<IQuantity>().WithName("Q1").WithDimension(_dimension1);
         _output2 = A.Fake<IQuantity>().WithName("Q2").WithDimension(_dimension2);
         sut.AddOutput(_output1, _output1.Name);
      }

      [Observation]
      public void should_not_add_the_field_to_the_analysis()
      {
         The.Action(() => sut.AddOutput(_output2, _output2.Name)).ShouldThrowAn<PKSimException>();
      }
   }
}