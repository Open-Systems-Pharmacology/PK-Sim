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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.btnScale = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.tbValue = new DevExpress.XtraEditors.TextEdit();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.btnReset = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbValue.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl.AutoScroll = false;
         this.layoutControl.Controls.Add(this.btnReset);
         this.layoutControl.Controls.Add(this.tbValue);
         this.layoutControl.Controls.Add(this.btnScale);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(412, 22);
         this.layoutControl.TabIndex = 5;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(412, 22);
         this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // btnScale
         // 
         this.btnScale.Location = new System.Drawing.Point(0, 0);
         this.btnScale.Name = "btnScale";
         this.btnScale.Size = new System.Drawing.Size(88, 22);
         this.btnScale.StyleController = this.layoutControl;
         this.btnScale.TabIndex = 4;
         this.btnScale.Text = "btnScale";
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.btnScale;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlItem1.Size = new System.Drawing.Size(88, 22);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // tbValue
         // 
         this.tbValue.Location = new System.Drawing.Point(88, 0);
         this.tbValue.Name = "tbValue";
         this.tbValue.Properties.AutoHeight = false;
         this.tbValue.Size = new System.Drawing.Size(232, 22);
         this.tbValue.StyleController = this.layoutControl;
         this.tbValue.TabIndex = 5;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.tbValue;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(88, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlItem2.Size = new System.Drawing.Size(232, 22);
         this.layoutControlItem2.Text = "layoutControlItem2";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextToControlDistance = 0;
         this.layoutControlItem2.TextVisible = false;
         // 
         // btnReset
         // 
         this.btnReset.Location = new System.Drawing.Point(320, 0);
         this.btnReset.Name = "btnReset";
         this.btnReset.Size = new System.Drawing.Size(92, 22);
         this.btnReset.StyleController = this.layoutControl;
         this.btnReset.TabIndex = 6;
         this.btnReset.Text = "btnReset";
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.btnReset;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(320, 0);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlItem3.Size = new System.Drawing.Size(92, 22);
         this.layoutControlItem3.Text = "layoutControlItem3";
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextToControlDistance = 0;
         this.layoutControlItem3.TextVisible = false;
         // 
         // ScaleParametersView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "ScaleParametersView";
         this.Size = new System.Drawing.Size(412, 22);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbValue.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SimpleButton btnReset;
      private DevExpress.XtraEditors.TextEdit tbValue;
      private DevExpress.XtraEditors.SimpleButton btnScale;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;

   }
}
