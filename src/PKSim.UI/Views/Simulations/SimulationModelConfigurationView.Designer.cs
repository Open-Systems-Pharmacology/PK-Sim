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
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelModelSelection);
         this.layoutControl.Controls.Add(this.panelCompoundList);
         this.layoutControl.Controls.Add(this.panelSubjectSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(442, 545);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelModelSelection
         // 
         this.panelModelSelection.Location = new System.Drawing.Point(168, 281);
         this.panelModelSelection.Name = "panelModelSelection";
         this.panelModelSelection.Size = new System.Drawing.Size(262, 252);
         this.panelModelSelection.TabIndex = 6;
         // 
         // panelCompoundList
         // 
         this.panelCompoundList.Location = new System.Drawing.Point(180, 177);
         this.panelCompoundList.Name = "panelCompoundList";
         this.panelCompoundList.Size = new System.Drawing.Size(238, 88);
         this.panelCompoundList.TabIndex = 5;
         // 
         // panelSubjectSelection
         // 
         this.panelSubjectSelection.Location = new System.Drawing.Point(180, 43);
         this.panelSubjectSelection.Name = "panelSubjectSelection";
         this.panelSubjectSelection.Size = new System.Drawing.Size(238, 87);
         this.panelSubjectSelection.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemModelSelectionView,
            this.layoutGroupCompoundsSelection,
            this.layoutGroupSubjectSelection});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(442, 545);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemModelSelectionView
         // 
         this.layoutItemModelSelectionView.Control = this.panelModelSelection;
         this.layoutItemModelSelectionView.CustomizationFormText = "layoutItemModelSelectionView";
         this.layoutItemModelSelectionView.Location = new System.Drawing.Point(0, 269);
         this.layoutItemModelSelectionView.Name = "layoutItemModelSelectionView";
         this.layoutItemModelSelectionView.Size = new System.Drawing.Size(422, 256);
         this.layoutItemModelSelectionView.Text = "layoutItemModelSelectionView";
         this.layoutItemModelSelectionView.TextSize = new System.Drawing.Size(153, 13);
         // 
         // layoutGroupCompoundsSelection
         // 
         this.layoutGroupCompoundsSelection.CustomizationFormText = "layoutGroupCompoundsSelection";
         this.layoutGroupCompoundsSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemCompoundListView});
         this.layoutGroupCompoundsSelection.Location = new System.Drawing.Point(0, 134);
         this.layoutGroupCompoundsSelection.Name = "layoutGroupCompoundsSelection";
         this.layoutGroupCompoundsSelection.Size = new System.Drawing.Size(422, 135);
         this.layoutGroupCompoundsSelection.Text = "layoutControlGroup1";
         // 
         // layoutItemCompoundListView
         // 
         this.layoutItemCompoundListView.Control = this.panelCompoundList;
         this.layoutItemCompoundListView.CustomizationFormText = "layoutItemCompoundListView";
         this.layoutItemCompoundListView.Location = new System.Drawing.Point(0, 0);
         this.layoutItemCompoundListView.Name = "layoutItemCompoundListView";
         this.layoutItemCompoundListView.Size = new System.Drawing.Size(398, 92);
         this.layoutItemCompoundListView.Text = "layoutItemCompoundListView";
         this.layoutItemCompoundListView.TextSize = new System.Drawing.Size(153, 13);
         // 
         // layoutGroupSubjectSelection
         // 
         this.layoutGroupSubjectSelection.CustomizationFormText = "layoutControlGroup2";
         this.layoutGroupSubjectSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSubjectSelectionView});
         this.layoutGroupSubjectSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupSubjectSelection.Name = "layoutGroupSubjectSelection";
         this.layoutGroupSubjectSelection.Size = new System.Drawing.Size(422, 134);
         this.layoutGroupSubjectSelection.Text = "layoutGroupSubjectSelection";
         // 
         // layoutItemSubjectSelectionView
         // 
         this.layoutItemSubjectSelectionView.Control = this.panelSubjectSelection;
         this.layoutItemSubjectSelectionView.CustomizationFormText = "layoutItemSubjectSelectionView";
         this.layoutItemSubjectSelectionView.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSubjectSelectionView.MaxSize = new System.Drawing.Size(0, 91);
         this.layoutItemSubjectSelectionView.MinSize = new System.Drawing.Size(261, 91);
         this.layoutItemSubjectSelectionView.Name = "layoutItemSubjectSelectionView";
         this.layoutItemSubjectSelectionView.Size = new System.Drawing.Size(398, 91);
         this.layoutItemSubjectSelectionView.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemSubjectSelectionView.Text = "layoutItemSubjectSelectionView";
         this.layoutItemSubjectSelectionView.TextSize = new System.Drawing.Size(153, 13);
         // 
         // SimulationModelConfigurationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationModelConfigurationView";
         this.Size = new System.Drawing.Size(442, 545);
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
   }
}
