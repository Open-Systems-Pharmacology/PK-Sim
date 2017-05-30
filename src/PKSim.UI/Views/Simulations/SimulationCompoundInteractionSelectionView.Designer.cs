
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundInteractionSelectionView
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
         this.panelWarning = new UxHintPanel();
         this.btnAddInteraction = new DevExpress.XtraEditors.SimpleButton();
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemInteractionSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemAddInteraction = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemWarning = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemInteractionSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddInteraction)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelWarning);
         this.layoutControl.Controls.Add(this.btnAddInteraction);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(571, 322);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelWarning
         // 
         this.panelWarning.Location = new System.Drawing.Point(2, 2);
         this.panelWarning.MaximumSize = new System.Drawing.Size(1000000, 50);
         this.panelWarning.MinimumSize = new System.Drawing.Size(200, 50);
         this.panelWarning.Name = "panelWarning";
         this.panelWarning.NoteText = "";
         this.panelWarning.Size = new System.Drawing.Size(567, 50);
         this.panelWarning.TabIndex = 6;
         // 
         // btnAddInteraction
         // 
         this.btnAddInteraction.Location = new System.Drawing.Point(287, 56);
         this.btnAddInteraction.Name = "btnAddInteraction";
         this.btnAddInteraction.Size = new System.Drawing.Size(282, 22);
         this.btnAddInteraction.StyleController = this.layoutControl;
         this.btnAddInteraction.TabIndex = 5;
         this.btnAddInteraction.Text = "btnAddInhibition";
         // 
         // gridControl
         // 
         this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
         this.gridControl.Location = new System.Drawing.Point(144, 82);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(425, 238);
         this.gridControl.TabIndex = 4;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.AllowsFiltering = true;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.MultiSelect = false;
         this.gridView.Name = "gridView";
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemInteractionSelection,
            this.layoutItemAddInteraction,
            this.emptySpaceItem,
            this.layoutItemWarning});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(571, 322);
         this.layoutControlGroup.Text = "layoutControlGroup1";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemInteractionSelection
         // 
         this.layoutItemInteractionSelection.Control = this.gridControl;
         this.layoutItemInteractionSelection.CustomizationFormText = "layoutControlItem1";
         this.layoutItemInteractionSelection.Location = new System.Drawing.Point(0, 80);
         this.layoutItemInteractionSelection.Name = "layoutItemInteractionSelection";
         this.layoutItemInteractionSelection.Size = new System.Drawing.Size(571, 242);
         this.layoutItemInteractionSelection.Text = "layoutItemInhibitionSelection";
         this.layoutItemInteractionSelection.TextSize = new System.Drawing.Size(139, 13);
         // 
         // layoutItemAddInteraction
         // 
         this.layoutItemAddInteraction.Control = this.btnAddInteraction;
         this.layoutItemAddInteraction.CustomizationFormText = "layoutItemAddInhibition";
         this.layoutItemAddInteraction.Location = new System.Drawing.Point(285, 54);
         this.layoutItemAddInteraction.Name = "layoutItemAddInteraction";
         this.layoutItemAddInteraction.Size = new System.Drawing.Size(286, 26);
         this.layoutItemAddInteraction.Text = "layoutItemAddInhibition";
         this.layoutItemAddInteraction.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAddInteraction.TextToControlDistance = 0;
         this.layoutItemAddInteraction.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 54);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(285, 26);
         this.emptySpaceItem.Text = "emptySpaceItem1";
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemWarning
         // 
         this.layoutItemWarning.Control = this.panelWarning;
         this.layoutItemWarning.CustomizationFormText = "layoutItemWarning";
         this.layoutItemWarning.Location = new System.Drawing.Point(0, 0);
         this.layoutItemWarning.Name = "layoutItemWarning";
         this.layoutItemWarning.Size = new System.Drawing.Size(571, 54);
         this.layoutItemWarning.Text = "layoutItemWarning";
         this.layoutItemWarning.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemWarning.TextToControlDistance = 0;
         this.layoutItemWarning.TextVisible = false;
         // 
         // SimulationCompoundInteractionSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationCompoundInteractionSelectionView";
         this.Size = new System.Drawing.Size(571, 322);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemInteractionSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddInteraction)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraGrid.GridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemInteractionSelection;
      private DevExpress.XtraEditors.SimpleButton btnAddInteraction;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAddInteraction;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private UxHintPanel panelWarning;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemWarning;

   }
}
