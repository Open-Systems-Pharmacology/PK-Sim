namespace PKSim.UI.Views
{
   partial class BuildingBlockSelectionView
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
         cleanup();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.btnLoadBuildingBlock = new DevExpress.XtraEditors.SimpleButton();
         this.btnCreateBuildingBlock = new DevExpress.XtraEditors.SimpleButton();
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         this.cbBuildingBlocks = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbBuildingBlocks.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // btnLoadBuildingBlock
         // 
         this.tablePanel.SetColumn(this.btnLoadBuildingBlock, 2);
         this.btnLoadBuildingBlock.Location = new System.Drawing.Point(378, 130);
         this.btnLoadBuildingBlock.Margin = new System.Windows.Forms.Padding(4);
         this.btnLoadBuildingBlock.Name = "btnLoadBuildingBlock";
         this.tablePanel.SetRow(this.btnLoadBuildingBlock, 0);
         this.btnLoadBuildingBlock.Size = new System.Drawing.Size(303, 27);
         this.btnLoadBuildingBlock.TabIndex = 7;
         this.btnLoadBuildingBlock.Text = "btnLoadBuildingBlock";
         // 
         // btnCreateBuildingBlock
         // 
         this.tablePanel.SetColumn(this.btnCreateBuildingBlock, 1);
         this.btnCreateBuildingBlock.Location = new System.Drawing.Point(237, 129);
         this.btnCreateBuildingBlock.Margin = new System.Windows.Forms.Padding(4);
         this.btnCreateBuildingBlock.Name = "btnCreateBuildingBlock";
         this.tablePanel.SetRow(this.btnCreateBuildingBlock, 0);
         this.btnCreateBuildingBlock.Size = new System.Drawing.Size(133, 28);
         this.btnCreateBuildingBlock.TabIndex = 5;
         this.btnCreateBuildingBlock.Text = "btnCreateBuildingBlock";
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 37.32F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 22.68F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F)});
         this.tablePanel.Controls.Add(this.cbBuildingBlocks);
         this.tablePanel.Controls.Add(this.btnCreateBuildingBlock);
         this.tablePanel.Controls.Add(this.btnLoadBuildingBlock);
         this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tablePanel.Location = new System.Drawing.Point(0, 0);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(685, 287);
         this.tablePanel.TabIndex = 1;
         // 
         // cbBuildingBlocks
         // 
         this.tablePanel.SetColumn(this.cbBuildingBlocks, 0);
         this.cbBuildingBlocks.Location = new System.Drawing.Point(4, 132);
         this.cbBuildingBlocks.Margin = new System.Windows.Forms.Padding(4);
         this.cbBuildingBlocks.Name = "cbBuildingBlocks";
         this.cbBuildingBlocks.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbBuildingBlocks, 0);
         this.cbBuildingBlocks.Size = new System.Drawing.Size(225, 22);
         this.cbBuildingBlocks.TabIndex = 6;
         // 
         // BuildingBlockSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.tablePanel);
         this.Margin = new System.Windows.Forms.Padding(5);
         this.Name = "BuildingBlockSelectionView";
         this.Size = new System.Drawing.Size(685, 287);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbBuildingBlocks.Properties)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion
      private DevExpress.XtraEditors.SimpleButton btnCreateBuildingBlock;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbBuildingBlocks;
      private DevExpress.XtraEditors.SimpleButton btnLoadBuildingBlock;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
   }
}


