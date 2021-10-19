namespace PKSim.UI.Starter.Views
{
   partial class StarterMainView
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
         this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
         this.btnRelativeExpression = new DevExpress.XtraEditors.SimpleButton();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.btnRelativeExpression);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.Root;
         this.layoutControl1.Size = new System.Drawing.Size(526, 428);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // btnRelativeExpression
         // 
         this.btnRelativeExpression.Location = new System.Drawing.Point(12, 12);
         this.btnRelativeExpression.Name = "btnRelativeExpression";
         this.btnRelativeExpression.Size = new System.Drawing.Size(502, 22);
         this.btnRelativeExpression.StyleController = this.layoutControl1;
         this.btnRelativeExpression.TabIndex = 4;
         this.btnRelativeExpression.Text = "btnRelativeExpression";
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(526, 428);
         this.Root.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.btnRelativeExpression;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(506, 26);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 26);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(506, 382);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // StarterMainView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "StarterMainView";
         this.ClientSize = new System.Drawing.Size(526, 428);
         this.Controls.Add(this.layoutControl1);
         this.Name = "StarterMainView";
         this.Text = "StarterMainView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraEditors.SimpleButton btnRelativeExpression;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}