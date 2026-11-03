using System.Collections.Generic;
using System.Data;
using System.Linq;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Presentation
{
   public abstract class concern_for_ProteinExpressionsPresenter : ContextSpecification<IProteinExpressionsPresenter>
   {
      protected IProteinExpressionsView _view;
      protected ISubPresenterItemManager<IExpressionItemPresenter> _subPresenterManager;
      protected IDialogCreator _dialogCreator;
      protected IGeneExpressionQueries _geneExpressionQueries;
      protected IMappingPresenter _mappingPresenter;
      private IProteinExpressionDataHelper _dataHelper;
      protected IExpressionQueryTask _expressionQueryTask;
      protected IExpressionQueryCalculator _expressionQueryCalculator;
      protected IExpressionDataTableMapper _expressionDataTableMapper;
      protected IExpressionDataPresenter _expressionDataPresenter;
      protected ITransferPresenter _transferPresenter;
      protected IProteinSelectionPresenter _proteinSelectionPresenter;
      protected ExpressionQuery _query;
      protected QueryExpressionSettings _querySettings;
      protected IReadOnlyList<ContainerExpressionDataRecord> _containerRecords;

      protected override void Context()
      {
         _view = A.Fake<IProteinExpressionsView>();
         _subPresenterManager = SubPresenterHelper.Create<IExpressionItemPresenter>();
         ExpressionItems.ProteinSelection.Index = 0;
         ExpressionItems.ExpressionData.Index = 1;
         ExpressionItems.Transfer.Index = 2;
         _proteinSelectionPresenter = _subPresenterManager.CreateFake(ExpressionItems.ProteinSelection);
         _expressionDataPresenter = _subPresenterManager.CreateFake(ExpressionItems.ExpressionData);
         _transferPresenter = _subPresenterManager.CreateFake(ExpressionItems.Transfer);
         _dialogCreator = A.Fake<IDialogCreator>();
         _geneExpressionQueries = A.Fake<IGeneExpressionQueries>();
         _mappingPresenter = A.Fake<IMappingPresenter>();
         _dataHelper = A.Fake<IProteinExpressionDataHelper>();
         _expressionQueryTask = A.Fake<IExpressionQueryTask>();
         _expressionQueryCalculator = A.Fake<IExpressionQueryCalculator>();
         _expressionDataTableMapper = A.Fake<IExpressionDataTableMapper>();

         sut = new ProteinExpressionsPresenter(_view, _subPresenterManager, _dialogCreator, _geneExpressionQueries, _mappingPresenter, _dataHelper,
            _expressionQueryTask, _expressionQueryCalculator, _expressionDataTableMapper);

         _query = ExpressionQueryForSpecs.Query();
         _query.LayoutSettings = "LAYOUT";
         _querySettings = new QueryExpressionSettings(ExpressionQueryForSpecs.Containers(), "QUERY_CONFIGURATION", "CYP3A4");
         _containerRecords = new List<ContainerExpressionDataRecord>();
         A.CallTo(() => _expressionQueryTask.QueryFrom("QUERY_CONFIGURATION")).Returns(_query);
         A.CallTo(() => _expressionQueryCalculator.ContainerRecordsFor(_query, A<IReadOnlyList<ExpressionContainerInfo>>._)).Returns(_containerRecords);
         var containerTable = new DataTable();
         containerTable.Columns.Add("A");
         containerTable.Columns.Add("B");
         containerTable.Columns.Add("C");
         A.CallTo(() => _dataHelper.ConvertToDataTable(A<object[]>._)).Returns(containerTable);
         sut.InitializeWith(A.Fake<ICommandCollector>());
      }
   }

   public class When_the_protein_expressions_presenter_is_closed : concern_for_ProteinExpressionsPresenter
   {
      protected override void Because()
      {
         sut.Dispose();
      }

      [Observation]
      public void should_close_modal_presenters()
      {
         A.CallTo(() => _mappingPresenter.Dispose()).MustHaveHappened();
      }
   }

   public class When_the_protein_expressions_presenter_is_initialized_with_a_saved_query : concern_for_ProteinExpressionsPresenter
   {
      protected override void Because()
      {
         sut.InitializeSettings(_querySettings);
      }

      [Observation]
      public void should_display_the_container_records_of_the_saved_query()
      {
         A.CallTo(() => _expressionDataPresenter.SetData("CYP3A4", _containerRecords)).MustHaveHappened();
      }

      [Observation]
      public void should_restore_the_layout_and_then_the_filter_of_the_saved_query()
      {
         A.CallTo(() => _expressionDataPresenter.SetLayoutSetting("LAYOUT")).MustHaveHappened()
            .Then(A.CallToSet(() => _expressionDataPresenter.Filter).To(_query.Filter).MustHaveHappened());
      }

      [Observation]
      public void should_not_warn_the_user()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_the_protein_expressions_presenter_is_initialized_with_a_query_saved_with_a_filter_that_cannot_be_restored : concern_for_ProteinExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _query.Filter = null;
         A.CallTo(() => _expressionDataPresenter.SetLayoutSetting("LAYOUT")).Returns("[NORM_VALUE] > 5");
      }

      protected override void Because()
      {
         sut.InitializeSettings(_querySettings);
      }

      [Observation]
      public void should_tell_the_user_which_part_of_the_filter_was_removed()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>.That.Contains("[NORM_VALUE] > 5"))).MustHaveHappened();
      }

      [Observation]
      public void should_keep_the_filter_restored_by_the_view()
      {
         A.CallToSet(() => _expressionDataPresenter.Filter).MustNotHaveHappened();
      }
   }

   public class When_the_protein_expressions_presenter_moves_to_the_transfer_of_the_expressions : concern_for_ProteinExpressionsPresenter
   {
      private ExpressionDataFilter _viewFilter;
      private IReadOnlyList<UnitExpression> _unitExpressions;

      protected override void Context()
      {
         base.Context();
         _query.SelectedUnit = ExpressionQueryForSpecs.EST;
         _viewFilter = new ExpressionDataFilter();
         _unitExpressions = new List<UnitExpression>();
         A.CallTo(() => _expressionDataPresenter.Filter).Returns(_viewFilter);
         A.CallTo(() => _expressionQueryCalculator.UnitExpressionsFor(_query, A<IReadOnlyList<ExpressionContainerInfo>>._)).Returns(_unitExpressions);
         sut.InitializeSettings(_querySettings);
      }

      protected override void Because()
      {
         sut.WizardCurrent(ExpressionItems.ExpressionData.Index, ExpressionItems.Transfer.Index);
      }

      [Observation]
      public void should_compute_the_unit_expressions_with_the_filter_edited_in_the_view()
      {
         _query.Filter.ShouldBeEqualTo(_viewFilter);
      }

      [Observation]
      public void should_display_the_unit_expressions_with_the_selected_unit_of_the_query()
      {
         A.CallTo(() => _transferPresenter.SetData(_unitExpressions, ExpressionQueryForSpecs.EST)).MustHaveHappened();
      }
   }

   public class When_the_protein_expressions_presenter_returns_to_the_expression_data_from_the_transfer : concern_for_ProteinExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _transferPresenter.GetSelectedUnit()).Returns(ExpressionQueryForSpecs.RT_PCR);
         sut.InitializeSettings(_querySettings);
      }

      protected override void Because()
      {
         sut.WizardCurrent(ExpressionItems.Transfer.Index, ExpressionItems.ExpressionData.Index);
      }

      [Observation]
      public void should_remember_the_unit_selected_in_the_transfer()
      {
         _query.SelectedUnit.ShouldBeEqualTo(ExpressionQueryForSpecs.RT_PCR);
      }
   }

   public class When_the_mapping_of_the_query_is_edited : concern_for_ProteinExpressionsPresenter
   {
      private DataTable _mappingTable;
      private IReadOnlyList<TissueContainerMapping> _newMapping;
      private IReadOnlyList<ContainerExpressionDataRecord> _newContainerRecords;
      private IEnumerable<string> _tissues;

      protected override void Context()
      {
         base.Context();
         _mappingTable = new DataTable();
         _newMapping = new List<TissueContainerMapping>();
         A.CallTo(() => _expressionDataTableMapper.MappingTableFrom(_query.Mapping)).Returns(_mappingTable);
         A.CallTo(() => _expressionDataTableMapper.MappingFrom(_mappingTable)).Returns(_newMapping);
         A.CallTo(() => _mappingPresenter.EditMapping(_mappingTable, A<DataTable>._, A<IEnumerable<string>>._))
            .Invokes(x => _tissues = x.GetArgument<IEnumerable<string>>(2).ToList());
         sut.InitializeSettings(_querySettings);
         _newContainerRecords = new List<ContainerExpressionDataRecord>();
         A.CallTo(() => _expressionQueryCalculator.ContainerRecordsFor(_query, A<IReadOnlyList<ExpressionContainerInfo>>._)).Returns(_newContainerRecords);
      }

      protected override void Because()
      {
         _expressionDataPresenter.OnEditMapping += Raise.FreeForm<System.Action>.With();
         sut.MappingChanged();
      }

      [Observation]
      public void should_edit_the_mapping_of_the_query_with_the_tissues_of_its_records()
      {
         _tissues.ShouldOnlyContainInOrder("intestine", "kidney", "liver", "muscle", "skin");
      }

      [Observation]
      public void should_update_the_query_mapping_and_the_displayed_container_records()
      {
         _query.Mapping.ShouldBeEqualTo(_newMapping);
         A.CallTo(() => _expressionDataPresenter.ActualizeData(_newContainerRecords)).MustHaveHappened();
      }
   }

   public class When_retrieving_the_results_of_the_protein_expression_query : concern_for_ProteinExpressionsPresenter
   {
      private QueryExpressionResults _queryResults;
      private QueryExpressionResults _result;
      private ExpressionDataFilter _viewFilter;

      protected override void Context()
      {
         base.Context();
         _viewFilter = new ExpressionDataFilter();
         _queryResults = new QueryExpressionResults(new List<ExpressionResult>());
         A.CallTo(() => _expressionDataPresenter.Filter).Returns(_viewFilter);
         A.CallTo(() => _expressionDataPresenter.GetLayoutSetting()).Returns("NEW_LAYOUT");
         A.CallTo(() => _transferPresenter.GetSelectedUnit()).Returns(ExpressionQueryForSpecs.EST);
         A.CallTo(() => _expressionQueryTask.ResultsFor(_query, A<IReadOnlyList<ExpressionContainerInfo>>._)).Returns(_queryResults);
         sut.InitializeSettings(_querySettings);
      }

      protected override void Because()
      {
         _result = sut.GetQueryResults();
      }

      [Observation]
      public void should_return_the_results_of_the_query_with_the_filter_layout_and_unit_selected_in_the_views()
      {
         _result.ShouldBeEqualTo(_queryResults);
         _query.Filter.ShouldBeEqualTo(_viewFilter);
         _query.LayoutSettings.ShouldBeEqualTo("NEW_LAYOUT");
         _query.SelectedUnit.ShouldBeEqualTo(ExpressionQueryForSpecs.EST);
      }
   }
}
