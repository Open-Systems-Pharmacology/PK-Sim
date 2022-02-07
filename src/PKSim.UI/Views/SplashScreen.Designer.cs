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
         this.progressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
         this.labelCopyright = new DevExpress.XtraEditors.LabelControl();
         this.labelStatus = new DevExpress.XtraEditors.LabelControl();
         this.peImage = new DevExpress.XtraEditors.PictureEdit();
         this.peLogo = new DevExpress.XtraEditors.PictureEdit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.peLogo.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // progressBarControl
         // 
         this.progressBarControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.progressBarControl.Location = new System.Drawing.Point(27, 205);
         this.progressBarControl.Margin = new System.Windows.Forms.Padding(4);
         this.progressBarControl.Name = "progressBarControl";
         this.progressBarControl.Size = new System.Drawing.Size(744, 20);
         this.progressBarControl.TabIndex = 5;
         // 
         // labelCopyright
         // 
         this.labelCopyright.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.labelCopyright.Location = new System.Drawing.Point(28, 260);
         this.labelCopyright.Margin = new System.Windows.Forms.Padding(4);
         this.labelCopyright.Name = "labelCopyright";
         this.labelCopyright.Size = new System.Drawing.Size(54, 16);
         this.labelCopyright.TabIndex = 6;
         this.labelCopyright.Text = "Copyright";
         // 
         // labelStatus
         // 
         this.labelStatus.Location = new System.Drawing.Point(27, 184);
         this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 1);
         this.labelStatus.Name = "labelStatus";
         this.labelStatus.Size = new System.Drawing.Size(57, 16);
         this.labelStatus.TabIndex = 7;
         this.labelStatus.Text = "Starting...";
         // 
         // peImage
         // 
         this.peImage.Dock = System.Windows.Forms.DockStyle.Top;
         this.peImage.EditValue = ((object)(resources.GetObject("peImage.EditValue")));
         this.peImage.Location = new System.Drawing.Point(1, 1);
         this.peImage.Margin = new System.Windows.Forms.Padding(4);
         this.peImage.Name = "peImage";
         this.peImage.Properties.AllowFocused = false;
         this.peImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.peImage.Properties.Appearance.Options.UseBackColor = true;
         this.peImage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.peImage.Properties.ShowMenu = false;
         this.peImage.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
         this.peImage.Properties.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
         this.peImage.Size = new System.Drawing.Size(798, 175);
         this.peImage.TabIndex = 9;
         // 
         // peLogo
         // 
         this.peLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.peLogo.EditValue = ((object)(resources.GetObject("peLogo.EditValue")));
         this.peLogo.Location = new System.Drawing.Point(429, 237);
         this.peLogo.Margin = new System.Windows.Forms.Padding(4);
         this.peLogo.Name = "peLogo";
         this.peLogo.Properties.AllowFocused = false;
         this.peLogo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.peLogo.Properties.Appearance.Options.UseBackColor = true;
         this.peLogo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.peLogo.Properties.ShowMenu = false;
         this.peLogo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
         this.peLogo.Size = new System.Drawing.Size(351, 52);
         this.peLogo.TabIndex = 8;
         // 
         // SplashScreen
         // 
         this.Appearance.BackColor = System.Drawing.Color.White;
         this.Appearance.Options.UseBackColor = true;
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SplashScreenNew";
         this.ClientSize = new System.Drawing.Size(800, 301);
         this.Controls.Add(this.peImage);
         this.Controls.Add(this.peLogo);
         this.Controls.Add(this.labelStatus);
         this.Controls.Add(this.labelCopyright);
         this.Controls.Add(this.progressBarControl);
         this.Name = "SplashScreen";
         this.Padding = new System.Windows.Forms.Padding(1);
         this.Text = "SplashScreenNew";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.progressBarControl.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.peLogo.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private DevExpress.XtraEditors.ProgressBarControl progressBarControl;
      private DevExpress.XtraEditors.LabelControl labelCopyright;
      private DevExpress.XtraEditors.LabelControl labelStatus;
      private DevExpress.XtraEditors.PictureEdit peLogo;
      private DevExpress.XtraEditors.PictureEdit peImage;
   }
}
