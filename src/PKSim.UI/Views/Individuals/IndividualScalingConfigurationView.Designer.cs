using PKSim.UI.Views.Core;
using PKSim.UI.Views.Parameters;

namespace PKSim.UI.Views.Individuals
{
   partial class IndividualScalingConfigurationView
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
         this.gridScalingConfiguration = new PKSim.UI.Views.Core.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.uxWeight = new UxParameterDTOEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemTargetBodyWeight = new DevExpress.XtraLayout.LayoutControlItem();
         this.spaceItemBodyWeight = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridScalingConfiguration)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTargetBodyWeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.spaceItemBodyWeight)).BeginInit();
         this.SuspendLayout();
         // 
         // gridScalingConfiguration
         // 
         gridLevelNode1.RelationName = "Level1";
         this.gridScalingConfiguration.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
         this.gridScalingConfiguration.Location = new System.Drawing.Point(0, 32);
         this.gridScalingConfiguration.MainView = this.gridView;
         this.gridScalingConfiguration.Name = "gridScalingConfiguration";
         this.gridScalingConfiguration.Size = new System.Drawing.Size(438, 330);
         this.gridScalingConfiguration.TabIndex = 0;
         this.gridScalingConfiguration.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridViewScalingConfiguration
         // 
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridScalingConfiguration;
         this.gridView.Name = "gridView";
         this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridView.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.uxWeight);
         this.layoutControl.Controls.Add(this.gridScalingConfiguration);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(484, 133, 250, 350);
         this.layoutControl.OptionsView.UseSkinIndents = false;
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(438, 362);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // uxWeight
         // 
         this.uxWeight.Caption = "";
         this.uxWeight.Location = new System.Drawing.Point(296, 5);
         this.uxWeight.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxWeight.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxWeight.Name = "uxWeight";
         this.uxWeight.Size = new System.Drawing.Size(137, 22);
         this.uxWeight.TabIndex = 11;
         this.uxWeight.ToolTip = "";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemGrid,
            this.layoutItemTargetBodyWeight,
            this.spaceItemBodyWeight});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.OptionsItemText.TextToControlDistance = 5;
         this.layoutControlGroup.Size = new System.Drawing.Size(438, 362);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutControlItemGrid
         // 
         this.layoutControlItemGrid.Control = this.gridScalingConfiguration;
         this.layoutControlItemGrid.CustomizationFormText = "layoutControlItemGrid";
         this.layoutControlItemGrid.Location = new System.Drawing.Point(0, 32);
         this.layoutControlItemGrid.Name = "layoutControlItemGrid";
         this.layoutControlItemGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlItemGrid.Size = new System.Drawing.Size(438, 330);
         this.layoutControlItemGrid.Text = "layoutControlItemGrid";
         this.layoutControlItemGrid.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItemGrid.TextToControlDistance = 0;
         this.layoutControlItemGrid.TextVisible = false;
         // 
         // layoutItemTargetBodyWeight
         // 
         this.layoutItemTargetBodyWeight.Control = this.uxWeight;
         this.layoutItemTargetBodyWeight.CustomizationFormText = "layoutItemTargetBodyWeight";
         this.layoutItemTargetBodyWeight.Location = new System.Drawing.Point(144, 0);
         this.layoutItemTargetBodyWeight.MaxSize = new System.Drawing.Size(294, 32);
         this.layoutItemTargetBodyWeight.MinSize = new System.Drawing.Size(294, 32);
         this.layoutItemTargetBodyWeight.Name = "layoutItemTargetBodyWeight";
         this.layoutItemTargetBodyWeight.Size = new System.Drawing.Size(294, 32);
         this.layoutItemTargetBodyWeight.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemTargetBodyWeight.Text = "layoutItemTargetBodyWeight";
         this.layoutItemTargetBodyWeight.TextSize = new System.Drawing.Size(142, 13);
         this.layoutItemTargetBodyWeight.TextToControlDistance = 5;
         // 
         // spaceItemBodyWeight
         // 
         this.spaceItemBodyWeight.AllowHotTrack = false;
         this.spaceItemBodyWeight.CustomizationFormText = "emptySpaceItem1";
         this.spaceItemBodyWeight.Location = new System.Drawing.Point(0, 0);
         this.spaceItemBodyWeight.Name = "spaceItemBodyWeight";
         this.spaceItemBodyWeight.Size = new System.Drawing.Size(144, 32);
         this.spaceItemBodyWeight.Text = "spaceItemBodyWeight";
         this.spaceItemBodyWeight.TextSize = new System.Drawing.Size(0, 0);
         // 
         // IndividualScalingConfigurationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualScalingConfigurationView";
         this.Size = new System.Drawing.Size(438, 362);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridScalingConfiguration)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTargetBodyWeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.spaceItemBodyWeight)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private UxGridControl gridScalingConfiguration;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGrid;
      private UxGridView gridView;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private UxParameterDTOEdit uxWeight;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTargetBodyWeight;
      private DevExpress.XtraLayout.EmptySpaceItem spaceItemBodyWeight;
   }
}


