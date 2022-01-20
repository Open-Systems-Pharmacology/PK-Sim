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
         this.progressBar = new DevExpress.XtraEditors.ProgressBarControl();
         this.lblProgress = new DevExpress.XtraEditors.LabelControl();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // progressBar
         // 
         this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.progressBar.Location = new System.Drawing.Point(0, 496);
         this.progressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.progressBar.Name = "progressBar";
         this.progressBar.Properties.LookAndFeel.SkinName = "Blue";
         this.progressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
         this.progressBar.Size = new System.Drawing.Size(626, 22);
         this.progressBar.TabIndex = 7;
         // 
         // lblProgress
         // 
         this.lblProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.lblProgress.Location = new System.Drawing.Point(0, 480);
         this.lblProgress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.lblProgress.Name = "lblProgress";
         this.lblProgress.Size = new System.Drawing.Size(63, 16);
         this.lblProgress.TabIndex = 8;
         this.lblProgress.Text = "lblProgress";
         // 
         // SplashScreen
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SplashScreen";
         this.ClientSize = new System.Drawing.Size(626, 518);
         this.Controls.Add(this.lblProgress);
         this.Controls.Add(this.progressBar);
         this.LookAndFeel.SkinName = "Blue";
         this.LookAndFeel.UseDefaultLookAndFeel = false;
         this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.Name = "SplashScreen";
         this.Text = "SplashScreen";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.progressBar.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ProgressBarControl progressBar;
        private DevExpress.XtraEditors.LabelControl lblProgress;




    }
}