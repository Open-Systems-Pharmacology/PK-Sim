using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   partial class CommitSimulationParametersView
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
         _parameterGridBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.listCompounds = new DevExpress.XtraEditors.CheckedListBoxControl();
         this.rightLayoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.gridParameters = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         this.radioGroupCommitMode = new DevExpress.XtraEditors.RadioGroup();
         this.tbNewSetName = new DevExpress.XtraEditors.TextEdit();
         this.cbExistingSet = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.rightLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupOptions = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemCommitMode = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNewSetName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemExistingSet = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.listCompounds)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.rightLayoutControl)).BeginInit();
         this.rightLayoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupCommitMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNewSetName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbExistingSet.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.rightLayoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupOptions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCommitMode)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNewSetName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExistingSet)).BeginInit();
         this.SuspendLayout();
         //
         // splitContainer
         //
         this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer.Location = new System.Drawing.Point(0, 0);
         this.splitContainer.Name = "splitContainer";
         this.splitContainer.Panel1.Controls.Add(this.listCompounds);
         this.splitContainer.Panel2.Controls.Add(this.rightLayoutControl);
         this.splitContainer.Size = new System.Drawing.Size(684, 418);
         this.splitContainer.SplitterPosition = 180;
         this.splitContainer.TabIndex = 38;
         //
         // listCompounds
         //
         this.listCompounds.CheckOnClick = true;
         this.listCompounds.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listCompounds.Location = new System.Drawing.Point(0, 0);
         this.listCompounds.Name = "listCompounds";
         this.listCompounds.Size = new System.Drawing.Size(180, 418);
         this.listCompounds.TabIndex = 0;
         //
         // rightLayoutControl
         //
         this.rightLayoutControl.Controls.Add(this.gridParameters);
         this.rightLayoutControl.Controls.Add(this.radioGroupCommitMode);
         this.rightLayoutControl.Controls.Add(this.tbNewSetName);
         this.rightLayoutControl.Controls.Add(this.cbExistingSet);
         this.rightLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.rightLayoutControl.Location = new System.Drawing.Point(0, 0);
         this.rightLayoutControl.Name = "rightLayoutControl";
         this.rightLayoutControl.Root = this.rightLayoutControlGroup;
         this.rightLayoutControl.Size = new System.Drawing.Size(500, 418);
         this.rightLayoutControl.TabIndex = 0;
         //
         // gridParameters
         //
         this.gridParameters.Location = new System.Drawing.Point(12, 12);
         this.gridParameters.MainView = this.gridViewParameters;
         this.gridParameters.Name = "gridParameters";
         this.gridParameters.Size = new System.Drawing.Size(476, 250);
         this.gridParameters.TabIndex = 4;
         this.gridParameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameters});
         //
         // gridViewParameters
         //
         this.gridViewParameters.AllowsFiltering = false;
         this.gridViewParameters.EnableColumnContextMenu = false;
         this.gridViewParameters.GridControl = this.gridParameters;
         this.gridViewParameters.Name = "gridViewParameters";
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedRow = true;
         //
         // radioGroupCommitMode
         //
         this.radioGroupCommitMode.Location = new System.Drawing.Point(24, 302);
         this.radioGroupCommitMode.Name = "radioGroupCommitMode";
         this.radioGroupCommitMode.Size = new System.Drawing.Size(452, 50);
         this.radioGroupCommitMode.StyleController = this.rightLayoutControl;
         this.radioGroupCommitMode.TabIndex = 5;
         //
         // tbNewSetName
         //
         this.tbNewSetName.Location = new System.Drawing.Point(117, 356);
         this.tbNewSetName.Name = "tbNewSetName";
         this.tbNewSetName.Size = new System.Drawing.Size(359, 20);
         this.tbNewSetName.StyleController = this.rightLayoutControl;
         this.tbNewSetName.TabIndex = 6;
         //
         // cbExistingSet
         //
         this.cbExistingSet.Location = new System.Drawing.Point(117, 380);
         this.cbExistingSet.Name = "cbExistingSet";
         this.cbExistingSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbExistingSet.Size = new System.Drawing.Size(359, 20);
         this.cbExistingSet.StyleController = this.rightLayoutControl;
         this.cbExistingSet.TabIndex = 7;
         //
         // rightLayoutControlGroup
         //
         this.rightLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.rightLayoutControlGroup.GroupBordersVisible = false;
         this.rightLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.layoutGroupOptions});
         this.rightLayoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.rightLayoutControlGroup.Name = "rightLayoutControlGroup";
         this.rightLayoutControlGroup.Size = new System.Drawing.Size(500, 418);
         this.rightLayoutControlGroup.TextVisible = false;
         //
         // layoutItemParameters
         //
         this.layoutItemParameters.Control = this.gridParameters;
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Size = new System.Drawing.Size(480, 254);
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextToControlDistance = 0;
         this.layoutItemParameters.TextVisible = false;
         //
         // layoutGroupOptions
         //
         this.layoutGroupOptions.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemCommitMode,
            this.layoutItemNewSetName,
            this.layoutItemExistingSet});
         this.layoutGroupOptions.Location = new System.Drawing.Point(0, 254);
         this.layoutGroupOptions.Name = "layoutGroupOptions";
         this.layoutGroupOptions.Size = new System.Drawing.Size(480, 144);
         this.layoutGroupOptions.Text = "Commit Options";
         //
         // layoutItemCommitMode
         //
         this.layoutItemCommitMode.Control = this.radioGroupCommitMode;
         this.layoutItemCommitMode.Location = new System.Drawing.Point(0, 0);
         this.layoutItemCommitMode.Name = "layoutItemCommitMode";
         this.layoutItemCommitMode.Size = new System.Drawing.Size(456, 54);
         this.layoutItemCommitMode.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCommitMode.TextToControlDistance = 0;
         this.layoutItemCommitMode.TextVisible = false;
         //
         // layoutItemNewSetName
         //
         this.layoutItemNewSetName.Control = this.tbNewSetName;
         this.layoutItemNewSetName.Location = new System.Drawing.Point(0, 54);
         this.layoutItemNewSetName.Name = "layoutItemNewSetName";
         this.layoutItemNewSetName.Size = new System.Drawing.Size(456, 24);
         this.layoutItemNewSetName.Text = "Name";
         this.layoutItemNewSetName.TextSize = new System.Drawing.Size(90, 13);
         //
         // layoutItemExistingSet
         //
         this.layoutItemExistingSet.Control = this.cbExistingSet;
         this.layoutItemExistingSet.Location = new System.Drawing.Point(0, 78);
         this.layoutItemExistingSet.Name = "layoutItemExistingSet";
         this.layoutItemExistingSet.Size = new System.Drawing.Size(456, 24);
         this.layoutItemExistingSet.Text = "Parameter Set";
         this.layoutItemExistingSet.TextSize = new System.Drawing.Size(90, 13);
         //
         // CommitSimulationParametersView
         //
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "CommitSimulationParametersView";
         this.ClientSize = new System.Drawing.Size(684, 461);
         this.Controls.Add(this.splitContainer);
         this.Name = "CommitSimulationParametersView";
         this.Text = "CommitSimulationParametersView";
         this.Controls.SetChildIndex(this.splitContainer, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.listCompounds)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.rightLayoutControl)).EndInit();
         this.rightLayoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupCommitMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNewSetName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbExistingSet.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.rightLayoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupOptions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCommitMode)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNewSetName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExistingSet)).EndInit();
         this.ResumeLayout(false);
      }

      #endregion

      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private DevExpress.XtraEditors.CheckedListBoxControl listCompounds;
      private OSPSuite.UI.Controls.UxLayoutControl rightLayoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup rightLayoutControlGroup;
      private OSPSuite.UI.Controls.UxGridControl gridParameters;
      private PKSim.UI.Views.Core.UxGridView gridViewParameters;
      private DevExpress.XtraEditors.RadioGroup radioGroupCommitMode;
      private DevExpress.XtraEditors.TextEdit tbNewSetName;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbExistingSet;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupOptions;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCommitMode;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNewSetName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExistingSet;
   }
}
