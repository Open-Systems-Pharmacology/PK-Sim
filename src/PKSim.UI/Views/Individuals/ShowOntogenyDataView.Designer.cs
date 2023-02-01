namespace PKSim.UI.Views.Individuals
{
   partial class ShowOntogenyDataView
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.uxLayoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.cbOntogeny = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutItemOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.listBoxContainer = new DevExpress.XtraEditors.ListBoxControl();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.uxLayoutControl1)).BeginInit();
         this.uxLayoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogeny.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).BeginInit();
         this.splitContainer.Panel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.listBoxContainer)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         this.SuspendLayout();
         // 
         // tablePanel
         // 
         this.tablePanel.Location = new System.Drawing.Point(0, 488);
         this.tablePanel.Size = new System.Drawing.Size(606, 43);
         // 
         // uxLayoutControl1
         // 
         this.uxLayoutControl1.AllowCustomization = false;
         this.uxLayoutControl1.Controls.Add(this.splitContainer);
         this.uxLayoutControl1.Controls.Add(this.cbOntogeny);
         this.uxLayoutControl1.Controls.Add(this.lblDescription);
         this.uxLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.uxLayoutControl1.Location = new System.Drawing.Point(0, 0);
         this.uxLayoutControl1.Name = "uxLayoutControl1";
         this.uxLayoutControl1.Root = this.layoutControlGroup1;
         this.uxLayoutControl1.Size = new System.Drawing.Size(606, 488);
         this.uxLayoutControl1.TabIndex = 39;
         this.uxLayoutControl1.Text = "uxLayoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemOntogeny,
            this.layoutItemDescription,
            this.layoutControlItem2});
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(606, 488);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // cbOntogeny
         // 
         this.cbOntogeny.Location = new System.Drawing.Point(124, 12);
         this.cbOntogeny.Name = "cbOntogeny";
         this.cbOntogeny.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbOntogeny.Size = new System.Drawing.Size(470, 20);
         this.cbOntogeny.StyleController = this.uxLayoutControl1;
         this.cbOntogeny.TabIndex = 5;
         // 
         // layoutItemOntogeny
         // 
         this.layoutItemOntogeny.Control = this.cbOntogeny;
         this.layoutItemOntogeny.Location = new System.Drawing.Point(0, 0);
         this.layoutItemOntogeny.Name = "layoutItemOntogeny";
         this.layoutItemOntogeny.Size = new System.Drawing.Size(586, 24);
         this.layoutItemOntogeny.TextSize = new System.Drawing.Size(100, 13);
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 36);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.uxLayoutControl1;
         this.lblDescription.TabIndex = 6;
         this.lblDescription.Text = "lblDescription";
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
         this.layoutItemDescription.CustomizationFormText = "layoutControlItem1";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 24);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(586, 17);
         this.layoutItemDescription.Text = "layoutItemDescription";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // splitContainer
         // 
         this.splitContainer.Location = new System.Drawing.Point(12, 53);
         this.splitContainer.Name = "splitContainer";
         // 
         // splitContainer.Panel1
         // 
         this.splitContainer.Panel1.Controls.Add(this.listBoxContainer);
         this.splitContainer.Panel1.Text = "Panel1";
         // 
         // splitContainer.Panel2
         // 
         this.splitContainer.Panel2.Text = "Panel2";
         this.splitContainer.Size = new System.Drawing.Size(582, 423);
         this.splitContainer.SplitterPosition = 170;
         this.splitContainer.TabIndex = 7;
         this.splitContainer.Text = "splitContainer";
         // 
         // listBoxContainer
         // 
         this.listBoxContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listBoxContainer.Location = new System.Drawing.Point(0, 0);
         this.listBoxContainer.Name = "listBoxContainer";
         this.listBoxContainer.Size = new System.Drawing.Size(170, 423);
         this.listBoxContainer.TabIndex = 0;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.splitContainer;
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 41);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(586, 427);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // ShowOntogenyDataView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ShowOntogenyDataView";
         this.ClientSize = new System.Drawing.Size(606, 531);
         this.Controls.Add(this.uxLayoutControl1);
         this.Name = "ShowOntogenyDataView";
         this.Text = "ShowOntogenyDataView";
         this.Controls.SetChildIndex(this.tablePanel, 0);
         this.Controls.SetChildIndex(this.uxLayoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.uxLayoutControl1)).EndInit();
         this.uxLayoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogeny.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel1)).EndInit();
         this.splitContainer.Panel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer.Panel2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.listBoxContainer)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private OSPSuite.UI.Controls.UxLayoutControl uxLayoutControl1;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbOntogeny;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOntogeny;
      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private DevExpress.XtraEditors.ListBoxControl listBoxContainer;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
   }
}