namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class PopulationAnalysisParameterSelectionView
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
         this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
         this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
         this.panelSelectedParameters = new DevExpress.XtraEditors.PanelControl();
         this.panelParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemButtonAdd = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonRemove = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         this.splitContainerControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
   // 
         // splitContainerControl
         // 
         this.splitContainerControl.Horizontal = false;
         this.splitContainerControl.Location = new System.Drawing.Point(2, 2);
         this.splitContainerControl.Name = "splitContainerControl";
         this.splitContainerControl.Panel1.Controls.Add(this.layoutControl);
         this.splitContainerControl.Panel1.Text = "Panel1";
         this.splitContainerControl.Panel2.Text = "Panel2";
         this.splitContainerControl.Size = new System.Drawing.Size(773, 489);
         this.splitContainerControl.SplitterPosition = 265;
         this.splitContainerControl.TabIndex = 4;
         this.splitContainerControl.Text = "splitContainerControl1";
         // 
         // layoutControl1
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.btnRemove);
         this.layoutControl.Controls.Add(this.btnAdd);
         this.layoutControl.Controls.Add(this.panelSelectedParameters);
         this.layoutControl.Controls.Add(this.panelParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(773, 265);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnRemove
         // 
         this.btnRemove.Location = new System.Drawing.Point(341, 138);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(93, 22);
         this.btnRemove.StyleController = this.layoutControl;
         this.btnRemove.TabIndex = 7;
         this.btnRemove.Text = "btnRemove";
         // 
         // btnAdd
         // 
         this.btnAdd.Location = new System.Drawing.Point(341, 112);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(93, 22);
         this.btnAdd.StyleController = this.layoutControl;
         this.btnAdd.TabIndex = 6;
         this.btnAdd.Text = "btnAdd";
         // 
         // panelSelectedParameters
         // 
         this.panelSelectedParameters.Location = new System.Drawing.Point(438, 2);
         this.panelSelectedParameters.Name = "panelSelectedParameters";
         this.panelSelectedParameters.Size = new System.Drawing.Size(333, 261);
         this.panelSelectedParameters.TabIndex = 5;
         // 
         // panelParameters
         // 
         this.panelParameters.Location = new System.Drawing.Point(2, 2);
         this.panelParameters.Name = "panelParameters";
         this.panelParameters.Padding = new System.Windows.Forms.Padding(10);
         this.panelParameters.Size = new System.Drawing.Size(335, 261);
         this.panelParameters.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutItemButtonAdd,
            this.layoutItemButtonRemove});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(773, 265);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.panelParameters;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(339, 265);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.panelSelectedParameters;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(436, 0);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Size = new System.Drawing.Size(337, 265);
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(339, 0);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(97, 110);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(339, 162);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(97, 103);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemButtonAdd
         // 
         this.layoutItemButtonAdd.Control = this.btnAdd;
         this.layoutItemButtonAdd.CustomizationFormText = "layoutControlItem4";
         this.layoutItemButtonAdd.Location = new System.Drawing.Point(339, 110);
         this.layoutItemButtonAdd.Name = "layoutItemButtonAdd";
         this.layoutItemButtonAdd.Size = new System.Drawing.Size(97, 26);
         this.layoutItemButtonAdd.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAdd.TextVisible = false;
         // 
         // layoutItemButtonRemove
         // 
         this.layoutItemButtonRemove.Control = this.btnRemove;
         this.layoutItemButtonRemove.CustomizationFormText = "layoutControlItem5";
         this.layoutItemButtonRemove.Location = new System.Drawing.Point(339, 136);
         this.layoutItemButtonRemove.Name = "layoutItemButtonRemove";
         this.layoutItemButtonRemove.Size = new System.Drawing.Size(97, 26);
         this.layoutItemButtonRemove.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonRemove.TextVisible = false;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(777, 493);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.splitContainerControl;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(777, 493);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // PopulationAnalysisParameterSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Name = "PopulationAnalysisParameterSelectionView";
         this.Size = new System.Drawing.Size(777, 493);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SimpleButton btnRemove;
      private DevExpress.XtraEditors.SimpleButton btnAdd;
      private DevExpress.XtraEditors.PanelControl panelSelectedParameters;
      private DevExpress.XtraEditors.PanelControl panelParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAdd;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonRemove;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}
