
namespace PKSim.UI.Views.ExpressionProfiles
{
   partial class EditExpressionProfileView
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
         this.panelControl = new DevExpress.XtraEditors.PanelControl();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemPanelControl = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelControl)).BeginInit();
         this.SuspendLayout();
         // 
         // panelControl
         // 
         this.panelControl.Location = new System.Drawing.Point(12, 12);
         this.panelControl.Name = "panelControl";
         this.panelControl.Size = new System.Drawing.Size(679, 531);
         this.panelControl.TabIndex = 0;
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(703, 555);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPanelControl});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(703, 555);
         this.Root.TextVisible = false;
         // 
         // layoutItemPanelControl
         // 
         this.layoutItemPanelControl.Control = this.panelControl;
         this.layoutItemPanelControl.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPanelControl.Name = "layoutItemPanelControl";
         this.layoutItemPanelControl.Size = new System.Drawing.Size(683, 535);
         this.layoutItemPanelControl.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPanelControl.TextVisible = false;
         // 
         // EditExpressionProfileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "EditExpressionProfileView";
         this.ClientSize = new System.Drawing.Size(703, 555);
         this.Controls.Add(this.layoutControl);
         this.Name = "EditExpressionProfileView";
         this.Text = "EditExpressionProfileView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelControl)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.PanelControl panelControl;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanelControl;
   }
}