using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using DevExpress.Charts.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraPivotGrid;
using PKSim.Assets;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.ProteinExpression
{
   internal partial class ExpressionDataView : BaseUserControl, IExpressionDataView
   {
      private readonly IProteinExpressionToolTipCreator _toolTipCreator;
      private IExpressionDataPresenter _presenter;

      private PivotGridField _fieldVariantName;
      private PivotGridField _fieldDataBase;
      private PivotGridField _fieldDataBaseRecId;
      private PivotGridField _fieldGender;
      private PivotGridField _fieldTissue;
      private PivotGridField _fieldHealthState;
      private PivotGridField _fieldSampleSource;
      private PivotGridField _fieldAgeMin;
      private PivotGridField _fieldAgeMax;
      private PivotGridField _fieldSampleCount;
      private PivotGridField _fieldTotalCount;
      private PivotGridField _fieldRatio;
      private PivotGridField _fieldNormValue;
      private PivotGridField _fieldNormValue2;
      private PivotGridField _fieldUnit;
      private PivotGridField _fieldContainer;
      private PivotGridField _fieldAge;

      private string _selectedUnit;
      private bool _isFieldFilterChanging;
      private bool _isPrefilterCriteriaChanging;
      
      public ExpressionDataView(IImageListRetriever imageListRetriever, IProteinExpressionToolTipCreator toolTipCreator )
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();

         var toolTipController = new ToolTipController();
         toolTipController.Initialize(imageListRetriever);
         pgrdExpressionData.ToolTipController = toolTipController;

         //assign event handlers
         pgrdExpressionData.FieldFilterChanged += onFieldFilterChanged;
         pgrdExpressionData.PrefilterCriteriaChanged += onPrefilterCriteriaChanged;
         pgrdExpressionData.CustomUnboundFieldData += onCustomUnboundFieldData;
         pgrdExpressionData.DoubleClick += onDoubleClick;
         pgrdExpressionData.ToolTipController.GetActiveObjectInfo += onGetActiveObjectInfo;
         pgrdExpressionData.CustomSummary += onCustomSummary;
         pgrdExpressionData.FieldAreaChanging += onFieldAreaChanging;
         pgrdExpressionData.PopupMenuShowing += onPopupMenuShowing;
         ApplicationIcon = ApplicationIcons.Histogram;
      }


      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Histogram;
      }

      /// <summary>
      /// This event handler hides the unused reload data menu entry.
      /// </summary>
      private void onPopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
      {
         if (e.MenuType == PivotGridMenuType.Header || e.MenuType == PivotGridMenuType.HeaderArea)
         {
            e.Menu.Items[0].Visible = false;
            e.Menu.Items[1].BeginGroup = false;
            if (e.Field == _fieldUnit || e.Field == _fieldContainer || e.Field == _fieldNormValue || e.Field == _fieldNormValue2)
               e.Menu.Items[1].Visible = false;
         }
      }

      void onFieldAreaChanging(object sender, PivotAreaChangingEventArgs e)
      {
         e.Allow = (e.Field.FieldName != chrtExpressionData.SeriesTemplate.ArgumentDataMember &&
                    e.Field.FieldName != chrtExpressionData.SeriesDataMember);
      }

      /// <summary>
      /// This function handle the event which takes care of tool tips.
      /// The code checks on which area the mouse currently is positioned.
      /// Depending on the area and the field and the field value different tool tips are generated.
      /// </summary>
      /// <remarks>The object reference for a new ToolTipControlInfo object can be any unique string.</remarks>
      private void onGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != pgrdExpressionData) return;
         if (pgrdExpressionData.Fields.Count == 0) return;

         PivotGridHitInfo hi = pgrdExpressionData.CalcHitInfo(pgrdExpressionData.PointToClient(MousePosition));

         //Tool Tips
         if (hi.HitTest == PivotGridHitTest.HeadersArea)
         {
            #region HeaderFields

            if (hi.HeaderField != null)
            {
               if (e.Info == null)
               {
                  object o = string.Format("Header:{0}-{1}", hi.HeadersAreaInfo.Area, hi.HeadersAreaInfo.Field);
                  e.Info = new ToolTipControlInfo(o, hi.HeaderField.FieldName);
               }

               #region VariantName

               if (hi.HeaderField == _fieldVariantName)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForVariantHeader(_fieldVariantName.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion;
               #region Database

               else if (hi.HeaderField == _fieldDataBase)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForDataBaseHeader(_fieldDataBase.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region DatabaseRecId

               else if (hi.HeaderField == _fieldDataBaseRecId)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForDataBaseRecIdHeader(_fieldDataBaseRecId.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region Gender

               else if (hi.HeaderField == _fieldGender)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForGenderHeader(_fieldGender.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region Tissue

               else if (hi.HeaderField == _fieldTissue)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForTissueHeader(_fieldTissue.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region HealthState

               else if (hi.HeaderField == _fieldHealthState)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForTissueHeader(_fieldHealthState.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion;
               #region SampleSource

               else if (hi.HeaderField == _fieldSampleSource)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForSampleSourceHeader(_fieldSampleSource.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion;
               #region Age

               else if (hi.HeaderField == _fieldAge)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForAgeHeader(_fieldAge.Caption, _fieldAgeMin.Caption, _fieldAgeMax.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion;
               #region AgeMin

               else if (hi.HeaderField == _fieldAgeMin)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForAgeMinHeader(_fieldAgeMin.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region AgeMax

               else if (hi.HeaderField == _fieldAgeMax)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForAgeMaxHeader(_fieldAgeMax.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region Ratio

               else if (hi.HeaderField == _fieldRatio)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForRatioHeader(_fieldRatio.Caption, _fieldSampleCount.Caption, _fieldTotalCount.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region SampleCount

               else if (hi.HeaderField == _fieldSampleCount)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForSampleCountHeader(_fieldSampleCount.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region TotalCount

               else if (hi.HeaderField == _fieldTotalCount)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForTotalCountHeader(_fieldTotalCount.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region Unit

               else if (hi.HeaderField == _fieldUnit)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForUnitHeader(_fieldUnit.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
               #endregion
               #region Container

               else if (hi.HeaderField == _fieldContainer)
               {
                  e.Info.SuperTip = _toolTipCreator.GetTipForContainerHeader(_fieldContainer.Caption);
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }

               #endregion
            }

            #endregion
         }
         else if (hi.HitTest == PivotGridHitTest.Value)
         {
            #region ValueFields

            if (hi.ValueInfo != null)
            {
               if (e.Info == null)
               {
                  if (hi.ValueInfo.Value == null)
                  {
                     object o = string.Format("Value:{0}", hi.ValueInfo.Field);
                     e.Info = new ToolTipControlInfo(o, "");
                  }
                  else
                  {
                     object o = string.Format("Value:{0}-{1}", hi.ValueInfo.Field, hi.ValueInfo.Value);
                     e.Info = new ToolTipControlInfo(o, hi.ValueInfo.Value.ToString());
                  }
               }

               #region Variant Name

               if (hi.ValueInfo.Field == _fieldVariantName)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForVariant(_fieldVariantName.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }

               #endregion

               #region Data Base

               if (hi.ValueInfo.Field == _fieldDataBase)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForDatabase(_fieldDataBase.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }

               #endregion

               #region Data Base Rec ID

               if (hi.ValueInfo.Field == _fieldDataBaseRecId)
               {
                  // Get the database of that record id
                  PivotDrillDownDataSource ds = hi.ValueInfo.CreateDrillDownDataSource();
                  PivotDrillDownDataRow row = ds[0];
                  var database = (string)row[_fieldDataBase];

                  if (hi.ValueInfo.Value != null)
                  {
                     string recid = hi.ValueInfo.Value.ToString();
                     e.Info.SuperTip = _toolTipCreator.GetTipForDatabaseRecId(_fieldDataBaseRecId.Caption, database, recid);
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }
               #endregion
               #region Gender

               else if (hi.ValueInfo.Field == _fieldGender)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForGender(_fieldGender.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }
               #endregion
               #region Tissue
               else if (hi.ValueInfo.Field == _fieldTissue)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForTissue(_fieldTissue.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }
               #endregion
               #region Container
               else if (hi.ValueInfo.Field == _fieldContainer)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForContainer(_fieldContainer.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }
               #endregion
               #region Health State

               else if (hi.ValueInfo.Field == _fieldHealthState)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForHealthState(_fieldHealthState.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }
               #endregion
               #region Sample Source

               else if (hi.ValueInfo.Field == _fieldSampleSource)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForSampleSource(_fieldSampleSource.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }

               #endregion

               #region Age

               if (hi.ValueInfo.Field == _fieldAge)
               {
                  if (hi.ValueInfo.Value != null && hi.ValueInfo.Value.ToString().Length > 0)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForAge(_fieldAge.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }

               #endregion

               #region AgeMin

               if (hi.ValueInfo.Field == _fieldAgeMin)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForAgeMin(_fieldAgeMin.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }

               #endregion

               #region AgeMax

               if (hi.ValueInfo.Field == _fieldAgeMax)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForAgeMax(_fieldAgeMax.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }
               #endregion
               #region Unit

               else if (hi.ValueInfo.Field == _fieldUnit)
               {
                  if (hi.ValueInfo.Value != null)
                  {
                     e.Info.SuperTip = _toolTipCreator.GetTipForUnit(_fieldUnit.Caption, hi.ValueInfo.Value.ToString());
                     e.Info.ToolTipType = ToolTipType.SuperTip;
                  }
               }

               #endregion
            }

            #endregion
         }

         else if (hi.HitTest == PivotGridHitTest.Cell)
         {
            #region CellFields

            if (hi.CellInfo != null)
            {
               if (hi.CellInfo.Value != null)
               {
                  if (e.Info == null)
                  {
                     object o = string.Format("Cell:{0}-{1}-{2}-{3}", hi.CellInfo.RowField, hi.CellInfo.Item.RowIndex, hi.CellInfo.ColumnField, hi.CellInfo.Item.ColumnIndex);
                     e.Info = new ToolTipControlInfo(o, hi.CellInfo.Value.ToString());
                  }
                  e.Info.SuperTip = new SuperToolTip() {AllowHtmlText = DefaultBoolean.True};
                  e.Info.SuperTip.Items.AddTitle(hi.CellInfo.DataField.Caption);
                  e.Info.SuperTip.Items.Add(hi.CellInfo.Value.ToString());
                  e.Info.ToolTipType = ToolTipType.SuperTip;
               }
            }

            #endregion
         }
      }

      private void onDoubleClick(object sender, EventArgs e)
      {
         PivotGridHitInfo hi = pgrdExpressionData.CalcHitInfo(pgrdExpressionData.PointToClient(MousePosition));
         if (hi.ValueInfo != null)
         {
            if (hi.ValueInfo.Field == _fieldContainer || hi.ValueInfo.Field == _fieldTissue)
            {
               _presenter.EditMapping();
               return;
            }
         }

         if (hi.HeaderField == null) return;

         if (hi.HeaderField.Area == PivotArea.RowArea && !hi.HeaderField.HasNullValues)
         {
            //switch argument on x-axis in chart
            chrtExpressionData.SeriesTemplate.ArgumentDataMember = hi.HeaderField.FieldName;
            setTitleForXAxes(hi.HeaderField.HeaderDisplayText);
            chrtExpressionData.RefreshData();
            return;
         }

         if (hi.HeaderField.Area == PivotArea.ColumnArea && !hi.HeaderField.HasNullValues)
         {
            //switch argument on y-axis in chart
            chrtExpressionData.SeriesDataMember = hi.HeaderField.FieldName;
            chrtExpressionData.RefreshData();
            return;
         }

         if (hi.HeaderField.Area == PivotArea.FilterArea)
         {
            // reset fields to their default position
            chrtExpressionData.SeriesTemplate.ArgumentDataMember = _fieldContainer.FieldName;
            setTitleForXAxes(_fieldContainer.HeaderDisplayText);
            chrtExpressionData.SeriesDataMember = _fieldUnit.FieldName;
            chrtExpressionData.RefreshData();
            setFieldsToDefaultArea();
         }
      }

      private void setTitleForXAxes(string title)
      {
         var diagram = chrtExpressionData.Diagram as XYDiagram;
         if (diagram == null) return;
         diagram.AxisX.Title.Visibility = DefaultBoolean.True;
         diagram.AxisX.Title.Text = title;
      }

      private void onCustomUnboundFieldData(object sender, CustomFieldDataEventArgs e)
      {
         if (e.Field != _fieldAge) return;
         object ageMin = e.GetListSourceColumnValue(_fieldAgeMin.FieldName);
         object ageMax = e.GetListSourceColumnValue(_fieldAgeMax.FieldName);
         string ageMinValue = string.Empty;
         string ageMaxValue = string.Empty;

         if (ageMin != null) ageMinValue = ageMin.ToString();
         if (ageMax != null) ageMaxValue = ageMax.ToString();

         e.Value = (ageMinValue == ageMaxValue) ? ageMinValue : string.Format("{0} - {1}", ageMinValue, ageMaxValue);

         if (string.Empty.Equals(e.Value)) e.Value = "UNSPECIFIED";
      }

      public void AttachPresenter(IExpressionDataPresenter presenter)
      {
         _presenter = presenter;
      }

      public override string Caption
      {
         get { return PKSimConstants.ProteinExpressions.MainView.TabPageExpressionData; }
      }

      public void SetData(string proteinName, DataTable expressionDataTable, string selectedUnit)
      {
         _selectedUnit = selectedUnit;

         // reset pivot grid to supress side effects
         pgrdExpressionData.ForceInitialize();
         pgrdExpressionData.Fields.Clear();

         // reset chart to supress side effects
         chrtExpressionData.SeriesTemplate.NumericSummaryOptions.SummaryFunction = "";
         chrtExpressionData.ResetSummaryFunctions();
         pgrdExpressionData.DataSource = expressionDataTable;
         pgrdExpressionData.RefreshData();

         createPivotGridFields();
         configPivotGrid();
         configChart(proteinName);
      }

      public void SetLayoutSettings(string layoutSettings)
      {
         Stream  ms = new MemoryStream();
         var sw = new StreamWriter(ms);
         sw.Write(layoutSettings, 0, layoutSettings.Length);
         sw.Flush();
         ms.Position = 0;
         pgrdExpressionData.RestoreLayoutFromStream(ms);
         sw.Close();
         pgrdExpressionData.RefreshData();
      }

      public string GetLayoutSettings()
      {
         Stream ms = new MemoryStream();
         var sr = new StreamReader(ms);
         pgrdExpressionData.SaveLayoutToStream(ms);
         ms.Flush();
         ms.Position = 0;
         string layoutSettings = sr.ReadToEnd();
         sr.Close();
         return layoutSettings;       
      }

      public void ActualizeData(DataTable expressionDataTable)
      {
         pgrdExpressionData.BeginUpdate();
         // must be set to null first to avoid internal errors by the DevExpress pivot grid
         pgrdExpressionData.DataSource = null;
         pgrdExpressionData.DataSource = expressionDataTable;
         pgrdExpressionData.EndUpdate();
         pgrdExpressionData.RefreshData();
      }

      public string GetFilterInformation()
      {
         if (pgrdExpressionData == null) return String.Empty;
         if (pgrdExpressionData.Prefilter == null) return String.Empty;
         return pgrdExpressionData.Prefilter.CriteriaString;
      }

      public string GetMappingInformation()
      {
         if (pgrdExpressionData == null) return String.Empty;
         if (pgrdExpressionData.Prefilter == null) return String.Empty;
         return pgrdExpressionData.Prefilter.CriteriaString;
      }

      public void SetSelectedUnit(string unit)
      {
         _selectedUnit = unit;
      }

      public string GetSelectedUnit()
      {
         return _selectedUnit;
      }

      public DataTable GetSelectedData()
      {
         var ret = new DataTable("TransferData");
         ret.Columns.Add(ColumnNamesOfTransferTable.Container.ToString(), _fieldContainer.DataType);
         ret.Columns.Add(ColumnNamesOfTransferTable.ExpressionValue.ToString(), _fieldNormValue.DataType);
         ret.Columns.Add(ColumnNamesOfTransferTable.RelativeExpressionNew.ToString(), _fieldNormValue.DataType);
         ret.Columns.Add(ColumnNamesOfTransferTable.Unit.ToString(), _fieldUnit.DataType);

         if (chrtExpressionData.Series.Count == 0) return ret;

         string oldDataMember = chrtExpressionData.SeriesDataMember;
         string oldArgument = chrtExpressionData.SeriesTemplate.ArgumentDataMember;
         chrtExpressionData.SeriesDataMember = _fieldUnit.FieldName;
         chrtExpressionData.SeriesTemplate.ArgumentDataMember = _fieldContainer.FieldName;
         try
         {
            //get the data from the chart control because here we already have the wanted aggregation
            foreach (Series expSeries in chrtExpressionData.Series)
            {
               if (expSeries != null)
               {
                  double maximum = Double.NegativeInfinity;
                  foreach (SeriesPoint point in expSeries.Points)
                  {
                     double value = point.Values[0];
                     if (value <= maximum) continue;
                     maximum = value;
                  }
                  foreach (SeriesPoint point in expSeries.Points)
                  {
                     double value = point.Values[0];
                     var values = new ArrayList {point.Argument, value};
                     if (maximum == 0)
                        values.Add(0);
                     else
                        values.Add(value/maximum);
                     values.Add(expSeries.Name);
                     ret.Rows.Add(values.ToArray());
                  }
               }
            }
         }
         finally
         {
            chrtExpressionData.SeriesDataMember = oldDataMember;
            chrtExpressionData.SeriesTemplate.ArgumentDataMember = oldArgument;
         }
         return ret;
      }

      private SeriesPoint[] customSummary(Series series, object argument, string[] functionArguments, 
         DataSourceValues[] values, object[] colors)
      {
         double sumCount = 0;

         for (int i = 0; i < values.Length; i++)
         {
            double count = Convert.ToDouble(values[i][functionArguments[1]]);
            sumCount += count;
         }

         double weightedAverage = 0;
         // Calculate the resulting series points as weighted average.
         for (int i = 0; i < values.Length; i++)
         {
            double average = Convert.ToDouble(values[i][functionArguments[0]]);
            double count = Convert.ToDouble(values[i][functionArguments[1]]);
            weightedAverage += average*count/sumCount;
         }
         // Create an array of the resulting series points.
         var points = new SeriesPoint[1];
         points[0] = new SeriesPoint(argument, weightedAverage);

         // Return the result.
         return points;
      }

      private void configChart(string titleText)
      {
         ChartControl chart = chrtExpressionData;

         //prepare chart
         chart.Series.Clear();

         //assign axis
         chart.SeriesDataMember = _fieldUnit.FieldName;
         chart.SeriesTemplate.ArgumentDataMember = _fieldContainer.FieldName;
         setTitleForXAxes(_fieldContainer.HeaderDisplayText);
         chart.SeriesTemplate.ValueDataMembers.AddRange(new[]
                                                           {
                                                              _fieldNormValue.FieldName + "_" +
                                                              _fieldNormValue.SummaryType
                                                           });

         //set chart data
         PivotSummaryDataSource chartData = pgrdExpressionData.CreateSummaryDataSource();
         chart.DataSource = chartData;

         //set width of bars
         var chartBarView = chart.SeriesTemplate.View as SideBySideBarSeriesView;
         if (chartBarView != null) chartBarView.EqualBarWidth = true;

         // Create argument descriptions for the summary function.
         // Average
         var argument1Description =
            new SummaryFunctionArgumentDescription("Average", ScaleType.Numerical);
         // Count
         var argument2Description =
            new SummaryFunctionArgumentDescription("Count", ScaleType.Numerical);

         // Register the summary function in chart.
         const string STR_CUSTOM_SUMMARY_FUNCTION_NAME = "WeightedAverage";
         const string STR_CUSTOM_SUMMARY_FUNCTION_DISPLAY_NAME = "WeightedAverage";
         chart.RegisterSummaryFunction(STR_CUSTOM_SUMMARY_FUNCTION_NAME, STR_CUSTOM_SUMMARY_FUNCTION_DISPLAY_NAME, 1,
                                       new[] {argument1Description, argument2Description},
                                       customSummary);

         chart.SeriesTemplate.NumericSummaryOptions.SummaryFunction = 
            $"{STR_CUSTOM_SUMMARY_FUNCTION_NAME}([{_fieldNormValue.FieldName + "_" + _fieldNormValue.SummaryType}],[{_fieldNormValue2.FieldName + "_" + _fieldNormValue2.SummaryType}])";

         //config layout
         chart.SeriesTemplate.View = new SideBySideBarSeriesView();
         var barView = chart.SeriesTemplate.View as BarSeriesView;
         chart.Legend.EnableAntialiasing = DefaultBoolean.True;
         chart.Legend.Border.Thickness = 1;
         chart.SeriesSorting = SortingMode.Ascending;
         chart.SeriesTemplate.LabelsVisibility =DefaultBoolean.True;

         //Axis Layout
         barView.AxisX.Label.Angle = 270;
         barView.AxisX.Label.EnableAntialiasing = DefaultBoolean.True;
         barView.AxisX.Title.EnableAntialiasing = DefaultBoolean.True;

         barView.AxisY.Title.Text = "Expression";
         barView.AxisY.Title.Visibility = DefaultBoolean.True;
         barView.AxisY.Title.EnableAntialiasing = DefaultBoolean.True;

         var barLabel = chart.SeriesTemplate.Label as BarSeriesLabel;
         if (barLabel != null)
            chart.SeriesTemplate.LabelsVisibility =  DefaultBoolean.False;

         chart.Titles.Clear();
         var title = new ChartTitle
                        {
                           Text = titleText,
                           TextColor = Color.Blue,
                           EnableAntialiasing = DefaultBoolean.True,
                           Alignment = StringAlignment.Center
                        };
         chart.Titles.Add(title);

         chart.SelectionMode = ElementSelectionMode.Multiple;
         chart.ObjectSelected += onChartControlObjectSelected;
         chart.ObjectHotTracked += onChartControlObjectHotTracked;
      }

     

      private static void onChartControlObjectSelected(object sender, HotTrackEventArgs e)
      {
         if (e.HitInfo.Series == null) e.Cancel = true;
      }

      private static void onChartControlObjectHotTracked(object sender, HotTrackEventArgs e)
      {
         if (e.HitInfo.Series == null) e.Cancel = true;
      }

      private void createPivotGridFields()
      {
         pgrdExpressionData.Fields.Clear();
         var myData = pgrdExpressionData.DataSource as DataTable;

         if (myData != null)
         {
            foreach (DataColumn col in myData.Columns)
            {
               switch (col.ColumnName)
               {
                  case DatabaseConfiguration.ExpressionDataColumns.COL_VARIANT_NAME:
                     _fieldVariantName = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_VARIANT_NAME
                     };
                     pgrdExpressionData.Fields.Add(_fieldVariantName);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_DATA_BASE:
                     _fieldDataBase = new PivotGridField(col.ColumnName, PivotArea.ColumnArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_DATA_BASE
                     };
                     pgrdExpressionData.Fields.Add(_fieldDataBase);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_DATA_BASE_REC_ID:
                     _fieldDataBaseRecId = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_DATA_BASE_REC_ID
                     };
                     pgrdExpressionData.Fields.Add(_fieldDataBaseRecId);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_GENDER:
                     _fieldGender = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_GENDER
                     };
                     pgrdExpressionData.Fields.Add(_fieldGender);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_TISSUE:
                     _fieldTissue = new PivotGridField(col.ColumnName, PivotArea.RowArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_TISSUE
                     };
                     pgrdExpressionData.Fields.Add(_fieldTissue);
                     break;
                  case DatabaseConfiguration.MappingColumns.COL_CONTAINER:
                     _fieldContainer = new PivotGridField(ColumnNamesOfTransferTable.DisplayName.ToString(), PivotArea.RowArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_CONTAINER
                     };
                     pgrdExpressionData.Fields.Add(_fieldContainer);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_HEALTH_STATE:
                     _fieldHealthState = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_HEALTH_STATE
                     };
                     pgrdExpressionData.Fields.Add(_fieldHealthState);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_SAMPLE_SOURCE:
                     _fieldSampleSource = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_SAMPLE_SOURCE
                     };
                     pgrdExpressionData.Fields.Add(_fieldSampleSource);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_AGE_MIN:
                     _fieldAgeMin = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_AGE_MIN
                     };
                     pgrdExpressionData.Fields.Add(_fieldAgeMin);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_AGE_MAX:
                     _fieldAgeMax = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_AGE_MAX
                     };
                     pgrdExpressionData.Fields.Add(_fieldAgeMax);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_SAMPLE_COUNT:
                     _fieldSampleCount = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_SAMPLE_COUNT
                     };
                     pgrdExpressionData.Fields.Add(_fieldSampleCount);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_TOTAL_COUNT:
                     _fieldTotalCount = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_TOTAL_COUNT
                     };
                     pgrdExpressionData.Fields.Add(_fieldTotalCount);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_RATIO:
                     _fieldRatio = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_RATIO
                     };
                     pgrdExpressionData.Fields.Add(_fieldRatio);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_NORM_VALUE:
                     _fieldNormValue = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_NORM_VALUE
                     };
                     pgrdExpressionData.Fields.Add(_fieldNormValue);
                     _fieldNormValue2 = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_NORM_VALUE
                     };
                     pgrdExpressionData.Fields.Add(_fieldNormValue2);
                     break;
                  case DatabaseConfiguration.ExpressionDataColumns.COL_UNIT:
                     _fieldUnit = new PivotGridField(col.ColumnName, PivotArea.FilterArea)
                     {
                        Caption =
                           PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_UNIT
                     };
                     pgrdExpressionData.Fields.Add(_fieldUnit);
                     break;
               }
            }
         }

         //create an unbound field for the age grouping
         const string STR_AGE = "AGE";
         _fieldAge = new PivotGridField(STR_AGE, PivotArea.FilterArea)
                        {
                           UnboundType = UnboundColumnType.String,
                           UnboundFieldName = STR_AGE,
                           Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.ExpressionData.COL_AGE
                        };
         pgrdExpressionData.Fields.Add(_fieldAge);

         pgrdExpressionData.RefreshData();
      }

      private void configPivotGrid()
      {
         pgrdExpressionData.OptionsView.ShowColumnTotals = false;
         pgrdExpressionData.OptionsView.ShowColumnGrandTotals = false;
         pgrdExpressionData.OptionsView.ShowRowGrandTotals = false;
         pgrdExpressionData.OptionsView.ShowRowTotals = true;
         pgrdExpressionData.OptionsView.ShowFilterSeparatorBar = true;
         pgrdExpressionData.OptionsView.ShowDataHeaders = false;
         pgrdExpressionData.OptionsDataField.ColumnValueLineCount = 1;
         pgrdExpressionData.OptionsDataField.EnableFilteringByData = true;
         pgrdExpressionData.OptionsDataField.DataFieldVisible = false;
         pgrdExpressionData.OptionsBehavior.CopyToClipboardWithFieldValues = true;
         pgrdExpressionData.OptionsDataField.FieldNaming = DataFieldNaming.FieldName;
         pgrdExpressionData.OptionsHint.ShowCellHints = true;
         pgrdExpressionData.OptionsHint.ShowHeaderHints = true;
         pgrdExpressionData.OptionsHint.ShowValueHints = true;

         foreach (PivotGridField fld in pgrdExpressionData.Fields)
         {
            fld.Options.AllowEdit = false;
            fld.Options.AllowExpand = DefaultBoolean.False;
         }

         // Settings for freezing fields used by chart
         _fieldUnit.Options.AllowDrag = DefaultBoolean.False;
         _fieldContainer.Options.AllowDrag = DefaultBoolean.False;
         _fieldNormValue.Options.AllowDrag = DefaultBoolean.False;
         _fieldNormValue2.Options.AllowDrag = DefaultBoolean.False;

         // Settings for Data Fields
         const string NUMBER_FORMAT = "#0.000000;[#0.000000];0";

         _fieldNormValue.SummaryType = PivotSummaryType.Average;
         _fieldNormValue.CellFormat.FormatType = FormatType.Numeric;
         _fieldNormValue.CellFormat.FormatString = NUMBER_FORMAT;

         _fieldNormValue2.SummaryType = PivotSummaryType.Count;
         _fieldNormValue2.Options.ShowValues = false;

         // Cell format and summary function settings
         _fieldAge.CellFormat.FormatType = FormatType.None;
         _fieldAge.SummaryType = PivotSummaryType.Min;
         _fieldDataBaseRecId.CellFormat.FormatType = FormatType.None;
         _fieldDataBaseRecId.SummaryType = PivotSummaryType.Min;
         _fieldRatio.SummaryType = PivotSummaryType.Custom;
         _fieldRatio.CellFormat.FormatType = FormatType.Numeric;
         _fieldRatio.CellFormat.FormatString = "E"; // use scientific notation
         _fieldSampleCount.SummaryType = PivotSummaryType.Sum;
         _fieldSampleCount.CellFormat.FormatType = FormatType.Numeric;
         _fieldTotalCount.SummaryType = PivotSummaryType.Sum;
         _fieldTotalCount.CellFormat.FormatType = FormatType.Numeric;
         _fieldAgeMin.CellFormat.FormatType = FormatType.Numeric;
         _fieldAgeMin.SummaryType = PivotSummaryType.Min;
         _fieldAgeMax.CellFormat.FormatType = FormatType.Numeric;
         _fieldAgeMax.SummaryType = PivotSummaryType.Max;

         _fieldContainer.EmptyValueText = "Not Assigned";

         setFieldsToDefaultArea();
      }

      private void onCustomSummary(object sender, PivotGridCustomSummaryEventArgs e)
      {
         if (e.DataField != _fieldRatio) return;

         // Get the record set corresponding to the current cell.
         PivotDrillDownDataSource ds = e.CreateDrillDownDataSource();
         // Iterate through the records and calculate the sums.
         double sampleSum = 0;
         double totalSum = 0;
         for (int i = 0; i < ds.RowCount; i++)
         {
            PivotDrillDownDataRow row = ds[i];
            // Get the order's total sum.
            sampleSum += (double) row[_fieldSampleCount];
            totalSum += (double) row[_fieldTotalCount];
         }
         // Calculate the ratio.
         if (ds.RowCount > 0)
         {
            e.CustomValue = sampleSum/totalSum;
         }
      }

      private void setFieldsToDefaultArea()
      {
         // FilterArea
         _fieldTissue.Area = PivotArea.FilterArea;
         _fieldTissue.AreaIndex = 0;
         _fieldTissue.Visible = true;

         _fieldVariantName.Area = PivotArea.FilterArea;
         _fieldVariantName.AreaIndex = 1;
         _fieldVariantName.Visible = true;

         _fieldDataBaseRecId.Area = PivotArea.FilterArea;
         _fieldDataBaseRecId.AreaIndex = 2;
         _fieldDataBaseRecId.Visible = true;

         _fieldGender.Area = PivotArea.FilterArea;
         _fieldGender.AreaIndex = 3;
         _fieldGender.Visible = true;

         _fieldHealthState.Area = PivotArea.FilterArea;
         _fieldHealthState.AreaIndex = 4;
         _fieldHealthState.Visible = true;

         _fieldSampleSource.Area = PivotArea.FilterArea;
         _fieldSampleSource.AreaIndex = 5;
         _fieldSampleSource.Visible = true;

         _fieldAge.Area = PivotArea.FilterArea;
         _fieldAge.AreaIndex = 6;
         _fieldAge.Visible = true;

         _fieldAgeMin.Area = PivotArea.FilterArea;
         _fieldAgeMin.AreaIndex = -1;

         _fieldAgeMax.Area = PivotArea.FilterArea;
         _fieldAgeMax.AreaIndex = -1;

         _fieldSampleCount.Area = PivotArea.FilterArea;
         _fieldSampleCount.AreaIndex = -1;

         _fieldTotalCount.Area = PivotArea.FilterArea;
         _fieldTotalCount.AreaIndex = -1;

         _fieldRatio.Area = PivotArea.FilterArea;
         _fieldRatio.AreaIndex = -1;

         //RowArea
         _fieldContainer.Area = PivotArea.RowArea;
         _fieldContainer.AreaIndex = 0;
         _fieldContainer.Visible = true;

         //ColumnArea
         _fieldUnit.Area = PivotArea.ColumnArea;
         _fieldUnit.AreaIndex = 0;
         _fieldUnit.Visible = true;

         _fieldDataBase.Area = PivotArea.ColumnArea;
         _fieldDataBase.AreaIndex = 1;
         _fieldDataBase.Visible = true;

         //DataArea
         _fieldNormValue.Area = PivotArea.DataArea;
         _fieldNormValue.AreaIndex = 0;
         _fieldNormValue.Visible = true;
         _fieldNormValue2.Area = PivotArea.DataArea;
         _fieldNormValue2.AreaIndex = 1;
         _fieldNormValue2.Visible = true;
      }

      /// <summary>
      /// In this event all data filters are displayed as prefilters.
      /// </summary>
      /// <remarks>Has been adopted from DevExpress help page.
      /// http://www.devexpress.com/Support/Center/e/E1678.aspx </remarks>
      private void onFieldFilterChanged(object sender, PivotFieldEventArgs e)
      {
         if (_isPrefilterCriteriaChanging) return;
         _isFieldFilterChanging = true;

         try
         {
            var pivot = sender as PivotGridControl;
            if (pivot == null) return;

            var oldFilter = pivot.Prefilter.Criteria as GroupOperator;
            var field = e.Field;
            var filterValues = field.FilterValues;
            var rootGroup = new GroupOperator(GroupOperatorType.And);

            if (filterValues.HasFilter)
            {
               CriteriaOperator newFilter;
               if (filterValues.ValuesIncluded.Length > filterValues.ValuesExcluded.Length &&
                   filterValues.ValuesExcluded.Length > 0)
               {
                  newFilter = new InOperator(field.FieldName, filterValues.ValuesExcluded);
                  newFilter = new NotOperator(newFilter);
               }
               else
               {
                  newFilter = new InOperator(field.FieldName, filterValues.ValuesIncluded);
               }
               if (field.FilterValues.ShowBlanks)
               {
                  var newGroup = new GroupOperator(GroupOperatorType.Or);
                  newGroup.Operands.Add(newFilter);
                  newGroup.Operands.Add(new NullOperator(field.FieldName));
                  rootGroup = newGroup;
               }
               else
               {
                  rootGroup.Operands.Add(newFilter);
               }
               pgrdExpressionData.Prefilter.Enabled = true;
            }

            //try to remove all filters on current field
            if (!ReferenceEquals(oldFilter, null))
               removeCriteria(oldFilter, field.DataControllerColumnName, oldFilter);

            //add old filter
            if (!ReferenceEquals(oldFilter, null))
               if (oldFilter.Operands.Count > 0)
               {
                  rootGroup = new GroupOperator(GroupOperatorType.And, rootGroup);
                  rootGroup.Operands.Add(oldFilter);
               }

            //purge empty groups
            if (rootGroup.Operands.Count > 0)
            {
               var grop = rootGroup.Operands[0] as GroupOperator;
               while (!ReferenceEquals(grop, null) && grop.Operands.Count == 0)
               {
                  rootGroup.Operands.Remove(grop);
                  if (rootGroup.Operands.Count == 0) break;
                  grop = rootGroup.Operands[0] as GroupOperator;
               }
            }

            pivot.Prefilter.Criteria = rootGroup.Operands.Count > 0 ? rootGroup : null;
            pivot.RefreshData();
         }
         finally
         {
            _isFieldFilterChanging = false;
         }
      }

      /// <summary>
      /// Remove recursively all operands according to given fieldName.
      /// </summary>
      private static void removeCriteria(GroupOperator parent, String fieldName, CriteriaOperator criteria)
      {
         if (criteria is BinaryOperator)
         {
            var op = criteria as BinaryOperator;
            if (op.LeftOperand is OperandProperty)
            {
               var prob = op.LeftOperand as OperandProperty;
               if (prob.PropertyName == fieldName) parent.Operands.Remove(op);
            }
         }
         else if (criteria is UnaryOperator)
         {
            var op = criteria as UnaryOperator;
            if (op.Operand is OperandProperty)
            {
               var prob = op.Operand as OperandProperty;
               if (prob.PropertyName == fieldName) parent.Operands.Remove(op);
            }
            else if (op.Operand is InOperator)
            {
               var inOp = op.Operand as InOperator;
               if (inOp.LeftOperand is OperandProperty)
               {
                  var prob = inOp.LeftOperand as OperandProperty;
                  if (prob.PropertyName == fieldName) parent.Operands.Remove(op);
               }
            }
            else if (op.Operand is BinaryOperator)
            {
               var binOp = op.Operand as BinaryOperator;
               if (binOp.LeftOperand is OperandProperty)
               {
                  var prob = binOp.LeftOperand as OperandProperty;
                  if (prob.PropertyName == fieldName) parent.Operands.Remove(op);
               }
            }
         }
         else if (criteria is InOperator)
         {
            var op = criteria as InOperator;
            if (op.LeftOperand is OperandProperty)
            {
               var prob = op.LeftOperand as OperandProperty;
               if (prob.PropertyName == fieldName) parent.Operands.Remove(op);
            }
            else
            {
               removeCriteria(parent, fieldName, op.LeftOperand);
            }
         }
         else if (criteria is GroupOperator)
         {
            var op = criteria as GroupOperator;
            for (int i = op.Operands.Count - 1; i >= 0; i--) removeCriteria(op, fieldName, op.Operands[i]);

            if (op.Operands.Count == 0)
               if (!ReferenceEquals(parent, op)) parent.Operands.Remove(op);
         }
      }

      /// <summary>
      /// If the prefilter has been changed all field filters are resetted to no filter.
      /// </summary>
      private void onPrefilterCriteriaChanged(object sender, EventArgs e)
      {
         if (_isFieldFilterChanging) return;
         _isPrefilterCriteriaChanging = true;

         try
         {
            if (sender is PivotGridControl pivot)
            {
               foreach (PivotGridField fld in pivot.Fields)
                  fld.FilterValues.Clear();

               pivot.RefreshData();
            }
         }
         finally
         {
            _isPrefilterCriteriaChanging = false;
         }
      }
   }
}