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
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.listBoxContainer = new DevExpress.XtraEditors.ListBoxControl();
         this.cbOntogeny = new DevExpress.XtraEditors.ComboBoxEdit();
         this.layoutControl = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.listBoxContainer)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogeny.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(492, 12);
         this.btnCancel.Size = new System.Drawing.Size(102, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(369, 12);
         this.btnOk.Size = new System.Drawing.Size(119, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 485);
         this.layoutControlBase.Size = new System.Drawing.Size(606, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(174, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(606, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(357, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(123, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(480, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(106, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(178, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(179, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(178, 26);
         // 
         // layoutControl1
         // 
         this.layoutControl1.AllowCustomization = false;
         this.layoutControl1.Controls.Add(this.lblDescription);
         this.layoutControl1.Controls.Add(this.splitContainer);
         this.layoutControl1.Controls.Add(this.cbOntogeny);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControl;
         this.layoutControl1.Size = new System.Drawing.Size(606, 485);
         this.layoutControl1.TabIndex = 34;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 36);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl1;
         this.lblDescription.TabIndex = 6;
         this.lblDescription.Text = "lblDescription";
         // 
         // splitContainer
         // 
         this.splitContainer.Location = new System.Drawing.Point(12, 53);
         this.splitContainer.Name = "splitContainer";
         this.splitContainer.Panel1.Controls.Add(this.listBoxContainer);
         this.splitContainer.Panel1.Text = "Panel1";
         this.splitContainer.Panel2.Text = "Panel2";
         this.splitContainer.Size = new System.Drawing.Size(582, 420);
         this.splitContainer.SplitterPosition = 170;
         this.splitContainer.TabIndex = 5;
         this.splitContainer.Text = "splitContainer";
         // 
         // listBoxContainer
         // 
         this.listBoxContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listBoxContainer.Location = new System.Drawing.Point(0, 0);
         this.listBoxContainer.Name = "listBoxContainer";
         this.listBoxContainer.Size = new System.Drawing.Size(170, 420);
         this.listBoxContainer.TabIndex = 0;
         // 
         // cbOntogeny
         // 
         this.cbOntogeny.Location = new System.Drawing.Point(115, 12);
         this.cbOntogeny.Name = "cbOntogeny";
         this.cbOntogeny.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbOntogeny.Size = new System.Drawing.Size(479, 20);
         this.cbOntogeny.StyleController = this.layoutControl1;
         this.cbOntogeny.TabIndex = 4;
         // 
         // layoutControl
         // 
         this.layoutControl.CustomizationFormText = "layoutControlGroup1";
         this.layoutControl.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControl.GroupBordersVisible = false;
         this.layoutControl.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutItemOntogeny,
            this.layoutControlItem1});
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Size = new System.Drawing.Size(606, 485);
         this.layoutControl.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.splitContainer;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 41);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(586, 424);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutItemOntogeny
         // 
         this.layoutItemOntogeny.Control = this.cbOntogeny;
         this.layoutItemOntogeny.CustomizationFormText = "layoutItemOntogeny";
         this.layoutItemOntogeny.Location = new System.Drawing.Point(0, 0);
         this.layoutItemOntogeny.Name = "layoutItemOntogeny";
         this.layoutItemOntogeny.Size = new System.Drawing.Size(586, 24);
         this.layoutItemOntogeny.TextSize = new System.Drawing.Size(100, 13);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.lblDescription;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(586, 17);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // ShowOntogenyDataView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ShowOntogenyDataView";
         this.ClientSize = new System.Drawing.Size(606, 531);
         this.Controls.Add(this.layoutControl1);
         this.Name = "ShowOntogenyDataView";
         this.Text = "ShowOntogenyDataView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.listBoxContainer)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogeny.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControl;
      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private DevExpress.XtraEditors.ListBoxControl listBoxContainer;
      private DevExpress.XtraEditors.ComboBoxEdit cbOntogeny;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOntogeny;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}