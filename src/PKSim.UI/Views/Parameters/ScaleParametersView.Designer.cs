namespace PKSim.UI.Views.Parameters
{
   partial class ScaleParametersView
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
         this.btnReset = new DevExpress.XtraEditors.SimpleButton();
         this.tbValue = new DevExpress.XtraEditors.TextEdit();
         this.btnScale = new DevExpress.XtraEditors.SimpleButton();
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbValue.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnReset
         // 
         this.tablePanel.SetColumn(this.btnReset, 2);
         this.btnReset.Location = new System.Drawing.Point(334, 3);
         this.btnReset.Name = "btnReset";
         this.tablePanel.SetRow(this.btnReset, 0);
         this.btnReset.Size = new System.Drawing.Size(120, 20);
         this.btnReset.TabIndex = 6;
         this.btnReset.Text = "btnReset";
         // 
         // tbValue
         // 
         this.tablePanel.SetColumn(this.tbValue, 1);
         this.tbValue.Location = new System.Drawing.Point(107, 3);
         this.tbValue.Name = "tbValue";
         this.tbValue.Properties.AutoHeight = false;
         this.tablePanel.SetRow(this.tbValue, 0);
         this.tbValue.Size = new System.Drawing.Size(221, 20);
         this.tbValue.TabIndex = 5;
         // 
         // btnScale
         // 
         this.tablePanel.SetColumn(this.btnScale, 0);
         this.btnScale.Location = new System.Drawing.Point(3, 3);
         this.btnScale.Name = "btnScale";
         this.tablePanel.SetRow(this.btnScale, 0);
         this.btnScale.Size = new System.Drawing.Size(98, 20);
         this.btnScale.TabIndex = 4;
         this.btnScale.Text = "btnScale";
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 24.95F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 54.62F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 30.43F)});
         this.tablePanel.Controls.Add(this.btnScale);
         this.tablePanel.Controls.Add(this.tbValue);
         this.tablePanel.Controls.Add(this.btnReset);
         this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tablePanel.Location = new System.Drawing.Point(0, 0);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(457, 28);
         this.tablePanel.TabIndex = 6;
         // 
         // ScaleParametersView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.tablePanel);
         this.Name = "ScaleParametersView";
         this.Size = new System.Drawing.Size(457, 28);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbValue.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion
      private DevExpress.XtraEditors.SimpleButton btnReset;
      private DevExpress.XtraEditors.TextEdit tbValue;
      private DevExpress.XtraEditors.SimpleButton btnScale;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
   }
}
