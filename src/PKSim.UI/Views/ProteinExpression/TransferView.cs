using System;
using System.Collections.Generic;
using System.Data;
using OSPSuite.Assets;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using PKSim.Assets;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.ProteinExpression
{
   internal partial class TransferView : BaseUserControl, ITransferView
   {
      private readonly IProteinExpressionToolTipCreator _toolTipCreator;
      private const string STR_Percentage = "Percentage";
      private DataTable _transferData;
      private string _selectedUnit;
      private ITransferPresenter _presenter;

      public TransferView(IProteinExpressionToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();

         grdTransfer.ToolTipController = new ToolTipController();
         grdTransfer.ToolTipController.GetActiveObjectInfo += OnGetActiveObjectInfo;

         radioGroup.SelectedIndexChanged += OnRadioGroupSelectedIndexChanged;

         //option settings
         var view = grdTransfer.MainView as Core.UxGridView;
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
         BindDataToGrid();
      }

      /// <summary>
      /// This function handle the event which takes care of tool tips.
      /// The code checks on which cell the mouse currently is positioned.
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
            if (hi.Column.FieldName == ColumnNamesOfTransferTable.RelativeExpressionNew.ToString() ||
                hi.Column.FieldName == string.Concat(ColumnNamesOfTransferTable.RelativeExpressionNew.ToString(), STR_Percentage))
            {
               string cellKey = string.Concat(hi.RowHandle.ToString(), "-", hi.Column.ColumnHandle.ToString());
               string text = view.GetRowCellDisplayText(hi.RowHandle, hi.Column);
               if (e.Info == null) e.Info = new ToolTipControlInfo(cellKey, text);
               e.Info.SuperTip = new SuperToolTip();
               e.Info.SuperTip.Items.AddTitle(string.Concat(ColumnNamesOfTransferTable.ExpressionValue.ToString(), ":"));
               e.Info.SuperTip.Items.Add(view.GetRowCellDisplayText(hi.RowHandle, ColumnNamesOfTransferTable.ExpressionValue.ToString()));
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
         dataView.Sort = ColumnNamesOfTransferTable.Unit.ToString();
         var units = new List<string>();
         foreach (DataRow unit in dataView.ToTable(true, new[] { ColumnNamesOfTransferTable.Unit.ToString() }).Rows)
         {
            var unitString = unit[ColumnNamesOfTransferTable.Unit.ToString()].ToString();
            if (String.IsNullOrEmpty(unitString)) continue;
            var newRadioItem = new RadioGroupItem {Description = unitString };

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

         BindDataToGrid();
         ConfigGridColumns();
      }

      private void BindDataToGrid()
      {
         var transferTable = _transferData.Copy();
         transferTable.Columns.Add(
            string.Concat(ColumnNamesOfTransferTable.RelativeExpressionOld.ToString(), STR_Percentage),
            typeof(double), string.Concat(ColumnNamesOfTransferTable.RelativeExpressionOld.ToString(), "*100"));
         transferTable.Columns.Add(
            string.Concat(ColumnNamesOfTransferTable.RelativeExpressionNew.ToString(), STR_Percentage),
            typeof(double), string.Concat(ColumnNamesOfTransferTable.RelativeExpressionNew.ToString(), "*100"));

         var bindData = transferTable.DefaultView;
         if (!String.IsNullOrEmpty(_selectedUnit))
            bindData.RowFilter = String.Format("[{0}] = '{1}'", ColumnNamesOfTransferTable.Unit,
                                              _selectedUnit);
         grdTransfer.BeginUpdate();
         grdTransfer.DataSource = null;
         grdTransfer.ResetBindings();
         grdTransfer.DataSource = bindData;
         grdTransfer.RefreshDataSource();
         ConfigGridColumns();
         grdTransfer.EndUpdate();
      }

      private void ConfigGridColumns()
      {
         var view = grdTransfer.MainView as GridView;
         if (view == null) return;
         var itemProgressBar = new RepositoryItemProgressBar
                                                        {
                                                           ReadOnly = true,
                                                           Enabled = false,
                                                           BorderStyle = BorderStyles.NoBorder,
                                                           Maximum = 100,
                                                           Minimum = 0,
                                                           NullText = string.Empty,
                                                           PercentView = false,
                                                           ShowTitle = true
                                                        };
         itemProgressBar.DisplayFormat.FormatType = FormatType.Numeric;
         itemProgressBar.CustomDisplayText += OnProgressBarCustomDisplayText;

         foreach (GridColumn col in view.Columns)
         {
            col.OptionsColumn.AllowEdit = false;
            if (col.FieldName == ColumnNamesOfTransferTable.Container.ToString())
            {
               col.Visible = true;
               col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.Transfer.COL_CONTAINER;
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
                     string container = dr[ColumnNamesOfTransferTable.Container.ToString()].ToString();
                     string displayName = dr[ColumnNamesOfTransferTable.DisplayName.ToString()].ToString();
                     ApplicationIcon icon = ApplicationIcons.IconByName(container);
                     if (icon != null && icon != ApplicationIcons.EmptyIcon)
                     {
                        smallImages.AddImage(icon.ToImage());
                        itemImageComboBox.Items.Add(new ImageComboBoxItem(displayName, container, smallImages.Images.Count-1));
                     } else 
                       itemImageComboBox.Items.Add(new ImageComboBoxItem(displayName, container));
                  }
               }
               itemImageComboBox.Items.EndUpdate();
               col.ColumnEdit = itemImageComboBox;
               col.SortOrder = ColumnSortOrder.Ascending;
            }
            else if (col.FieldName == ColumnNamesOfTransferTable.DisplayName.ToString())
            {
               col.Visible = false;
            }
            else if (col.FieldName == ColumnNamesOfTransferTable.RelativeExpressionOld.ToString())
            {
               col.Visible = false;
            }
            else if (col.FieldName == ColumnNamesOfTransferTable.ExpressionValue.ToString())
            {
               col.Visible = false;
            }
            else if (col.FieldName == ColumnNamesOfTransferTable.Unit.ToString())
            {
               col.Visible = false;
            }
            else if (col.FieldName == string.Concat(ColumnNamesOfTransferTable.RelativeExpressionOld.ToString(), STR_Percentage))
            {
               col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.Transfer.COL_OLDVALUE;
               col.Visible = _presenter.ShowOldValues;
               col.OptionsColumn.AllowEdit = false;
               col.ColumnEdit = itemProgressBar;
            }
            else if (col.FieldName == ColumnNamesOfTransferTable.RelativeExpressionNew.ToString())
            {
               col.Visible = false;
            }
            else if (col.FieldName == string.Concat(ColumnNamesOfTransferTable.RelativeExpressionNew.ToString(), STR_Percentage))
            {
               col.Caption = PKSimConstants.ProteinExpressions.ColumnCaptions.Transfer.COL_NEWVALUE;
               col.OptionsColumn.AllowEdit = false;
               col.ColumnEdit = itemProgressBar;
            }
         }
      }

      static void OnProgressBarCustomDisplayText(object sender, CustomDisplayTextEventArgs e)
      {
         try
         {
            if (e.Value == DBNull.Value || e.Value == null) e.DisplayText = string.Empty;
            else e.DisplayText = ((double) e.Value).ToString("F2");
         }
         catch (Exception )
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
         retData.RowFilter = $"[{ColumnNamesOfTransferTable.Unit}] = '{_selectedUnit}'";

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

      protected override int TopicId => HelpId.PKSim_Expression_ReviewGeneExpressionTransfer;
   }
}