
namespace PKSim.UI.Views.ExpressionProfiles
{
   partial class ExpressionProfileMoleculesView
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
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.btnLoadFromDatabase = new OSPSuite.UI.Controls.UxSimpleButton();
         this.tbCategory = new DevExpress.XtraEditors.TextEdit();
         this.cbMoleculeName = new OSPSuite.UI.Controls.UxMRUEdit();
         this.cbSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         this.panelExpression = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemMoleculeName = new DevExpress.XtraEditors.LabelControl();
         this.layoutItemCategory = new DevExpress.XtraEditors.LabelControl();
         this.layoutItemSpecies = new DevExpress.XtraEditors.LabelControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).BeginInit();
         this.SuspendLayout();
         // 
         // btnLoadFromDatabase
         // 
         this.tablePanel.SetColumn(this.btnLoadFromDatabase, 2);
         this.btnLoadFromDatabase.Location = new System.Drawing.Point(503, 34);
         this.btnLoadFromDatabase.Manager = null;
         this.btnLoadFromDatabase.Margin = new System.Windows.Forms.Padding(4);
         this.btnLoadFromDatabase.Name = "btnLoadFromDatabase";
         this.tablePanel.SetRow(this.btnLoadFromDatabase, 1);
         this.btnLoadFromDatabase.Shortcut = System.Windows.Forms.Keys.None;
         this.btnLoadFromDatabase.Size = new System.Drawing.Size(476, 22);
         this.btnLoadFromDatabase.TabIndex = 8;
         this.btnLoadFromDatabase.Text = "btnLoadFromDatabased";
         // 
         // tbCategory
         // 
         this.tablePanel.SetColumn(this.tbCategory, 1);
         this.tablePanel.SetColumnSpan(this.tbCategory, 2);
         this.tbCategory.Location = new System.Drawing.Point(153, 64);
         this.tbCategory.Margin = new System.Windows.Forms.Padding(4);
         this.tbCategory.Name = "tbCategory";
         this.tablePanel.SetRow(this.tbCategory, 2);
         this.tbCategory.Size = new System.Drawing.Size(826, 22);
         this.tbCategory.TabIndex = 6;
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.tablePanel.SetColumn(this.cbMoleculeName, 1);
         this.cbMoleculeName.Location = new System.Drawing.Point(153, 34);
         this.cbMoleculeName.Margin = new System.Windows.Forms.Padding(4);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbMoleculeName, 1);
         this.cbMoleculeName.Size = new System.Drawing.Size(342, 22);
         this.cbMoleculeName.TabIndex = 5;
         // 
         // cbSpecies
         // 
         this.tablePanel.SetColumn(this.cbSpecies, 1);
         this.tablePanel.SetColumnSpan(this.cbSpecies, 2);
         this.cbSpecies.Location = new System.Drawing.Point(153, 4);
         this.cbSpecies.Margin = new System.Windows.Forms.Padding(4);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.AllowMouseWheel = false;
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbSpecies, 0);
         this.cbSpecies.Size = new System.Drawing.Size(826, 22);
         this.cbSpecies.TabIndex = 4;
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 162.28F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 36.12F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 50F)});
         this.tablePanel.Controls.Add(this.panelExpression);
         this.tablePanel.Controls.Add(this.layoutItemMoleculeName);
         this.tablePanel.Controls.Add(this.layoutItemCategory);
         this.tablePanel.Controls.Add(this.layoutItemSpecies);
         this.tablePanel.Controls.Add(this.cbSpecies);
         this.tablePanel.Controls.Add(this.tbCategory);
         this.tablePanel.Controls.Add(this.btnLoadFromDatabase);
         this.tablePanel.Controls.Add(this.cbMoleculeName);
         this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tablePanel.Location = new System.Drawing.Point(0, 0);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(983, 848);
         this.tablePanel.TabIndex = 1;
         // 
         // panelExpression
         // 
         this.tablePanel.SetColumn(this.panelExpression, 0);
         this.tablePanel.SetColumnSpan(this.panelExpression, 3);
         this.panelExpression.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panelExpression.Location = new System.Drawing.Point(0, 90);
         this.panelExpression.Margin = new System.Windows.Forms.Padding(0);
         this.panelExpression.Name = "panelExpression";
         this.tablePanel.SetRow(this.panelExpression, 3);
         this.panelExpression.Size = new System.Drawing.Size(983, 758);
         this.panelExpression.TabIndex = 12;
         // 
         // layoutItemMoleculeName
         // 
         this.tablePanel.SetColumn(this.layoutItemMoleculeName, 0);
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(3, 37);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.tablePanel.SetRow(this.layoutItemMoleculeName, 1);
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(143, 16);
         this.layoutItemMoleculeName.TabIndex = 11;
         this.layoutItemMoleculeName.Text = "layoutItemMoleculeName";
         // 
         // layoutItemCategory
         // 
         this.tablePanel.SetColumn(this.layoutItemCategory, 0);
         this.layoutItemCategory.Location = new System.Drawing.Point(3, 67);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.tablePanel.SetRow(this.layoutItemCategory, 2);
         this.layoutItemCategory.Size = new System.Drawing.Size(111, 16);
         this.layoutItemCategory.TabIndex = 10;
         this.layoutItemCategory.Text = "layoutItemCategory";
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Location = new System.Drawing.Point(3, 7);
         this.layoutItemSpecies.Name = "layoutItemSpecies";
         this.layoutItemSpecies.Size = new System.Drawing.Size(104, 16);
         this.layoutItemSpecies.TabIndex = 9;
         this.layoutItemSpecies.Text = "layoutItemSpecies";
         // 
         // ExpressionProfileMoleculesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.tablePanel);
         this.Margin = new System.Windows.Forms.Padding(5);
         this.Name = "ExpressionProfileMoleculesView";
         this.Size = new System.Drawing.Size(983, 848);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         this.tablePanel.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion
      private Core.UxImageComboBoxEdit cbSpecies;
      private DevExpress.XtraEditors.TextEdit tbCategory;
      private OSPSuite.UI.Controls.UxMRUEdit cbMoleculeName;
      private OSPSuite.UI.Controls.UxSimpleButton btnLoadFromDatabase;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
      private DevExpress.XtraEditors.LabelControl layoutItemMoleculeName;
      private DevExpress.XtraEditors.LabelControl layoutItemCategory;
      private DevExpress.XtraEditors.LabelControl layoutItemSpecies;
      private DevExpress.XtraEditors.PanelControl panelExpression;
   }
}
