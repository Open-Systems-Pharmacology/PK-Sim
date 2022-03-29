namespace PKSim.UI.Views.Individuals
{
   partial class OntogenySelectionView
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
         _presenter = null;
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.cbOntogeny = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         this.btnLoadOntogenyFromFile = new DevExpress.XtraEditors.SimpleButton();
         this.btnShowOntogeny = new DevExpress.XtraEditors.SimpleButton();
         this.layoutItemOntogeny = new DevExpress.XtraEditors.LabelControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogeny.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         this.SuspendLayout();
         // 
         // cbOntogey
         // 
         this.tablePanel.SetColumn(this.cbOntogeny, 1);
         this.cbOntogeny.Location = new System.Drawing.Point(124, 4);
         this.cbOntogeny.Margin = new System.Windows.Forms.Padding(4);
         this.cbOntogeny.Name = "cbOntogeny";
         
         this.cbOntogeny.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbOntogeny, 0);
         this.cbOntogeny.Size = new System.Drawing.Size(262, 26);
         this.cbOntogeny.TabIndex = 0;
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 31.28F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 28.72F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F)});
         this.tablePanel.Controls.Add(this.btnLoadOntogenyFromFile);
         this.tablePanel.Controls.Add(this.btnShowOntogeny);
         this.tablePanel.Controls.Add(this.layoutItemOntogeny);
         this.tablePanel.Controls.Add(this.cbOntogeny);
         this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tablePanel.Location = new System.Drawing.Point(0, 0);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(647, 61);
         this.tablePanel.TabIndex = 2;
         // 
         // btnLoadOntogenyFromFile
         // 
         this.tablePanel.SetColumn(this.btnLoadOntogenyFromFile, 3);
         this.btnLoadOntogenyFromFile.Location = new System.Drawing.Point(550, 3);
         this.btnLoadOntogenyFromFile.Name = "btnLoadOntogenyFromFile";
         this.tablePanel.SetRow(this.btnLoadOntogenyFromFile, 0);
         this.btnLoadOntogenyFromFile.Size = new System.Drawing.Size(94, 29);
         this.btnLoadOntogenyFromFile.TabIndex = 8;
         this.btnLoadOntogenyFromFile.Text = "btnLoadOntogenyFromFile";
         // 
         // btnShowOntogeny
         // 
         this.tablePanel.SetColumn(this.btnShowOntogeny, 2);
         this.btnShowOntogeny.Location = new System.Drawing.Point(393, 3);
         this.btnShowOntogeny.Name = "btnShowOntogeny";
         this.tablePanel.SetRow(this.btnShowOntogeny, 0);
         this.btnShowOntogeny.Size = new System.Drawing.Size(151, 29);
         this.btnShowOntogeny.TabIndex = 7;
         this.btnShowOntogeny.Text = "btnShowOntogeny";
         // 
         // layoutItemOntogeny
         // 
         this.tablePanel.SetColumn(this.layoutItemOntogeny, 0);
         this.layoutItemOntogeny.Location = new System.Drawing.Point(3, 9);
         this.layoutItemOntogeny.Name = "layoutItemOntogeny";
         this.tablePanel.SetRow(this.layoutItemOntogeny, 0);
         this.layoutItemOntogeny.Size = new System.Drawing.Size(114, 16);
         this.layoutItemOntogeny.TabIndex = 6;
         this.layoutItemOntogeny.Text = "layoutItemOntogeny";
         // 
         // OntogenySelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
         this.Controls.Add(this.tablePanel);
         this.Margin = new System.Windows.Forms.Padding(5);
         this.Name = "OntogenySelectionView";
         this.Size = new System.Drawing.Size(647, 61);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogeny.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         this.tablePanel.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxComboBoxEdit cbOntogeny;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
      private DevExpress.XtraEditors.LabelControl layoutItemOntogeny;
      private DevExpress.XtraEditors.SimpleButton btnLoadOntogenyFromFile;
      private DevExpress.XtraEditors.SimpleButton btnShowOntogeny;
   }
}
