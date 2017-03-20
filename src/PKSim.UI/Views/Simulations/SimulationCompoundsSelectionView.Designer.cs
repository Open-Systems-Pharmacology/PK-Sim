
namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundsSelectionView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.uxHintPanel = new OSPSuite.UI.Controls.UxHintPanel();
         this.btnLoadCompound = new DevExpress.XtraEditors.SimpleButton();
         this.btnCreateCompound = new DevExpress.XtraEditors.SimpleButton();
         this.gridControl = new DevExpress.XtraGrid.GridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemAddCompound = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLoadCompound = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCompounds = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemWarning = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddCompound)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoadCompound)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCompounds)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.uxHintPanel);
         this.layoutControl.Controls.Add(this.btnLoadCompound);
         this.layoutControl.Controls.Add(this.btnCreateCompound);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(545, 67, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(323, 296);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // uxHintPanel
         // 
         this.uxHintPanel.Location = new System.Drawing.Point(2, 2);
         this.uxHintPanel.MaximumSize = new System.Drawing.Size(1000000, 40);
         this.uxHintPanel.MinimumSize = new System.Drawing.Size(200, 40);
         this.uxHintPanel.Name = "uxHintPanel";
         this.uxHintPanel.NoteText = "";
         this.uxHintPanel.Size = new System.Drawing.Size(319, 40);
         this.uxHintPanel.TabIndex = 7;
         // 
         // btnLoadCompound
         // 
         this.btnLoadCompound.Location = new System.Drawing.Point(211, 46);
         this.btnLoadCompound.Name = "btnLoadCompound";
         this.btnLoadCompound.Size = new System.Drawing.Size(110, 22);
         this.btnLoadCompound.StyleController = this.layoutControl;
         this.btnLoadCompound.TabIndex = 6;
         this.btnLoadCompound.Text = "btnLoadCompound";
         // 
         // btnCreateCompound
         // 
         this.btnCreateCompound.Location = new System.Drawing.Point(87, 46);
         this.btnCreateCompound.Name = "btnCreateCompound";
         this.btnCreateCompound.Size = new System.Drawing.Size(120, 22);
         this.btnCreateCompound.StyleController = this.layoutControl;
         this.btnCreateCompound.TabIndex = 5;
         this.btnCreateCompound.Text = "btnCreateCompound";
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(2, 72);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(319, 222);
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
            this.emptySpaceItem1,
            this.layoutItemAddCompound,
            this.layoutItemLoadCompound,
            this.layoutItemCompounds,
            this.layoutItemWarning});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(323, 296);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 44);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(85, 26);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemAddCompound
         // 
         this.layoutItemAddCompound.Control = this.btnCreateCompound;
         this.layoutItemAddCompound.CustomizationFormText = "layoutControlItem1";
         this.layoutItemAddCompound.Location = new System.Drawing.Point(85, 44);
         this.layoutItemAddCompound.Name = "layoutItemAddCompound";
         this.layoutItemAddCompound.Size = new System.Drawing.Size(124, 26);
         this.layoutItemAddCompound.Text = "layoutItemAddCompound";
         this.layoutItemAddCompound.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAddCompound.TextToControlDistance = 0;
         this.layoutItemAddCompound.TextVisible = false;
         // 
         // layoutItemLoadCompound
         // 
         this.layoutItemLoadCompound.Control = this.btnLoadCompound;
         this.layoutItemLoadCompound.CustomizationFormText = "layoutControlItem2";
         this.layoutItemLoadCompound.Location = new System.Drawing.Point(209, 44);
         this.layoutItemLoadCompound.Name = "layoutItemLoadCompound";
         this.layoutItemLoadCompound.Size = new System.Drawing.Size(114, 26);
         this.layoutItemLoadCompound.Text = "layoutItemLoadCompound";
         this.layoutItemLoadCompound.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLoadCompound.TextToControlDistance = 0;
         this.layoutItemLoadCompound.TextVisible = false;
         // 
         // layoutItemCompounds
         // 
         this.layoutItemCompounds.Control = this.gridControl;
         this.layoutItemCompounds.CustomizationFormText = "layoutItemCompounds";
         this.layoutItemCompounds.Location = new System.Drawing.Point(0, 70);
         this.layoutItemCompounds.Name = "layoutItemCompounds";
         this.layoutItemCompounds.Size = new System.Drawing.Size(323, 226);
         this.layoutItemCompounds.Text = "layoutItemCompounds";
         this.layoutItemCompounds.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCompounds.TextToControlDistance = 0;
         this.layoutItemCompounds.TextVisible = false;
         // 
         // layoutItemWarning
         // 
         this.layoutItemWarning.Control = this.uxHintPanel;
         this.layoutItemWarning.CustomizationFormText = "layoutControlItem1";
         this.layoutItemWarning.Location = new System.Drawing.Point(0, 0);
         this.layoutItemWarning.Name = "layoutItemWarning";
         this.layoutItemWarning.Size = new System.Drawing.Size(323, 44);
         this.layoutItemWarning.Text = "uxWarningItem";
         this.layoutItemWarning.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemWarning.TextToControlDistance = 0;
         this.layoutItemWarning.TextVisible = false;
         // 
         // SimulationCompoundsSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationCompoundsSelectionView";
         this.Size = new System.Drawing.Size(323, 296);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddCompound)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoadCompound)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCompounds)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraGrid.GridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SimpleButton btnLoadCompound;
      private DevExpress.XtraEditors.SimpleButton btnCreateCompound;
      private OSPSuite.UI.Controls.UxHintPanel uxHintPanel;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAddCompound;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLoadCompound;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCompounds;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemWarning;
   }
}
