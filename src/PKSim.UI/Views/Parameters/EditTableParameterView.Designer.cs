namespace PKSim.UI.Views.Parameters
{
   partial class EditTableParameterView
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
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.layoutItemSplit = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplit)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(438, 12);
         this.btnCancel.Size = new System.Drawing.Size(90, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(328, 12);
         this.btnOk.Size = new System.Drawing.Size(106, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 618);
         this.layoutControlBase.Size = new System.Drawing.Size(540, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.splitContainer);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(540, 618);
         this.layoutControl1.TabIndex = 34;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSplit});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(540, 618);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // splitContainer
         // 
         this.splitContainer.Horizontal = false;
         this.splitContainer.Location = new System.Drawing.Point(12, 12);
         this.splitContainer.Name = "splitContainer";
         this.splitContainer.Panel1.Text = "Panel1";
         this.splitContainer.Panel2.Text = "Panel2";
         this.splitContainer.Size = new System.Drawing.Size(516, 594);
         this.splitContainer.SplitterPosition = 283;
         this.splitContainer.TabIndex = 4;
         this.splitContainer.Text = "splitContainerControl1";
         // 
         // layoutItemSplit
         // 
         this.layoutItemSplit.Control = this.splitContainer;
         this.layoutItemSplit.CustomizationFormText = "layoutItemSplit";
         this.layoutItemSplit.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSplit.Name = "layoutItemSplit";
         this.layoutItemSplit.Size = new System.Drawing.Size(520, 598);
         this.layoutItemSplit.Text = "layoutItemSplit";
         this.layoutItemSplit.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSplit.TextToControlDistance = 0;
         this.layoutItemSplit.TextVisible = false;
         // 
         // EditTableParameterView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "EditTableParameterView";
         this.ClientSize = new System.Drawing.Size(540, 664);
         this.Controls.Add(this.layoutControl1);
         this.Name = "EditTableParameterView";
         this.Text = "EditTableParameterView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplit)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSplit;

   }
}