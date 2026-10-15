using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.ProteinExpression;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public static class ColumnNamesOfTransferTable
   {
      public static string Container = "Container";
      public static string DisplayName = "DisplayName";
      public static string RelativeExpressionOld = "RelativeExpressionOld";
      public static string RelativeExpressionOldPercentage = "RelativeExpressionOldPercentage";
      public static string ExpressionValue = "ExpressionValue";
      public static string RelativeExpressionNew = "RelativeExpressionNew";
      public static string RelativeExpressionNewPercentage = "RelativeExpressionNewPercentage";
      public static string Unit = "Unit";
   };

   public interface IProteinExpressionsPresenter : IWizardPresenter, IPresenter<IProteinExpressionsView>
   {
      /// <summary>
      ///    Prepare the presenter to perform a query according to the settings defined in
      ///    <para>querySettings</para>
      /// </summary>
      /// <param name="querySettings">Settings used to initialize the presenter</param>
      void InitializeSettings(QueryExpressionSettings querySettings);

      /// <summary>
      ///    Retrieve the result of the query
      /// </summary>
      QueryExpressionResults GetQueryResults();

      void MappingChanged();

      string Title { set; }

      bool Start();
   }

   public class ProteinExpressionsPresenter : PKSimWizardPresenter<IProteinExpressionsView, IProteinExpressionsPresenter, IExpressionItemPresenter>, IProteinExpressionsPresenter
   {
      private readonly IGeneExpressionQueries _geneExpressionQueries;
      private readonly IMappingPresenter _mappingPresenter;
      private readonly IProteinExpressionDataHelper _dataHelper;
      private readonly IExpressionQueryTask _expressionQueryTask;
      private readonly IExpressionQueryCalculator _expressionQueryCalculator;
      private readonly IExpressionDataTableMapper _expressionDataTableMapper;
      private QueryExpressionSettings _querySettings;
      private IReadOnlyList<ExpressionContainerInfo> _containers;
      private ExpressionQuery _query;
      private DataTable _mappingTable;

      public ProteinExpressionsPresenter(IProteinExpressionsView view, ISubPresenterItemManager<IExpressionItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator,
         IGeneExpressionQueries geneExpressionQueries, IMappingPresenter mappingPresenter, IProteinExpressionDataHelper dataHelper,
         IExpressionQueryTask expressionQueryTask, IExpressionQueryCalculator expressionQueryCalculator, IExpressionDataTableMapper expressionDataTableMapper)
         : base(view, subPresenterItemManager, ExpressionItems.All, dialogCreator)
      {
         _geneExpressionQueries = geneExpressionQueries;
         _mappingPresenter = mappingPresenter;
         _dataHelper = dataHelper;
         _expressionQueryTask = expressionQueryTask;
         _expressionQueryCalculator = expressionQueryCalculator;
         _expressionDataTableMapper = expressionDataTableMapper;
         _mappingPresenter.MappingChanged += MappingChanged;
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         PresenterAt(ExpressionItems.ProteinSelection).OnSelectProtein += SelectProtein;
         PresenterAt(ExpressionItems.ProteinSelection).OnProteinSearched += UpdateControls;
         PresenterAt(ExpressionItems.ProteinSelection).OnSetActiveControl += onProteinSelectionPresenterOnSetActiveControl;

         PresenterAt(ExpressionItems.ExpressionData).OnEditMapping += EditMapping;
      }

      private void onProteinSelectionPresenterOnSetActiveControl()
      {
         _view.ActivateControl(ExpressionItems.ProteinSelection);
         SetWizardButtonEnabled(ExpressionItems.ProteinSelection);
      }

      public void MappingChanged()
      {
         _query.Mapping = _expressionDataTableMapper.MappingFrom(_mappingTable);
         PresenterAt(ExpressionItems.ExpressionData).ActualizeData(containerRecords());
      }

      /// <summary>
      ///    This method is called to display a mapping view to enable the user to change the container-tissue mapping.
      /// </summary>
      public void EditMapping()
      {
         _mappingTable = _expressionDataTableMapper.MappingTableFrom(_query.Mapping);
         var tissues = _query.Records.Select(x => x.Tissue).Distinct().OrderBy(x => x, StringComparer.CurrentCultureIgnoreCase);
         _mappingPresenter.EditMapping(_mappingTable, getContainerTableFromQuerySettings(), tissues);
      }

      public override void WizardCurrent(int previousIndex, int newIndex)
      {
         if (newIndex == ExpressionItems.ExpressionData.Index &&
             PresenterAt(ExpressionItems.ProteinSelection).ProteinSelectionChanged)
            PresenterAt(ExpressionItems.ProteinSelection).SelectProtein();

         if (newIndex == ExpressionItems.ExpressionData.Index && previousIndex == ExpressionItems.Transfer.Index)
            _query.SelectedUnit = PresenterAt(ExpressionItems.Transfer).GetSelectedUnit();

         if (newIndex == ExpressionItems.Transfer.Index)
         {
            if (PresenterAt(ExpressionItems.ProteinSelection).ProteinSelectionChanged)
               PresenterAt(ExpressionItems.ProteinSelection).SelectProtein();
            selectTransferData();
         }

         base.WizardCurrent(previousIndex, newIndex);
      }

      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         bool enable = isOldQuery;

         if (indexThatWillHaveFocus == ExpressionItems.ProteinSelection.Index && PresenterAt(ExpressionItems.ProteinSelection).ProteinSelectionChanged)
            enable = PresenterAt(ExpressionItems.ProteinSelection).ProteinHasData;
         else
            enable |= PresenterAt(ExpressionItems.ProteinSelection).ProteinHasData;
         var transferHasData = PresenterAt(ExpressionItems.Transfer).HasData();
         _view.OkEnabled = transferHasData;
         _view.SetControlEnabled(ExpressionItems.ExpressionData, enable);
         _view.SetControlEnabled(ExpressionItems.Transfer, enable);
         _view.NextEnabled = enable;
         _view.PreviousEnabled = enable && (indexThatWillHaveFocus != ExpressionItems.ProteinSelection.Index);
      }

      public void SelectProtein(DataRow selectedRow)
      {
         if (selectedRow == null) return;

         var id = (long) selectedRow[DatabaseConfiguration.ProteinColumns.COL_ID];
         _query = _expressionQueryTask.CreateQueryFor(id, proteinNameFrom(selectedRow));

         PresenterAt(ExpressionItems.ExpressionData).SetData(_query.ProteinName, containerRecords());
         _view.ActivateControl(ExpressionItems.ExpressionData);
         _view.SetControlEnabled(ExpressionItems.Transfer, true);
         SetWizardButtonEnabled(ExpressionItems.ExpressionData);
      }

      private static string proteinNameFrom(DataRow selectedRow)
      {
         if (selectedRow[DatabaseConfiguration.ProteinColumns.COL_SYMBOL] != DBNull.Value)
            return (string) selectedRow[DatabaseConfiguration.ProteinColumns.COL_SYMBOL];

         if (selectedRow[DatabaseConfiguration.ProteinColumns.COL_GENE_NAME] != DBNull.Value)
            return (string) selectedRow[DatabaseConfiguration.ProteinColumns.COL_GENE_NAME];

         return selectedRow[DatabaseConfiguration.ProteinColumns.COL_GENE_ID] as string;
      }

      private IReadOnlyList<ContainerExpressionDataRecord> containerRecords() => _expressionQueryCalculator.ContainerRecordsFor(_query, _containers);

      private bool isOldQuery
      {
         get { return !string.IsNullOrEmpty(_querySettings.QueryConfiguration); }
      }

      public void InitializeSettings(QueryExpressionSettings querySettings)
      {
         _querySettings = querySettings;
         _containers = querySettings.ExpressionContainers.ToList();
         PresenterAt(ExpressionItems.Transfer).ShowOldValues = isOldQuery;

         if (isOldQuery)
         {
            restoreQuery(_querySettings.QueryConfiguration);
            _view.ActivateControl(ExpressionItems.ExpressionData);
            _view.SetControlEnabled(ExpressionItems.Transfer, true);
            SetWizardButtonEnabled(ExpressionItems.ExpressionData);
         }
         else
         {
            _view.ActivateControl(ExpressionItems.ProteinSelection);
            PresenterAt(ExpressionItems.ProteinSelection).ActualizeSelection();
            if(querySettings.MoleculeName.StringIsNotEmpty())
               PresenterAt(ExpressionItems.ProteinSelection).InitWithProteinName(querySettings.MoleculeName);
            SetWizardButtonEnabled(ExpressionItems.ProteinSelection);
         }
      }

      public QueryExpressionResults GetQueryResults()
      {
         _query.Filter = PresenterAt(ExpressionItems.ExpressionData).Filter;
         _query.SelectedUnit = PresenterAt(ExpressionItems.Transfer).GetSelectedUnit();
         _query.LayoutSettings = PresenterAt(ExpressionItems.ExpressionData).GetLayoutSetting();
         return _expressionQueryTask.ResultsFor(_query, _containers);
      }

      private void selectTransferData()
      {
         _query.Filter = PresenterAt(ExpressionItems.ExpressionData).Filter;
         PresenterAt(ExpressionItems.Transfer).SetData(_expressionQueryCalculator.UnitExpressionsFor(_query, _containers), _query.SelectedUnit);
         _view.ActivateControl(ExpressionItems.Transfer);
         SetWizardButtonEnabled(ExpressionItems.Transfer);
      }

      public string Title
      {
         set { View.Caption = value; }
      }

      public bool Start()
      {
         _geneExpressionQueries.ValidateDatabase();
         if (!isOldQuery)
            PresenterAt(ExpressionItems.ProteinSelection).Activate();

         _view.Display();
         return !View.Canceled;
      }

      private DataTable getContainerTableFromQuerySettings()
      {
         DataTable containers = _dataHelper.ConvertToDataTable(_querySettings.ExpressionContainers.ToArray());
         containers.Columns[0].ColumnName = ColumnNamesOfTransferTable.Container.ToString();
         containers.Columns[1].ColumnName = ColumnNamesOfTransferTable.DisplayName.ToString();
         containers.Columns[2].ColumnName = ColumnNamesOfTransferTable.RelativeExpressionOld.ToString();
         return containers;
      }

      private void restoreQuery(string queryConfiguration)
      {
         _query = _expressionQueryTask.QueryFrom(queryConfiguration);
         var expressionDataPresenter = PresenterAt(ExpressionItems.ExpressionData);
         expressionDataPresenter.SetData(_query.ProteinName, containerRecords());

         if (_query.LayoutSettings.StringIsNotEmpty())
         {
            var discardedFilter = expressionDataPresenter.SetLayoutSetting(_query.LayoutSettings);
            if (discardedFilter.StringIsNotEmpty())
               _dialogCreator.MessageBoxInfo(PKSimConstants.Information.ExpressionQueryFilterDiscarded(discardedFilter));
         }

         if (_query.Filter != null)
            expressionDataPresenter.Filter = _query.Filter;
      }

      protected override void Cleanup()
      {
         try
         {
            _mappingPresenter.Dispose();
         }
         finally
         {
            base.Cleanup();
         }
      }
   }
}
