using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   partial class SimulationSelectionForComparisonView
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         _gridViewBinder.Dispose();
         _screenBinder.Dispose();
         _groupingItemBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblReferenceDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbSymbol = new DevExpress.XtraEditors.ComboBoxEdit();
         this.colorSelection = new UxColorPickEditWithHistory();
         this.tbLabel = new DevExpress.XtraEditors.TextEdit();
         this.cbReferenceSimulation = new DevExpress.XtraEditors.ComboBoxEdit();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemGridView = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupReferenceSimulation = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemReferenceSimulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLabel = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemColorSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSymbol = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbSymbol.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorSelection.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbLabel.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbReferenceSimulation.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupReferenceSimulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemReferenceSimulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLabel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemColorSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSymbol)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.lblReferenceDescription);
         this.layoutControl.Controls.Add(this.cbSymbol);
         this.layoutControl.Controls.Add(this.colorSelection);
         this.layoutControl.Controls.Add(this.tbLabel);
         this.layoutControl.Controls.Add(this.cbReferenceSimulation);
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(504, 347);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblReferenceDescription
         // 
         this.lblReferenceDescription.Location = new System.Drawing.Point(24, 214);
         this.lblReferenceDescription.Name = "lblReferenceDescription";
         this.lblReferenceDescription.Size = new System.Drawing.Size(113, 13);
         this.lblReferenceDescription.StyleController = this.layoutControl;
         this.lblReferenceDescription.TabIndex = 11;
         this.lblReferenceDescription.Text = "lblReferenceDescription";
         // 
         // cbSymbol
         // 
         this.cbSymbol.Location = new System.Drawing.Point(177, 303);
         this.cbSymbol.Name = "cbSymbol";
         this.cbSymbol.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSymbol.Size = new System.Drawing.Size(303, 20);
         this.cbSymbol.StyleController = this.layoutControl;
         this.cbSymbol.TabIndex = 10;
         // 
         // colorSelection
         // 
         this.colorSelection.EditValue = System.Drawing.Color.Empty;
         this.colorSelection.Location = new System.Drawing.Point(177, 279);
         this.colorSelection.Name = "colorSelection";
         this.colorSelection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.colorSelection.Size = new System.Drawing.Size(303, 20);
         this.colorSelection.StyleController = this.layoutControl;
         this.colorSelection.TabIndex = 9;
         // 
         // tbLabel
         // 
         this.tbLabel.Location = new System.Drawing.Point(177, 255);
         this.tbLabel.Name = "tbLabel";
         this.tbLabel.Size = new System.Drawing.Size(303, 20);
         this.tbLabel.StyleController = this.layoutControl;
         this.tbLabel.TabIndex = 8;
         // 
         // cbReferenceSimulation
         // 
         this.cbReferenceSimulation.Location = new System.Drawing.Point(177, 231);
         this.cbReferenceSimulation.Name = "cbReferenceSimulation";
         this.cbReferenceSimulation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbReferenceSimulation.Size = new System.Drawing.Size(303, 20);
         this.cbReferenceSimulation.StyleController = this.layoutControl;
         this.cbReferenceSimulation.TabIndex = 7;
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 12);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 6;
         this.lblDescription.Text = "lblDescription";
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(12, 29);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(480, 150);
         this.gridControl.TabIndex = 4;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.AllowsFiltering = true;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.MultiSelect = true;
         this.gridView.Name = "gridView";
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemGridView,
            this.layoutItemDescription,
            this.layoutGroupReferenceSimulation});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(504, 347);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemGridView
         // 
         this.layoutItemGridView.Control = this.gridControl;
         this.layoutItemGridView.CustomizationFormText = "layoutControlItem1";
         this.layoutItemGridView.Location = new System.Drawing.Point(0, 17);
         this.layoutItemGridView.Name = "layoutItemGridView";
         this.layoutItemGridView.Size = new System.Drawing.Size(484, 154);
         this.layoutItemGridView.Text = "layoutControlItem1";
         this.layoutItemGridView.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemGridView.TextToControlDistance = 0;
         this.layoutItemGridView.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutControlItem2";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(484, 17);
         this.layoutItemDescription.Text = "layoutControlItem2";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextToControlDistance = 0;
         this.layoutItemDescription.TextVisible = false;
         // 
         // layoutGroupReferenceSimulation
         // 
         this.layoutGroupReferenceSimulation.CustomizationFormText = "layoutGroupReferenceSimulation";
         this.layoutGroupReferenceSimulation.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemReferenceSimulation,
            this.layoutItemLabel,
            this.layoutItemColorSelection,
            this.layoutItemSymbol,
            this.layoutControlItem1});
         this.layoutGroupReferenceSimulation.Location = new System.Drawing.Point(0, 171);
         this.layoutGroupReferenceSimulation.Name = "layoutGroupReferenceSimulation";
         this.layoutGroupReferenceSimulation.Size = new System.Drawing.Size(484, 156);
         this.layoutGroupReferenceSimulation.Text = "layoutGroupReferenceSimulation";
         // 
         // layoutItemReferenceSimulation
         // 
         this.layoutItemReferenceSimulation.Control = this.cbReferenceSimulation;
         this.layoutItemReferenceSimulation.CustomizationFormText = "layoutItemReferenceSimulation";
         this.layoutItemReferenceSimulation.Location = new System.Drawing.Point(0, 17);
         this.layoutItemReferenceSimulation.Name = "layoutItemReferenceSimulation";
         this.layoutItemReferenceSimulation.Size = new System.Drawing.Size(460, 24);
         this.layoutItemReferenceSimulation.Text = "layoutItemReferenceSimulation";
         this.layoutItemReferenceSimulation.TextSize = new System.Drawing.Size(150, 13);
         // 
         // layoutItemLabel
         // 
         this.layoutItemLabel.Control = this.tbLabel;
         this.layoutItemLabel.CustomizationFormText = "layoutItemLabel";
         this.layoutItemLabel.Location = new System.Drawing.Point(0, 41);
         this.layoutItemLabel.Name = "layoutItemLabel";
         this.layoutItemLabel.Size = new System.Drawing.Size(460, 24);
         this.layoutItemLabel.Text = "layoutItemLabel";
         this.layoutItemLabel.TextSize = new System.Drawing.Size(150, 13);
         // 
         // layoutItemColorSelection
         // 
         this.layoutItemColorSelection.Control = this.colorSelection;
         this.layoutItemColorSelection.CustomizationFormText = "layoutItemColorSelection";
         this.layoutItemColorSelection.Location = new System.Drawing.Point(0, 65);
         this.layoutItemColorSelection.Name = "layoutItemColorSelection";
         this.layoutItemColorSelection.Size = new System.Drawing.Size(460, 24);
         this.layoutItemColorSelection.Text = "layoutItemColorSelection";
         this.layoutItemColorSelection.TextSize = new System.Drawing.Size(150, 13);
         // 
         // layoutItemSymbol
         // 
         this.layoutItemSymbol.Control = this.cbSymbol;
         this.layoutItemSymbol.CustomizationFormText = "layoutItemSymbol";
         this.layoutItemSymbol.Location = new System.Drawing.Point(0, 89);
         this.layoutItemSymbol.Name = "layoutItemSymbol";
         this.layoutItemSymbol.Size = new System.Drawing.Size(460, 24);
         this.layoutItemSymbol.Text = "layoutItemSymbol";
         this.layoutItemSymbol.TextSize = new System.Drawing.Size(150, 13);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.lblReferenceDescription;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(460, 17);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // SimulationSelectionForComparisonView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SimulationSelectionForComparisonView";
         this.ClientSize = new System.Drawing.Size(504, 393);
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationSelectionForComparisonView";
         this.Text = "SimulationSelectionForComparisonView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbSymbol.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorSelection.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbLabel.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbReferenceSimulation.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupReferenceSimulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemReferenceSimulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLabel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemColorSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSymbol)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGridView;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraEditors.ComboBoxEdit cbSymbol;
      private UxColorPickEditWithHistory colorSelection;
      private DevExpress.XtraEditors.TextEdit tbLabel;
      private DevExpress.XtraEditors.ComboBoxEdit cbReferenceSimulation;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemReferenceSimulation;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLabel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemColorSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSymbol;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupReferenceSimulation;
      private DevExpress.XtraEditors.LabelControl lblReferenceDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}