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
         this.labelVersion = new DevExpress.XtraEditors.LabelControl();
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
         this.progressBarControl.Location = new System.Drawing.Point(23, 167);
         this.progressBarControl.Name = "progressBarControl";
         this.progressBarControl.Size = new System.Drawing.Size(606, 16);
         this.progressBarControl.TabIndex = 5;
         // 
         // labelCopyright
         // 
         this.labelCopyright.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.labelCopyright.Location = new System.Drawing.Point(26, 218);
         this.labelCopyright.Name = "labelCopyright";
         this.labelCopyright.Size = new System.Drawing.Size(69, 13);
         this.labelCopyright.TabIndex = 6;
         this.labelCopyright.Text = "labelCopyright";
         // 
         // labelStatus
         // 
         this.labelStatus.Location = new System.Drawing.Point(23, 150);
         this.labelStatus.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
         this.labelStatus.Name = "labelStatus";
         this.labelStatus.Size = new System.Drawing.Size(50, 13);
         this.labelStatus.TabIndex = 7;
         this.labelStatus.Text = "Starting...";
         // 
         // peImage
         // 
         this.peImage.Anchor = System.Windows.Forms.AnchorStyles.None;
         this.peImage.EditValue = ((object)(resources.GetObject("peImage.EditValue")));
         this.peImage.Location = new System.Drawing.Point(23, 2);
         this.peImage.Margin = new System.Windows.Forms.Padding(0);
         this.peImage.Name = "peImage";
         this.peImage.Properties.AllowFocused = false;
         this.peImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.peImage.Properties.Appearance.Options.UseBackColor = true;
         this.peImage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.peImage.Properties.ShowMenu = false;
         this.peImage.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
         this.peImage.Properties.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
         this.peImage.Size = new System.Drawing.Size(606, 142);
         this.peImage.TabIndex = 9;
         // 
         // peLogo
         // 
         this.peLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.peLogo.EditValue = ((object)(resources.GetObject("peLogo.EditValue")));
         this.peLogo.Location = new System.Drawing.Point(368, 193);
         this.peLogo.Name = "peLogo";
         this.peLogo.Properties.AllowFocused = false;
         this.peLogo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.peLogo.Properties.Appearance.Options.UseBackColor = true;
         this.peLogo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.peLogo.Properties.ShowMenu = false;
         this.peLogo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
         this.peLogo.Size = new System.Drawing.Size(269, 42);
         this.peLogo.TabIndex = 8;
         // 
         // labelVersion
         // 
         this.labelVersion.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.labelVersion.Location = new System.Drawing.Point(26, 196);
         this.labelVersion.Name = "labelVersion";
         this.labelVersion.Size = new System.Drawing.Size(57, 13);
         this.labelVersion.TabIndex = 10;
         this.labelVersion.Text = "labelVersion";
         // 
         // SplashScreen
         // 
         this.Appearance.BackColor = System.Drawing.Color.White;
         this.Appearance.Options.UseBackColor = true;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SplashScreenNew";
         this.ClientSize = new System.Drawing.Size(654, 245);
         this.Controls.Add(this.labelVersion);
         this.Controls.Add(this.peImage);
         this.Controls.Add(this.peLogo);
         this.Controls.Add(this.labelStatus);
         this.Controls.Add(this.labelCopyright);
         this.Controls.Add(this.progressBarControl);
         this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
      private DevExpress.XtraEditors.LabelControl labelVersion;
   }
}
