using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Core.Serialization;
using OSPSuite.Utility.Extensions;

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

      void SelectTransferData(string selectedUnit, string defaultUnit);

      string Title { set; }

      bool Start();
   }

   public class ProteinExpressionsPresenter : PKSimWizardPresenter<IProteinExpressionsView, IProteinExpressionsPresenter, IExpressionItemPresenter>, IProteinExpressionsPresenter
   {
      private readonly IGeneExpressionQueries _geneExpressionQueries;
      private readonly IMappingPresenter _mappingPresenter;
      private readonly IProteinExpressionDataHelper _dataHelper;
      private DataSet _expressionDataSet;
      private string _proteinName;
      private QueryExpressionSettings _querySettings;
      private QueryExpressionResults _queryExpressionResults;

      public ProteinExpressionsPresenter(IProteinExpressionsView view, ISubPresenterItemManager<IExpressionItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator,
         IGeneExpressionQueries geneExpressionQueries, IMappingPresenter mappingPresenter, IProteinExpressionDataHelper dataHelper)
         : base(view, subPresenterItemManager, ExpressionItems.All, dialogCreator)
      {
         _geneExpressionQueries = geneExpressionQueries;
         _mappingPresenter = mappingPresenter;
         _dataHelper = dataHelper;
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
         DataTable newData = _dataHelper.CreateDataJoin(_expressionDataSet.Relations[0], JoinType.Inner, "ExpressionData");
         newData = joinExpressionDataWithContainers(newData);
         PresenterAt(ExpressionItems.ExpressionData).ActualizeData(newData);
      }

      /// <summary>
      ///    This method is called to display a mapping view to enable the user to change the container-tissue mapping.
      /// </summary>
      public void EditMapping()
      {
         DataTable mappingTable = _expressionDataSet.Tables[DatabaseConfiguration.TableNames.MAPPING_DATA];
         DataTable containerTable = getContainerTableFromQuerySettings();
         DataTable expressionDataTable = _expressionDataSet.Tables[DatabaseConfiguration.TableNames.EXPRESSION_DATA];
         _mappingPresenter.EditMapping(mappingTable, containerTable, expressionDataTable);
      }

      public override void WizardCurrent(int previousIndex, int newIndex)
      {
         if (newIndex == ExpressionItems.ExpressionData.Index &&
             PresenterAt(ExpressionItems.ProteinSelection).ProteinSelectionChanged)
            PresenterAt(ExpressionItems.ProteinSelection).SelectProtein();

         if (newIndex == ExpressionItems.ExpressionData.Index && previousIndex == ExpressionItems.Transfer.Index)
         {
            PresenterAt(ExpressionItems.ExpressionData).SetSelectedUnit(
               PresenterAt(ExpressionItems.Transfer).GetSelectedUnit());
         }

         if (newIndex == ExpressionItems.Transfer.Index)
         {
            if (PresenterAt(ExpressionItems.ProteinSelection).ProteinSelectionChanged)
               PresenterAt(ExpressionItems.ProteinSelection).SelectProtein();
            SelectTransferData(String.Empty, PresenterAt(ExpressionItems.ExpressionData).GetSelectedUnit());
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
         var expressionDataTable = _geneExpressionQueries.GetExpressionDataByGeneId(id);
         expressionDataTable.TableName = DatabaseConfiguration.TableNames.EXPRESSION_DATA;

         var mappingContainerTissue = _geneExpressionQueries.GetContainerTissueMapping();
         mappingContainerTissue.TableName = DatabaseConfiguration.TableNames.MAPPING_DATA;

         //This is required because the code below sets null value in a table that does not allow null value in the database
         mappingContainerTissue.Columns[DatabaseConfiguration.MappingColumns.COL_CONTAINER].AllowDBNull = true;


         //add all tissues in expression data which are not mapped to containers
         var mappedTissues = _dataHelper.GetDistinctLoV(mappingContainerTissue.Columns[DatabaseConfiguration.MappingColumns.COL_TISSUE]);
         var tissueLov = _dataHelper.GetDistinctLoV(expressionDataTable.Columns[DatabaseConfiguration.ExpressionDataColumns.COL_TISSUE]);

         mappingContainerTissue.BeginLoadData();
         foreach (string expressionTissue in tissueLov)
         {
            if (mappedTissues.Contains(expressionTissue)) continue;
            var newRow = mappingContainerTissue.NewRow();
            newRow[DatabaseConfiguration.MappingColumns.COL_CONTAINER] = DBNull.Value;
            newRow[DatabaseConfiguration.MappingColumns.COL_TISSUE] = expressionTissue;
            mappingContainerTissue.Rows.Add(newRow);
         }
         mappingContainerTissue.EndLoadData();
         mappingContainerTissue.AcceptChanges();

         //build data set
         _expressionDataSet = new DataSet("ExpressionData");
         _expressionDataSet.Tables.Add(expressionDataTable);
         _expressionDataSet.Tables.Add(mappingContainerTissue);
         DataColumn parentColumn = expressionDataTable.Columns[DatabaseConfiguration.ExpressionDataColumns.COL_TISSUE];
         DataColumn childColumn = mappingContainerTissue.Columns[DatabaseConfiguration.MappingColumns.COL_TISSUE];
         _expressionDataSet.Relations.Add("REL_TISSUE", parentColumn, childColumn, false);
         DataTable expressionData = _dataHelper.CreateDataJoin(_expressionDataSet.Relations[0], JoinType.Inner,
            "ExpressionData");

         // join expression data with containers to have the display name information
         expressionData = joinExpressionDataWithContainers(expressionData);

         // determine the name used for identifying the protein.
         if (selectedRow[DatabaseConfiguration.ProteinColumns.COL_SYMBOL] != DBNull.Value)
            _proteinName = (string) selectedRow[DatabaseConfiguration.ProteinColumns.COL_SYMBOL];
         else if (selectedRow[DatabaseConfiguration.ProteinColumns.COL_GENE_NAME] != DBNull.Value)
            _proteinName = (string) selectedRow[DatabaseConfiguration.ProteinColumns.COL_GENE_NAME];
         else if (selectedRow[DatabaseConfiguration.ProteinColumns.COL_GENE_ID] != DBNull.Value)
            _proteinName = (string) selectedRow[DatabaseConfiguration.ProteinColumns.COL_GENE_ID];

         PresenterAt(ExpressionItems.ExpressionData).SetData(_proteinName, expressionData, String.Empty);
         _view.ActivateControl(ExpressionItems.ExpressionData);
         _view.SetControlEnabled(ExpressionItems.Transfer, true);
         SetWizardButtonEnabled(ExpressionItems.ExpressionData);
      }

      private DataTable joinExpressionDataWithContainers(DataTable expressionData)
      {
         var ds = new DataSet();
         DataTable containers = getContainerTableFromQuerySettings();
         ds.Tables.Add(containers);
         ds.Tables.Add(expressionData);
         ds.Relations.Add("REL_CONTAINER", expressionData.Columns[DatabaseConfiguration.MappingColumns.COL_CONTAINER],
            containers.Columns[DatabaseConfiguration.MappingColumns.COL_CONTAINER], false);

         return _dataHelper.CreateDataJoin(ds.Relations[0], JoinType.Inner,
            "ExpressionData");
      }

      private bool isOldQuery
      {
         get { return !string.IsNullOrEmpty(_querySettings.QueryConfiguration); }
      }

      public void InitializeSettings(QueryExpressionSettings querySettings)
      {
         _querySettings = querySettings;
         PresenterAt(ExpressionItems.Transfer).ShowOldValues = isOldQuery;

         if (isOldQuery)
         {
            setQueryConfiguration(_querySettings.QueryConfiguration);
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
         var expResults = new List<ExpressionResult>();

         string selectedUnit = PresenterAt(ExpressionItems.Transfer).GetSelectedUnit();
         SelectTransferData(selectedUnit, selectedUnit);
         DataTable transferData = PresenterAt(ExpressionItems.Transfer).GetData();

         foreach (DataRow row in transferData.Rows)
         {
            string containerName = row[ColumnNamesOfTransferTable.Container.ToString()].ToString();

            double expRelValue;
            if (row[ColumnNamesOfTransferTable.RelativeExpressionNew.ToString()] == DBNull.Value)
               expRelValue = 0;
            else
               expRelValue = (double) row[ColumnNamesOfTransferTable.RelativeExpressionNew.ToString()];

            var expResult = new ExpressionResult {ContainerName = containerName, RelativeExpression = expRelValue};
            expResults.Add(expResult);
         }

         _queryExpressionResults = new QueryExpressionResults(expResults)
         {
            ProteinName = _proteinName,
            SelectedUnit = selectedUnit,
            QueryConfiguration = getQueryConfiguration(),
            Description = getQueryDescription()
         };
         return _queryExpressionResults;
      }

      private string getQueryDescription()
      {
         var description = new StringBuilder();
         description.AppendLine($"Selected protein: {_proteinName}");
         var selectedUnit = PresenterAt(ExpressionItems.Transfer).GetSelectedUnit();
         description.AppendLine($"Selected unit: {selectedUnit}");
         var filterInfo = PresenterAt(ExpressionItems.ExpressionData).GetFilterInformation();
         if (!String.IsNullOrEmpty(filterInfo))
         {
            description.AppendLine("Filter used: ");
            description.AppendLine(filterInfo);
         }
         var mappingTable = _expressionDataSet.Tables[DatabaseConfiguration.TableNames.MAPPING_DATA];
         if (mappingTable != null)
         {
            description.AppendLine("Mapping used: ");
            foreach (DataRow row in mappingTable.Rows)
            {
               var tissue = row[DatabaseConfiguration.MappingColumns.COL_TISSUE].ToString();
               if (String.IsNullOrEmpty(tissue)) continue;
               var container = row[DatabaseConfiguration.MappingColumns.COL_CONTAINER].ToString();
               if (String.IsNullOrEmpty(container)) continue;
               description.AppendLine($"Tissue [{tissue}] -> Container [{container}]");
            }
         }

         return description.ToString();
      }

      public void SelectTransferData(string selectedUnit, string defaultUnit)
      {
         var selectedData = PresenterAt(ExpressionItems.ExpressionData).GetSelectedData();
         var transferData = new DataTable();
         foreach (DataRow unitRow in selectedData.DefaultView.ToTable(true, new[] {ColumnNamesOfTransferTable.Unit.ToString()}).Rows)
         {
            var unit = unitRow[ColumnNamesOfTransferTable.Unit.ToString()].ToString();
            if (!String.IsNullOrEmpty(selectedUnit))
               if (unit != selectedUnit) continue;

            //filter on current unit and join with containers
            var expDataView = selectedData.DefaultView;
            expDataView.RowFilter = $"[{ColumnNamesOfTransferTable.Unit}] = '{unit}'";
            var expData = joinTransferDataWithContainers(expDataView.ToTable());
            //fill out unit for outer joined containers
            foreach (DataRow row in expData.Rows)
               row[ColumnNamesOfTransferTable.Unit.ToString()] = unit;
            transferData.Merge(expData);
         }
         transferData.AcceptChanges();

         PresenterAt(ExpressionItems.Transfer).SetData(transferData, defaultUnit);
         _view.ActivateControl(ExpressionItems.Transfer);
         SetWizardButtonEnabled(ExpressionItems.Transfer);
      }

      private DataTable joinTransferDataWithContainers(DataTable expData)
      {
         const string STR_TRANSFER_DATA = "TransferData";
         var transferDataSet = new DataSet(STR_TRANSFER_DATA);
         DataTable containers = getContainerTableFromQuerySettings();

         transferDataSet.Tables.Add(containers);
         transferDataSet.Tables.Add(expData);
         var parentColumn = containers.Columns[ColumnNamesOfTransferTable.DisplayName.ToString()];
         var childColumn = expData.Columns[ColumnNamesOfTransferTable.Container.ToString()];
         const string STR_REL_CONTAINER = "REL_CONTAINER";
         transferDataSet.Relations.Add(STR_REL_CONTAINER, parentColumn, childColumn, false);

         return _dataHelper.CreateDataJoin(transferDataSet.Relations[STR_REL_CONTAINER],
            JoinType.LeftOuter,
            STR_TRANSFER_DATA);
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

      private void setExpressionDataSet(string expressionDataSet)
      {
         _expressionDataSet = new DataSet();
         _expressionDataSet.ReadFromXmlString(expressionDataSet);
      }

      private string getExpressionDataSet()
      {
         return _expressionDataSet.SaveToXmlString();
      }

      private string getQueryConfiguration()
      {
         var element = new XElement("QueryConfiguration");
         element.Add(new XAttribute(CoreConstants.Serialization.Attribute.ProteinName, _proteinName));
         var selectedUnit = PresenterAt(ExpressionItems.Transfer).GetSelectedUnit();
         element.Add(new XAttribute(CoreConstants.Serialization.Attribute.SelectedUnit, selectedUnit));
         element.Add(new XElement(CoreConstants.Serialization.ExpressionDataSet, getExpressionDataSet()));
         element.Add(new XElement(CoreConstants.Serialization.LayoutSettings, PresenterAt(ExpressionItems.ExpressionData).GetLayoutSetting()));
         return element.ToString(SaveOptions.DisableFormatting);
      }

      private void setQueryConfiguration(string xml)
      {
         var rootElement = XElementSerializer.PermissiveLoad(new MemoryStream(Encoding.Default.GetBytes(xml)));

         var expressionDataSetElement = rootElement.Element(CoreConstants.Serialization.ExpressionDataSet);
         if (expressionDataSetElement == null)
            throw new PKSimException("XML Element ExpressionDataSet missing!");
         setExpressionDataSet(expressionDataSetElement.Value);

         var layoutSettingsElement = rootElement.Element(CoreConstants.Serialization.LayoutSettings);
         if (layoutSettingsElement == null)
            throw new PKSimException("XML Element LayoutSettings missing!");
         string layoutSettings = layoutSettingsElement.Value;

         var selectedUnitElement = rootElement.Attribute(CoreConstants.Serialization.Attribute.SelectedUnit);
         var selectedUnit = selectedUnitElement == null ? String.Empty : selectedUnitElement.Value;

         var proteinNameElement = rootElement.Attribute(CoreConstants.Serialization.Attribute.ProteinName);
         if (proteinNameElement == null)
            throw new PKSimException("XML Element ProteinName missing!");
         _proteinName = proteinNameElement.Value;

         var expressionData = _dataHelper.CreateDataJoin(_expressionDataSet.Relations[0],
            JoinType.Inner,
            "ExpressionData");
         expressionData = joinExpressionDataWithContainers(expressionData);

         PresenterAt(ExpressionItems.ExpressionData).SetData(_proteinName, expressionData, selectedUnit);
         PresenterAt(ExpressionItems.ExpressionData).SetLayoutSetting(layoutSettings);
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