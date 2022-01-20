namespace PKSim.UI.Views
{
    partial class SplashScreen
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
         this.progressBar = new DevExpress.XtraEditors.ProgressBarControl();
         this.lblProgress = new DevExpress.XtraEditors.LabelControl();
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         this.buttonHide = new DevExpress.XtraEditors.SimpleButton();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // progressBar
         // 
         this.tablePanel.SetColumn(this.progressBar, 0);
         this.tablePanel.SetColumnSpan(this.progressBar, 2);
         this.progressBar.Location = new System.Drawing.Point(0, 135);
         this.progressBar.Margin = new System.Windows.Forms.Padding(0);
         this.progressBar.Name = "progressBar";
         this.progressBar.Properties.LookAndFeel.SkinName = "Blue";
         this.progressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
         this.tablePanel.SetRow(this.progressBar, 2);
         this.progressBar.Size = new System.Drawing.Size(505, 20);
         this.progressBar.TabIndex = 7;
         // 
         // lblProgress
         // 
         this.tablePanel.SetColumn(this.lblProgress, 0);
         this.lblProgress.Location = new System.Drawing.Point(4, 115);
         this.lblProgress.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
         this.lblProgress.Name = "lblProgress";
         this.tablePanel.SetRow(this.lblProgress, 1);
         this.lblProgress.Size = new System.Drawing.Size(63, 16);
         this.lblProgress.TabIndex = 8;
         this.lblProgress.Text = "lblProgress";
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 53.73F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 1.27F)});
         this.tablePanel.Controls.Add(this.buttonHide);
         this.tablePanel.Controls.Add(this.pictureBox1);
         this.tablePanel.Controls.Add(this.progressBar);
         this.tablePanel.Controls.Add(this.lblProgress);
         this.tablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tablePanel.Location = new System.Drawing.Point(0, 0);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 110.8001F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 23.59935F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(505, 155);
         this.tablePanel.TabIndex = 9;
         // 
         // buttonHide
         // 
         this.tablePanel.SetColumn(this.buttonHide, 1);
         this.buttonHide.Location = new System.Drawing.Point(477, 114);
         this.buttonHide.Name = "buttonHide";
         this.tablePanel.SetRow(this.buttonHide, 1);
         this.buttonHide.Size = new System.Drawing.Size(25, 18);
         this.buttonHide.TabIndex = 10;
         this.buttonHide.Text = "x";
         // 
         // pictureBox1
         // 
         this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
         this.tablePanel.SetColumn(this.pictureBox1, 0);
         this.tablePanel.SetColumnSpan(this.pictureBox1, 2);
         this.pictureBox1.Location = new System.Drawing.Point(3, 3);
         this.pictureBox1.Name = "pictureBox1";
         this.tablePanel.SetRow(this.pictureBox1, 0);
         this.pictureBox1.Size = new System.Drawing.Size(499, 105);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
         this.pictureBox1.TabIndex = 9;
         this.pictureBox1.TabStop = false;
         // 
         // SplashScreen
         // 
         this.Appearance.BackColor = System.Drawing.Color.White;
         this.Appearance.Options.UseBackColor = true;
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SplashScreen";
         this.ClientSize = new System.Drawing.Size(505, 155);
         this.Controls.Add(this.tablePanel);
         this.LookAndFeel.SkinName = "Blue";
         this.LookAndFeel.UseDefaultLookAndFeel = false;
         this.Margin = new System.Windows.Forms.Padding(5);
         this.Name = "SplashScreen";
         this.Text = "SplashScreen";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         this.tablePanel.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ProgressBarControl progressBar;
        private DevExpress.XtraEditors.LabelControl lblProgress;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
      private System.Windows.Forms.PictureBox pictureBox1;
      private DevExpress.XtraEditors.SimpleButton buttonHide;
   }
}