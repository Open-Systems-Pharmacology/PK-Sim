namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class MultipleNumericFieldsView
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
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.btnMoveDown = new DevExpress.XtraEditors.SimpleButton();
         this.btnMoveUp = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemFields = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonUp = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonDown = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFields)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonUp)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonDown)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         this.SuspendLayout();
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(2, 19);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(207, 244);
         this.gridControl.TabIndex = 0;
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
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.btnMoveDown);
         this.layoutControl.Controls.Add(this.btnMoveUp);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(301, 265);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(2, 2);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 6;
         this.lblDescription.Text = "lblDescription";
         // 
         // btnMoveDown
         // 
         this.btnMoveDown.Location = new System.Drawing.Point(213, 45);
         this.btnMoveDown.Name = "btnMoveDown";
         this.btnMoveDown.Size = new System.Drawing.Size(86, 22);
         this.btnMoveDown.StyleController = this.layoutControl;
         this.btnMoveDown.TabIndex = 5;
         this.btnMoveDown.Text = "btnMoveDown";
         // 
         // btnMoveUp
         // 
         this.btnMoveUp.Location = new System.Drawing.Point(213, 19);
         this.btnMoveUp.Name = "btnMoveUp";
         this.btnMoveUp.Size = new System.Drawing.Size(86, 22);
         this.btnMoveUp.StyleController = this.layoutControl;
         this.btnMoveUp.TabIndex = 4;
         this.btnMoveUp.Text = "btnMoveUp";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemFields,
            this.layoutItemButtonUp,
            this.layoutItemButtonDown,
            this.emptySpaceItem1,
            this.layoutItemDescription});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(301, 265);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemFields
         // 
         this.layoutItemFields.Control = this.gridControl;
         this.layoutItemFields.CustomizationFormText = "layoutControlItem1";
         this.layoutItemFields.Location = new System.Drawing.Point(0, 17);
         this.layoutItemFields.Name = "layoutItemFields";
         this.layoutItemFields.Size = new System.Drawing.Size(211, 248);
         this.layoutItemFields.Text = "layoutItemFields";
         this.layoutItemFields.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemFields.TextToControlDistance = 0;
         this.layoutItemFields.TextVisible = false;
         // 
         // layoutItemButtonUp
         // 
         this.layoutItemButtonUp.Control = this.btnMoveUp;
         this.layoutItemButtonUp.CustomizationFormText = "layoutItemButtonUp";
         this.layoutItemButtonUp.Location = new System.Drawing.Point(211, 17);
         this.layoutItemButtonUp.Name = "layoutItemButtonUp";
         this.layoutItemButtonUp.Size = new System.Drawing.Size(90, 26);
         this.layoutItemButtonUp.Text = "layoutItemButtonUp";
         this.layoutItemButtonUp.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonUp.TextToControlDistance = 0;
         this.layoutItemButtonUp.TextVisible = false;
         // 
         // layoutItemButtonDown
         // 
         this.layoutItemButtonDown.Control = this.btnMoveDown;
         this.layoutItemButtonDown.CustomizationFormText = "layoutItemButtonDown";
         this.layoutItemButtonDown.Location = new System.Drawing.Point(211, 43);
         this.layoutItemButtonDown.Name = "layoutItemButtonDown";
         this.layoutItemButtonDown.Size = new System.Drawing.Size(90, 26);
         this.layoutItemButtonDown.Text = "layoutItemButtonDown";
         this.layoutItemButtonDown.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonDown.TextToControlDistance = 0;
         this.layoutItemButtonDown.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(211, 69);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(90, 196);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutControlItem2";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(301, 17);
         this.layoutItemDescription.Text = "layoutItemDescription";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextToControlDistance = 0;
         this.layoutItemDescription.TextVisible = false;
         // 
         // MultipleNumericFieldsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "MultipleNumericFieldsView";
         this.Size = new System.Drawing.Size(301, 265);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFields)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonUp)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonDown)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SimpleButton btnMoveDown;
      private DevExpress.XtraEditors.SimpleButton btnMoveUp;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemFields;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonUp;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonDown;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
   }
}
