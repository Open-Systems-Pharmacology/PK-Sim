using OSPSuite.Utility.Extensions;

namespace PKSim.UI.Views.Protocols
{
   partial class AdvancedProtocolView
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
         _gridProtocolBinder.Dispose();
         _comboBoxUnit.Dispose();
         _cache.DisposeAll();
         _screenBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
         DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
         this.gridViewSchemaItems = new PKSim.UI.Views.Core.UxGridView();
         this.gridProtocol = new OSPSuite.UI.Controls.UxGridControl();
         this.mainView = new PKSim.UI.Views.Core.UxGridView();
         this.gridViewDynamicParameters = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.cbTimeUnit = new DevExpress.XtraEditors.ComboBoxEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemTimeUnit = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewSchemaItems)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridProtocol)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewDynamicParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbTimeUnit.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTimeUnit)).BeginInit();
         this.SuspendLayout();
         // 
         // gridViewSchemaItems
         // 
         this.gridViewSchemaItems.AllowsFiltering = true;
         this.gridViewSchemaItems.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DodgerBlue;
         this.gridViewSchemaItems.Appearance.HeaderPanel.Options.UseBackColor = true;
         this.gridViewSchemaItems.EnableColumnContextMenu = true;
         this.gridViewSchemaItems.GridControl = this.gridProtocol;
         this.gridViewSchemaItems.MultiSelect = false;
         this.gridViewSchemaItems.Name = "gridViewSchemaItems";
         this.gridViewSchemaItems.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // gridProtocol
         // 
         gridLevelNode1.LevelTemplate = this.gridViewSchemaItems;
         gridLevelNode2.LevelTemplate = this.gridViewDynamicParameters;
         gridLevelNode2.RelationName = "DynamicParameters";
         gridLevelNode1.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode2});
         gridLevelNode1.RelationName = "SchemaItems";
         this.gridProtocol.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
         this.gridProtocol.Location = new System.Drawing.Point(2, 26);
         this.gridProtocol.MainView = this.mainView;
         this.gridProtocol.Name = "gridProtocol";
         this.gridProtocol.Size = new System.Drawing.Size(709, 517);
         this.gridProtocol.TabIndex = 1;
         this.gridProtocol.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.mainView,
            this.gridViewDynamicParameters,
            this.gridViewSchemaItems});
         // 
         // mainView
         // 
         this.mainView.AllowsFiltering = true;
         this.mainView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.PaleGreen;
         this.mainView.Appearance.HeaderPanel.Options.UseBackColor = true;
         this.mainView.EnableColumnContextMenu = true;
         this.mainView.GridControl = this.gridProtocol;
         this.mainView.MultiSelect = false;
         this.mainView.Name = "mainView";
         this.mainView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // gridViewDynamicParameters
         // 
         this.gridViewDynamicParameters.AllowsFiltering = true;
         this.gridViewDynamicParameters.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
         this.gridViewDynamicParameters.Appearance.HeaderPanel.Options.UseBackColor = true;
         this.gridViewDynamicParameters.EnableColumnContextMenu = true;
         this.gridViewDynamicParameters.GridControl = this.gridProtocol;
         this.gridViewDynamicParameters.MultiSelect = false;
         this.gridViewDynamicParameters.Name = "gridViewDynamicParameters";
         this.gridViewDynamicParameters.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.cbTimeUnit);
         this.layoutControl.Controls.Add(this.gridProtocol);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(713, 545);
         this.layoutControl.TabIndex = 2;
         this.layoutControl.Text = "layoutControl1";
         // 
         // cbTimeUnit
         // 
         this.cbTimeUnit.Location = new System.Drawing.Point(98, 2);
         this.cbTimeUnit.Name = "cbTimeUnit";
         this.cbTimeUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbTimeUnit.Size = new System.Drawing.Size(613, 20);
         this.cbTimeUnit.StyleController = this.layoutControl;
         this.cbTimeUnit.TabIndex = 7;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutItemTimeUnit});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(713, 545);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.gridProtocol;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(713, 521);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutItemTimeUnit
         // 
         this.layoutItemTimeUnit.Control = this.cbTimeUnit;
         this.layoutItemTimeUnit.CustomizationFormText = "layoutItemTimeUnit";
         this.layoutItemTimeUnit.Location = new System.Drawing.Point(0, 0);
         this.layoutItemTimeUnit.Name = "layoutItemTimeUnit";
         this.layoutItemTimeUnit.Size = new System.Drawing.Size(713, 24);
         this.layoutItemTimeUnit.TextSize = new System.Drawing.Size(93, 13);
         // 
         // AdvancedProtocolView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "AdvancedProtocolView";
         this.Size = new System.Drawing.Size(713, 545);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewSchemaItems)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridProtocol)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewDynamicParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbTimeUnit.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTimeUnit)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraGrid.GridControl gridProtocol;
      private PKSim.UI.Views.Core.UxGridView gridViewSchemaItems;
      private PKSim.UI.Views.Core.UxGridView mainView;
      private PKSim.UI.Views.Core.UxGridView gridViewDynamicParameters;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.ComboBoxEdit cbTimeUnit;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTimeUnit;

   }
}


