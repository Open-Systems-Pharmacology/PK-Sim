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
         this.gridParameters = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         this.radioGroupCommitMode = new DevExpress.XtraEditors.RadioGroup();
         this.tbNewSetName = new DevExpress.XtraEditors.TextEdit();
         this.cbExistingSet = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupOptions = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemCommitMode = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNewSetName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemExistingSet = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupCommitMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNewSetName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbExistingSet.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupOptions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCommitMode)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNewSetName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExistingSet)).BeginInit();
         this.SuspendLayout();
         //
         // layoutControl
         //
         this.layoutControl.Controls.Add(this.gridParameters);
         this.layoutControl.Controls.Add(this.radioGroupCommitMode);
         this.layoutControl.Controls.Add(this.tbNewSetName);
         this.layoutControl.Controls.Add(this.cbExistingSet);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(500, 418);
         this.layoutControl.TabIndex = 38;
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
         this.radioGroupCommitMode.StyleController = this.layoutControl;
         this.radioGroupCommitMode.TabIndex = 5;
         //
         // tbNewSetName
         //
         this.tbNewSetName.Location = new System.Drawing.Point(117, 356);
         this.tbNewSetName.Name = "tbNewSetName";
         this.tbNewSetName.Size = new System.Drawing.Size(359, 20);
         this.tbNewSetName.StyleController = this.layoutControl;
         this.tbNewSetName.TabIndex = 6;
         //
         // cbExistingSet
         //
         this.cbExistingSet.Location = new System.Drawing.Point(117, 380);
         this.cbExistingSet.Name = "cbExistingSet";
         this.cbExistingSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbExistingSet.Size = new System.Drawing.Size(359, 20);
         this.cbExistingSet.StyleController = this.layoutControl;
         this.cbExistingSet.TabIndex = 7;
         //
         // layoutControlGroup
         //
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.layoutGroupOptions});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(500, 418);
         this.layoutControlGroup.TextVisible = false;
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
         this.ClientSize = new System.Drawing.Size(700, 461);
         this.Controls.Add(this.layoutControl);
         this.Name = "CommitSimulationParametersView";
         this.Text = "CommitSimulationParametersView";
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupCommitMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNewSetName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbExistingSet.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupOptions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCommitMode)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNewSetName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExistingSet)).EndInit();
         this.ResumeLayout(false);
      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
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
