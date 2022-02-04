namespace PKSim.UI.Views.Simulations
{
   partial class SimulationModelConfigurationView
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
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelModelSelection = new DevExpress.XtraEditors.PanelControl();
         this.panelCompoundList = new DevExpress.XtraEditors.PanelControl();
         this.panelSubjectSelection = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemModelSelectionView = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupCompoundsSelection = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemCompoundListView = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupSubjectSelection = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSubjectSelectionView = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupModelSettings = new DevExpress.XtraLayout.LayoutControlGroup();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelModelSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelCompoundList)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelSubjectSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemModelSelectionView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupCompoundsSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCompoundListView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupSubjectSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSubjectSelectionView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupModelSettings)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelModelSelection);
         this.layoutControl.Controls.Add(this.panelCompoundList);
         this.layoutControl.Controls.Add(this.panelSubjectSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(516, 671);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelModelSelection
         // 
         this.panelModelSelection.Location = new System.Drawing.Point(224, 395);
         this.panelModelSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.panelModelSelection.Name = "panelModelSelection";
         this.panelModelSelection.Size = new System.Drawing.Size(265, 248);
         this.panelModelSelection.TabIndex = 6;
         // 
         // panelCompoundList
         // 
         this.panelCompoundList.Location = new System.Drawing.Point(224, 220);
         this.panelCompoundList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.panelCompoundList.Name = "panelCompoundList";
         this.panelCompoundList.Size = new System.Drawing.Size(265, 107);
         this.panelCompoundList.TabIndex = 5;
         // 
         // panelSubjectSelection
         // 
         this.panelSubjectSelection.Location = new System.Drawing.Point(224, 54);
         this.panelSubjectSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.panelSubjectSelection.Name = "panelSubjectSelection";
         this.panelSubjectSelection.Size = new System.Drawing.Size(265, 108);
         this.panelSubjectSelection.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupCompoundsSelection,
            this.layoutGroupSubjectSelection,
            this.layoutGroupModelSettings,
            this.emptySpaceItem1});
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(516, 671);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemModelSelectionView
         // 
         this.layoutItemModelSelectionView.Control = this.panelModelSelection;
         this.layoutItemModelSelectionView.CustomizationFormText = "layoutItemModelSelectionView";
         this.layoutItemModelSelectionView.Location = new System.Drawing.Point(0, 0);
         this.layoutItemModelSelectionView.Name = "layoutItemModelSelectionView";
         this.layoutItemModelSelectionView.Size = new System.Drawing.Size(466, 252);
         this.layoutItemModelSelectionView.TextSize = new System.Drawing.Size(183, 16);
         // 
         // layoutGroupCompoundsSelection
         // 
         this.layoutGroupCompoundsSelection.CustomizationFormText = "layoutGroupCompoundsSelection";
         this.layoutGroupCompoundsSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemCompoundListView});
         this.layoutGroupCompoundsSelection.Location = new System.Drawing.Point(0, 166);
         this.layoutGroupCompoundsSelection.Name = "layoutGroupCompoundsSelection";
         this.layoutGroupCompoundsSelection.Size = new System.Drawing.Size(492, 165);
         this.layoutGroupCompoundsSelection.Text = "layoutGroupCompoundsSelection";
         // 
         // layoutItemCompoundListView
         // 
         this.layoutItemCompoundListView.Control = this.panelCompoundList;
         this.layoutItemCompoundListView.CustomizationFormText = "layoutItemCompoundListView";
         this.layoutItemCompoundListView.Location = new System.Drawing.Point(0, 0);
         this.layoutItemCompoundListView.Name = "layoutItemCompoundListView";
         this.layoutItemCompoundListView.Size = new System.Drawing.Size(466, 111);
         this.layoutItemCompoundListView.TextSize = new System.Drawing.Size(183, 16);
         // 
         // layoutGroupSubjectSelection
         // 
         this.layoutGroupSubjectSelection.CustomizationFormText = "layoutControlGroup2";
         this.layoutGroupSubjectSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSubjectSelectionView});
         this.layoutGroupSubjectSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupSubjectSelection.Name = "layoutGroupSubjectSelection";
         this.layoutGroupSubjectSelection.Size = new System.Drawing.Size(492, 166);
         // 
         // layoutItemSubjectSelectionView
         // 
         this.layoutItemSubjectSelectionView.Control = this.panelSubjectSelection;
         this.layoutItemSubjectSelectionView.CustomizationFormText = "layoutItemSubjectSelectionView";
         this.layoutItemSubjectSelectionView.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSubjectSelectionView.MaxSize = new System.Drawing.Size(0, 112);
         this.layoutItemSubjectSelectionView.MinSize = new System.Drawing.Size(304, 112);
         this.layoutItemSubjectSelectionView.Name = "layoutItemSubjectSelectionView";
         this.layoutItemSubjectSelectionView.Size = new System.Drawing.Size(466, 112);
         this.layoutItemSubjectSelectionView.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemSubjectSelectionView.TextSize = new System.Drawing.Size(183, 16);
         // 
         // layoutGroupModelSettings
         // 
         this.layoutGroupModelSettings.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemModelSelectionView});
         this.layoutGroupModelSettings.Location = new System.Drawing.Point(0, 341);
         this.layoutGroupModelSettings.Name = "layoutGroupModelSettings";
         this.layoutGroupModelSettings.Size = new System.Drawing.Size(492, 306);
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 331);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(492, 10);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // SimulationModelConfigurationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.Name = "SimulationModelConfigurationView";
         this.Size = new System.Drawing.Size(516, 671);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelModelSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelCompoundList)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelSubjectSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemModelSelectionView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupCompoundsSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCompoundListView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupSubjectSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSubjectSelectionView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupModelSettings)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelModelSelection;
      private DevExpress.XtraEditors.PanelControl panelCompoundList;
      private DevExpress.XtraEditors.PanelControl panelSubjectSelection;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSubjectSelectionView;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemModelSelectionView;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCompoundListView;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupCompoundsSelection;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupSubjectSelection;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupModelSettings;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}
