namespace PKSim.UI.Views.Simulations
{
   partial class ImportPopulationSimulationView
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
         _screenBinder.Dispose();
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
         this.btnStartImport = new DevExpress.XtraEditors.SimpleButton();
         this.tbNumberOfIndividuals = new DevExpress.XtraEditors.TextEdit();
         this.tbLog = new DevExpress.XtraEditors.MemoEdit();
         this.tbPopulationFile = new DevExpress.XtraEditors.ButtonEdit();
         this.panelBuildingBlockSelection = new DevExpress.XtraEditors.PanelControl();
         this.rbPopulationSizeSelection = new System.Windows.Forms.RadioButton();
         this.rbPopulationFileSelection = new System.Windows.Forms.RadioButton();
         this.rbBuildingBockSelection = new System.Windows.Forms.RadioButton();
         this.tbSimulationFile = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlGroupSimulationFileSelection = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSimulationFileSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroupPopulationSelection = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemBuildingBlockSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPopulationFileSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNumberOfIndividuals = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemLog = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemButtonImport = new DevExpress.XtraLayout.LayoutControlItem();
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
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividuals.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbPopulationFile.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelBuildingBlockSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbSimulationFile.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupSimulationFileSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSimulationFileSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBuildingBlockSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulationFileSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividuals)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImport)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.btnStartImport);
         this.layoutControl.Controls.Add(this.tbNumberOfIndividuals);
         this.layoutControl.Controls.Add(this.tbLog);
         this.layoutControl.Controls.Add(this.tbPopulationFile);
         this.layoutControl.Controls.Add(this.panelBuildingBlockSelection);
         this.layoutControl.Controls.Add(this.rbPopulationSizeSelection);
         this.layoutControl.Controls.Add(this.rbPopulationFileSelection);
         this.layoutControl.Controls.Add(this.rbBuildingBockSelection);
         this.layoutControl.Controls.Add(this.tbSimulationFile);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(954, 235, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(641, 486);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl";
         // 
         // btnStartImport
         // 
         this.btnStartImport.Location = new System.Drawing.Point(322, 440);
         this.btnStartImport.Name = "btnStartImport";
         this.btnStartImport.Size = new System.Drawing.Size(295, 22);
         this.btnStartImport.StyleController = this.layoutControl;
         this.btnStartImport.TabIndex = 12;
         this.btnStartImport.Text = "btnStartImport";
         // 
         // tbNumberOfIndividuals
         // 
         this.tbNumberOfIndividuals.Location = new System.Drawing.Point(208, 251);
         this.tbNumberOfIndividuals.Name = "tbNumberOfIndividuals";
         this.tbNumberOfIndividuals.Size = new System.Drawing.Size(409, 20);
         this.tbNumberOfIndividuals.StyleController = this.layoutControl;
         this.tbNumberOfIndividuals.TabIndex = 11;
         // 
         // tbLog
         // 
         this.tbLog.Location = new System.Drawing.Point(24, 299);
         this.tbLog.Name = "tbLog";
         this.tbLog.Size = new System.Drawing.Size(593, 137);
         this.tbLog.StyleController = this.layoutControl;
         this.tbLog.TabIndex = 10;
         // 
         // tbPopulationFile
         // 
         this.tbPopulationFile.Location = new System.Drawing.Point(208, 198);
         this.tbPopulationFile.Name = "tbPopulationFile";
         this.tbPopulationFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.tbPopulationFile.Size = new System.Drawing.Size(409, 20);
         this.tbPopulationFile.StyleController = this.layoutControl;
         this.tbPopulationFile.TabIndex = 9;
         // 
         // panelBuildingBlockSelection
         // 
         this.panelBuildingBlockSelection.Location = new System.Drawing.Point(208, 139);
         this.panelBuildingBlockSelection.Name = "panelBuildingBlockSelection";
         this.panelBuildingBlockSelection.Size = new System.Drawing.Size(409, 26);
         this.panelBuildingBlockSelection.TabIndex = 8;
         // 
         // rbPopulationSizeSelection
         // 
         this.rbPopulationSizeSelection.Location = new System.Drawing.Point(24, 222);
         this.rbPopulationSizeSelection.Name = "rbPopulationSizeSelection";
         this.rbPopulationSizeSelection.Size = new System.Drawing.Size(593, 25);
         this.rbPopulationSizeSelection.TabIndex = 7;
         this.rbPopulationSizeSelection.TabStop = true;
         this.rbPopulationSizeSelection.Text = "rbPopulationSizeSelection";
         this.rbPopulationSizeSelection.UseVisualStyleBackColor = true;
         // 
         // rbPopulationFileSelection
         // 
         this.rbPopulationFileSelection.Location = new System.Drawing.Point(24, 169);
         this.rbPopulationFileSelection.Name = "rbPopulationFileSelection";
         this.rbPopulationFileSelection.Size = new System.Drawing.Size(593, 25);
         this.rbPopulationFileSelection.TabIndex = 6;
         this.rbPopulationFileSelection.TabStop = true;
         this.rbPopulationFileSelection.Text = "rbPopulationFileSelection";
         this.rbPopulationFileSelection.UseVisualStyleBackColor = true;
         // 
         // rbBuildingBockSelection
         // 
         this.rbBuildingBockSelection.Location = new System.Drawing.Point(24, 110);
         this.rbBuildingBockSelection.Name = "rbBuildingBockSelection";
         this.rbBuildingBockSelection.Size = new System.Drawing.Size(593, 25);
         this.rbBuildingBockSelection.TabIndex = 5;
         this.rbBuildingBockSelection.TabStop = true;
         this.rbBuildingBockSelection.Text = "rbBuildingBockSelection";
         this.rbBuildingBockSelection.UseVisualStyleBackColor = true;
         // 
         // tbSimulationFile
         // 
         this.tbSimulationFile.Location = new System.Drawing.Point(208, 43);
         this.tbSimulationFile.Name = "tbSimulationFile";
         this.tbSimulationFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.tbSimulationFile.Size = new System.Drawing.Size(409, 20);
         this.tbSimulationFile.StyleController = this.layoutControl;
         this.tbSimulationFile.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlGroupSimulationFileSelection,
            this.layoutControlGroupPopulationSelection,
            this.layoutControlGroup1});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(641, 486);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutControlGroupSimulationFileSelection
         // 
         this.layoutControlGroupSimulationFileSelection.CustomizationFormText = "layoutControlGroupSimulationFileSelection";
         this.layoutControlGroupSimulationFileSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSimulationFileSelection});
         this.layoutControlGroupSimulationFileSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroupSimulationFileSelection.Name = "layoutControlGroupSimulationFileSelection";
         this.layoutControlGroupSimulationFileSelection.Size = new System.Drawing.Size(621, 67);
         this.layoutControlGroupSimulationFileSelection.Text = "layoutControlGroupSimulationFileSelection";
         // 
         // layoutItemSimulationFileSelection
         // 
         this.layoutItemSimulationFileSelection.Control = this.tbSimulationFile;
         this.layoutItemSimulationFileSelection.CustomizationFormText = "layoutItemSimulationFile";
         this.layoutItemSimulationFileSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSimulationFileSelection.Name = "layoutItemSimulationFile";
         this.layoutItemSimulationFileSelection.Size = new System.Drawing.Size(597, 24);
         this.layoutItemSimulationFileSelection.Text = "layoutItemSimulationFile";
         this.layoutItemSimulationFileSelection.TextSize = new System.Drawing.Size(181, 13);
         // 
         // layoutControlGroupPopulationSelection
         // 
         this.layoutControlGroupPopulationSelection.CustomizationFormText = "layoutControlGroupPopulationSelection";
         this.layoutControlGroupPopulationSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutItemBuildingBlockSelection,
            this.layoutControlItem2,
            this.layoutItemPopulationFileSelection,
            this.layoutControlItem3,
            this.layoutItemNumberOfIndividuals});
         this.layoutControlGroupPopulationSelection.Location = new System.Drawing.Point(0, 67);
         this.layoutControlGroupPopulationSelection.Name = "layoutControlGroupPopulationSelection";
         this.layoutControlGroupPopulationSelection.Size = new System.Drawing.Size(621, 208);
         this.layoutControlGroupPopulationSelection.Text = "layoutControlGroupPopulationSelection";
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.rbBuildingBockSelection;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(597, 29);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutItemBuildingBlockSelection
         // 
         this.layoutItemBuildingBlockSelection.Control = this.panelBuildingBlockSelection;
         this.layoutItemBuildingBlockSelection.CustomizationFormText = "layoutItemPanelBuildingBlockSelection";
         this.layoutItemBuildingBlockSelection.Location = new System.Drawing.Point(0, 29);
         this.layoutItemBuildingBlockSelection.MaxSize = new System.Drawing.Size(0, 30);
         this.layoutItemBuildingBlockSelection.MinSize = new System.Drawing.Size(294, 30);
         this.layoutItemBuildingBlockSelection.Name = "layoutItemPanelBuildingBlockSelection";
         this.layoutItemBuildingBlockSelection.Size = new System.Drawing.Size(597, 30);
         this.layoutItemBuildingBlockSelection.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemBuildingBlockSelection.Text = "layoutItemPanelBuildingBlockSelection";
         this.layoutItemBuildingBlockSelection.TextSize = new System.Drawing.Size(181, 13);
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.rbPopulationFileSelection;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 59);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(597, 29);
         this.layoutControlItem2.Text = "layoutControlItem2";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextToControlDistance = 0;
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutItemPopulationFileSelection
         // 
         this.layoutItemPopulationFileSelection.Control = this.tbPopulationFile;
         this.layoutItemPopulationFileSelection.CustomizationFormText = "layoutControlItem4";
         this.layoutItemPopulationFileSelection.Location = new System.Drawing.Point(0, 88);
         this.layoutItemPopulationFileSelection.Name = "layoutItemPopulationFileSelection";
         this.layoutItemPopulationFileSelection.Size = new System.Drawing.Size(597, 24);
         this.layoutItemPopulationFileSelection.Text = "layoutItemPopulationFileSelection";
         this.layoutItemPopulationFileSelection.TextSize = new System.Drawing.Size(181, 13);
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.rbPopulationSizeSelection;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(0, 112);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Size = new System.Drawing.Size(597, 29);
         this.layoutControlItem3.Text = "layoutControlItem3";
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextToControlDistance = 0;
         this.layoutControlItem3.TextVisible = false;
         // 
         // layoutItemNumberOfIndividuals
         // 
         this.layoutItemNumberOfIndividuals.Control = this.tbNumberOfIndividuals;
         this.layoutItemNumberOfIndividuals.CustomizationFormText = "layoutItemNumberOfIndividuals";
         this.layoutItemNumberOfIndividuals.Location = new System.Drawing.Point(0, 141);
         this.layoutItemNumberOfIndividuals.Name = "layoutItemNumberOfIndividuals";
         this.layoutItemNumberOfIndividuals.Size = new System.Drawing.Size(597, 24);
         this.layoutItemNumberOfIndividuals.Text = "layoutItemNumberOfIndividuals";
         this.layoutItemNumberOfIndividuals.TextSize = new System.Drawing.Size(181, 13);
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemLog,
            this.emptySpaceItem,
            this.layoutItemButtonImport});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 275);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(621, 191);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemLog
         // 
         this.layoutItemLog.Control = this.tbLog;
         this.layoutItemLog.CustomizationFormText = "layoutItemLog";
         this.layoutItemLog.Location = new System.Drawing.Point(0, 0);
         this.layoutItemLog.Name = "layoutItemLog";
         this.layoutItemLog.Size = new System.Drawing.Size(597, 141);
         this.layoutItemLog.Text = "layoutItemLog";
         this.layoutItemLog.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLog.TextToControlDistance = 0;
         this.layoutItemLog.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.CustomizationFormText = "emptySpaceItem";
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 141);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(298, 26);
         this.emptySpaceItem.Text = "emptySpaceItem";
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemButtonImport
         // 
         this.layoutItemButtonImport.Control = this.btnStartImport;
         this.layoutItemButtonImport.CustomizationFormText = "layoutItemButtonImport";
         this.layoutItemButtonImport.Location = new System.Drawing.Point(298, 141);
         this.layoutItemButtonImport.Name = "layoutItemButtonImport";
         this.layoutItemButtonImport.Size = new System.Drawing.Size(299, 26);
         this.layoutItemButtonImport.Text = "layoutItemButtonImport";
         this.layoutItemButtonImport.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonImport.TextToControlDistance = 0;
         this.layoutItemButtonImport.TextVisible = false;
         // 
         // ImportPopulationSimulationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ImportPopulationSimulationView";
         this.ClientSize = new System.Drawing.Size(641, 532);
         this.Controls.Add(this.layoutControl);
         this.Name = "ImportPopulationSimulationView";
         this.Text = "ImportPopulationSimulationView";
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
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividuals.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbPopulationFile.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelBuildingBlockSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbSimulationFile.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupSimulationFileSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSimulationFileSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBuildingBlockSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulationFileSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividuals)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImport)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.TextEdit tbNumberOfIndividuals;
      private DevExpress.XtraEditors.MemoEdit tbLog;
      private DevExpress.XtraEditors.ButtonEdit tbPopulationFile;
      private DevExpress.XtraEditors.PanelControl panelBuildingBlockSelection;
      private System.Windows.Forms.RadioButton rbPopulationSizeSelection;
      private System.Windows.Forms.RadioButton rbPopulationFileSelection;
      private System.Windows.Forms.RadioButton rbBuildingBockSelection;
      private DevExpress.XtraEditors.ButtonEdit tbSimulationFile;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSimulationFileSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemBuildingBlockSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPopulationFileSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLog;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNumberOfIndividuals;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupSimulationFileSelection;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupPopulationSelection;
      private DevExpress.XtraEditors.SimpleButton btnStartImport;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonImport;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
   }
}