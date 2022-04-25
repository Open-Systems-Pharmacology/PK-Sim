using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.UI.Controls;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using static PKSim.Assets.PKSimConstants.ProteinExpressions.ColumnCaptions.Transfer;
using static PKSim.Presentation.Presenters.ProteinExpression.ColumnNamesOfTransferTable;
using UxGridView = PKSim.UI.Views.Core.UxGridView;

namespace PKSim.UI.Views.ProteinExpression
{
   internal partial class TransferView : BaseUserControl, ITransferView
   {
      private readonly IProteinExpressionToolTipCreator _toolTipCreator;
      private DataTable _transferData;
      private string _selectedUnit;
      private ITransferPresenter _presenter;
      private readonly DoubleFormatter _doubleFormatter;

      public TransferView(IProteinExpressionToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         _doubleFormatter = new DoubleFormatter();
         InitializeComponent();

         grdTransfer.ToolTipController = new ToolTipController();
         grdTransfer.ToolTipController.GetActiveObjectInfo += OnGetActiveObjectInfo;

         radioGroup.SelectedIndexChanged += OnRadioGroupSelectedIndexChanged;
         radioGroup.Properties.AllowMouseWheel = false;

         //option settings
         var view = grdTransfer.MainView as UxGridView;
         if (view != null)
         {
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsClipboard.CopyColumnHeaders = DefaultBoolean.True;
            view.OptionsBehavior.ReadOnly = true;
            view.OptionsFilter.AllowFilterEditor = false;
            view.OptionsFilter.AllowColumnMRUFilterList = false;
            view.OptionsMenu.EnableColumnMenu = false;
            view.OptionsMenu.EnableGroupPanelMenu = false;
            view.OptionsCustomization.AllowColumnMoving = false;
            view.OptionsCustomization.AllowFilter = false;
            view.OptionsCustomization.AllowGroup = false;
            view.OptionsCustomization.AllowQuickHideColumns = false;
            view.OptionsCustomization.AllowRowSizing = false;
            view.OptionsCustomization.AllowSort = true;
            view.OptionsSelection.UseIndicatorForSelection = true;
            view.MultiSelect = true;
         }
      }

      void OnRadioGroupSelectedIndexChanged(object sender, EventArgs e)
      {
         _selectedUnit = radioGroup.Properties.Items[radioGroup.SelectedIndex].Description;
         bindDataToGrid();
      }

      /// <summary>
      ///    This function handle the event which takes care of tool tips.
      ///    The code checks on which cell the mouse currently is positioned.
      /// </summary>
      /// <remarks>The object reference for a new ToolTipControlInfo object can be any unique string.</remarks>
      private void OnGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != grdTransfer) return;
         var view = grdTransfer.FocusedView as GridView;
         if (view == null) return;
         if (view.Columns.Count == 0) return;

         GridHitInfo hi = view.CalcHitInfo(e.ControlMousePosition);

         //Tool Tips
         if (hi.InRowCell)
         {
            if (hi.Column.FieldName == RelativeExpressionNew ||
                hi.Column.FieldName == RelativeExpressionNewPercentage)
            {
               string cellKey = string.Concat(hi.RowHandle.ToString(), "-", hi.Column.ColumnHandle.ToString());
               string text = view.GetRowCellDisplayText(hi.RowHandle, hi.Column);
               if (e.Info == null) e.Info = new ToolTipControlInfo(cellKey, text);
               e.Info.SuperTip = new SuperToolTip();
               e.Info.SuperTip.Items.AddTitle(string.Concat(ExpressionValue, ":"));
               e.Info.SuperTip.Items.Add(view.GetRowCellDisplayText(hi.RowHandle, ExpressionValue));
               e.Info.ToolTipType = ToolTipType.SuperTip;
            }
         }
      }

      public void SetData(DataTable transferTable, string selectedUnit)
      {
         _transferData = transferTable;
         if (transferTable.Columns.Count == 0)
         {
            radioGroup.Properties.Items.Clear();
            grdTransfer.BeginUpdate();
            grdTransfer.DataSource = null;
            grdTransfer.RefreshDataSource();
            grdTransfer.EndUpdate();
            return;
         }

         radioGroup.Properties.Items.Clear();
         var dataView = _transferData.DefaultView;
         dataView.Sort = Unit;
         var units = new List<string>();
         foreach (DataRow unit in dataView.ToTable(true, new[] {Unit}).Rows)
         {
            var unitString = unit[Unit].ToString();
            if (String.IsNullOrEmpty(unitString)) continue;
            var newRadioItem = new RadioGroupItem {Description = unitString};

            radioGroup.Properties.Items.Add(newRadioItem);
            units.Add(unitString);
            if (unitString != selectedUnit) continue;
            radioGroup.SelectedIndex = radioGroup.Properties.Items.IndexOf(newRadioItem);
         }

         radioGroup.SuperTip = _toolTipCreator.GetTipForUnits(units.ToArray());
         if (radioGroup.Properties.Items.Count == 0)
            _selectedUnit = String.Empty;
         else
         {
            if (radioGroup.SelectedIndex < 0 || radioGroup.SelectedIndex >= radioGroup.Properties.Items.Count)
               radioGroup.SelectedIndex = 0;
            _selectedUnit = radioGroup.Properties.Items[radioGroup.SelectedIndex].Description;
         }

         bindDataToGrid();
      }

      private void bindDataToGrid()
      {
         var transferTable = _transferData.Copy();
         transferTable.Columns.Add(
            RelativeExpressionOldPercentage,
            typeof(double), string.Concat(RelativeExpressionOld, "*100"));
         transferTable.Columns.Add(
            RelativeExpressionNewPercentage,
            typeof(double), string.Concat(RelativeExpressionNew, "*100"));

         var bindData = transferTable.DefaultView;
         if (!string.IsNullOrEmpty(_selectedUnit))
            bindData.RowFilter = $"[{Unit}] = '{_selectedUnit}'";

         grdTransfer.BeginUpdate();
         grdTransfer.DataSource = null;
         grdTransfer.ResetBindings();
         grdTransfer.DataSource = bindData;
         grdTransfer.RefreshDataSource();
         configGridColumns(bindData);
         grdTransfer.EndUpdate();
      }

      private RepositoryItemBaseProgressBar initProgressBarEditor(int max)
      {
         return new RepositoryItemProgressBar
         {
            ReadOnly = true,
            Enabled = false,
            BorderStyle = BorderStyles.NoBorder,
            Maximum = max,
            Minimum = 0,
            NullText = string.Empty,
            PercentView = false,
            ShowTitle = true,
            DisplayFormat = {FormatType = FormatType.Numeric}
         };
      }

      private int getPercentageMax(DataView dataView, string columnName)
      {
         try
         {
            var allValues = dataView.Cast<DataRowView>()
               .Select(row => row[columnName])
               .Where(x => x != null && x != DBNull.Value)
               .Select(x => x.ConvertedTo<double>());

            return Convert.ToInt32(allValues.Max());
         }
         catch (Exception)
         {
            return 100;
         }
      }

      private void configGridColumns(DataView dataView)
      {
         var view = grdTransfer.MainView as GridView;
         if (view == null)
            return;

         var maxOldExpression = getPercentageMax(dataView, RelativeExpressionOldPercentage);
         var maxNewExpression = getPercentageMax(dataView, RelativeExpressionNewPercentage);

         var itemProgressBarOldExpression = initProgressBarEditor(maxOldExpression);
         itemProgressBarOldExpression.CustomDisplayText += OnProgressBarCustomDisplayText;

         var itemProgressBarNewExpression = initProgressBarEditor(maxNewExpression);
         itemProgressBarNewExpression.CustomDisplayText += OnProgressBarCustomDisplayText;

         foreach (GridColumn col in view.Columns)
         {
            col.OptionsColumn.AllowEdit = false;
            if (col.FieldName == ColumnNamesOfTransferTable.Container)
            {
               col.Visible = true;
               col.Caption = COL_CONTAINER;
               var itemImageComboBox = new RepositoryItemImageComboBox {ReadOnly = true};
               var smallImages = new ImageCollection();
               itemImageComboBox.SmallImages = smallImages;
               itemImageComboBox.Items.BeginUpdate();
               var dv = view.DataSource as DataView;
               if (dv != null)
               {
                  var dt = dv.ToTable();
                  foreach (DataRow dr in dt.Rows)
                  {
                     string container = dr[ColumnNamesOfTransferTable.Container].ToString();
                     string displayName = dr[DisplayName].ToString();
                     ApplicationIcon icon = ApplicationIcons.IconByName(container);
                     if (icon != null && icon != ApplicationIcons.EmptyIcon)
                     {
                        smallImages.AddImage(icon.ToImage());
                        itemImageComboBox.Items.Add(new ImageComboBoxItem(displayName, container, smallImages.Images.Count - 1));
                     }
                     else
                        itemImageComboBox.Items.Add(new ImageComboBoxItem(displayName, container));
                  }
               }

               itemImageComboBox.Items.EndUpdate();
               col.ColumnEdit = itemImageComboBox;
               col.SortOrder = ColumnSortOrder.Ascending;
            }
            else if (col.FieldName == DisplayName)
            {
               col.Visible = false;
            }
            else if (col.FieldName == RelativeExpressionOld)
            {
               col.Visible = false;
            }
            else if (col.FieldName == ExpressionValue)
            {
               col.Visible = false;
            }
            else if (col.FieldName == Unit)
            {
               col.Visible = false;
            }
            else if (col.FieldName == RelativeExpressionOldPercentage)
            {
               col.Caption = COL_OLDVALUE;
               col.Visible = _presenter.ShowOldValues;
               col.OptionsColumn.AllowEdit = false;
               col.ColumnEdit = itemProgressBarOldExpression;
            }
            else if (col.FieldName == RelativeExpressionNew)
            {
               col.Visible = false;
            }
            else if (col.FieldName == RelativeExpressionNewPercentage)
            {
               col.Caption = COL_NEWVALUE;
               col.OptionsColumn.AllowEdit = false;
               col.ColumnEdit = itemProgressBarNewExpression;
            }
         }
      }

      private void OnProgressBarCustomDisplayText(object sender, CustomDisplayTextEventArgs e)
      {
         try
         {
            if (e.Value == DBNull.Value || e.Value == null)
               e.DisplayText = string.Empty;
            else
               //divide by 100 again to ensure that we show values formatted between 0 and 1
               e.DisplayText = _doubleFormatter.Format(((double) e.Value) / 100);
         }
         catch (Exception)
         {
         }
      }

      public string GetSelectedUnit()
      {
         return _selectedUnit;
      }

      public bool HasData()
      {
         return _transferData != null && _transferData.Columns.Count > 0 && _transferData.Rows.Count > 0;
      }

      public DataTable GetData()
      {
         var retData = _transferData.DefaultView;
         retData.RowFilter = $"[{Unit}] = '{_selectedUnit}'";

         return retData.ToTable();
      }

      public void AttachPresenter(ITransferPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.ProteinExpressions.MainView.TabPageTransfer;
         ApplicationIcon = ApplicationIcons.Parameters;
      }
   }
}