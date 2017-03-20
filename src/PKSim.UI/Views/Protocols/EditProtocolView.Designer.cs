namespace PKSim.UI.Views.Protocols
{
   partial class EditProtocolView
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
         this.layoutProtocol = new OSPSuite.UI.Controls.UxLayoutControl();
         this.splitter = new DevExpress.XtraEditors.SplitContainerControl();
         this.radioGroupProtocolMode = new DevExpress.XtraEditors.RadioGroup();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemProtocolMode = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSplitter = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutProtocol)).BeginInit();
         this.layoutProtocol.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
         this.splitter.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupProtocolMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtocolMode)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplitter)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutProtocol
         // 
         this.layoutProtocol.AllowCustomization = false;
         this.layoutProtocol.Controls.Add(this.splitter);
         this.layoutProtocol.Controls.Add(this.radioGroupProtocolMode);
         this.layoutProtocol.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutProtocol.Location = new System.Drawing.Point(0, 0);
         this.layoutProtocol.Name = "layoutProtocol";
         this.layoutProtocol.Root = this.layoutControlGroup1;
         this.layoutProtocol.Size = new System.Drawing.Size(566, 516);
         this.layoutProtocol.TabIndex = 0;
         this.layoutProtocol.Text = "layoutControl1";
         // 
         // splitter
         // 
         this.splitter.Horizontal = false;
         this.splitter.Location = new System.Drawing.Point(10, 39);
         this.splitter.Name = "splitter";
         this.splitter.Panel1.Text = "Panel1";
         this.splitter.Panel2.Text = "Panel2";
         this.splitter.Size = new System.Drawing.Size(546, 467);
         this.splitter.SplitterPosition = 231;
         this.splitter.TabIndex = 7;
         this.splitter.Text = "splitter";
         // 
         // radioGroupProtocolMode
         // 
         this.radioGroupProtocolMode.Location = new System.Drawing.Point(12, 12);
         this.radioGroupProtocolMode.Name = "radioGroupProtocolMode";
         this.radioGroupProtocolMode.Size = new System.Drawing.Size(542, 25);
         this.radioGroupProtocolMode.StyleController = this.layoutProtocol;
         this.radioGroupProtocolMode.TabIndex = 6;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemProtocolMode,
            this.layoutItemSplitter});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(566, 516);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemProtocolMode
         // 
         this.layoutItemProtocolMode.Control = this.radioGroupProtocolMode;
         this.layoutItemProtocolMode.CustomizationFormText = "layoutItemProtocolMode";
         this.layoutItemProtocolMode.Location = new System.Drawing.Point(0, 0);
         this.layoutItemProtocolMode.MaxSize = new System.Drawing.Size(0, 29);
         this.layoutItemProtocolMode.MinSize = new System.Drawing.Size(54, 29);
         this.layoutItemProtocolMode.Name = "layoutItemProtocolMode";
         this.layoutItemProtocolMode.Size = new System.Drawing.Size(546, 29);
         this.layoutItemProtocolMode.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemProtocolMode.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemProtocolMode.TextVisible = false;
         // 
         // layoutItemSplitter
         // 
         this.layoutItemSplitter.Control = this.splitter;
         this.layoutItemSplitter.CustomizationFormText = "layoutItemSplitter";
         this.layoutItemSplitter.Location = new System.Drawing.Point(0, 29);
         this.layoutItemSplitter.Name = "layoutItemSplitter";
         this.layoutItemSplitter.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemSplitter.Size = new System.Drawing.Size(546, 467);
         this.layoutItemSplitter.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSplitter.TextVisible = false;
         // 
         // EditProtocolView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "EditProtocolView";
         this.ClientSize = new System.Drawing.Size(566, 516);
         this.Controls.Add(this.layoutProtocol);
         this.Name = "EditProtocolView";
         this.Text = "EditProtocolView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutProtocol)).EndInit();
         this.layoutProtocol.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
         this.splitter.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupProtocolMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtocolMode)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplitter)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutProtocol;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.RadioGroup radioGroupProtocolMode;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProtocolMode;
      private DevExpress.XtraEditors.SplitContainerControl splitter;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSplitter;
   }
}