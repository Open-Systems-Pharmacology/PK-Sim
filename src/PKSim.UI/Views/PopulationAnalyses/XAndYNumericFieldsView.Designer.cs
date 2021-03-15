namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class XAndYNumericFieldsView
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
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbYField = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbXField = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemXField = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemYField = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbYField.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbXField.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemXField)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemYField)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.lblDescription);
         this.layoutControl1.Controls.Add(this.cbYField);
         this.layoutControl1.Controls.Add(this.cbXField);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(405, 142);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(2, 2);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl1;
         this.lblDescription.TabIndex = 6;
         this.lblDescription.Text = "lblDescription";
         // 
         // cbYField
         // 
         this.cbYField.Location = new System.Drawing.Point(85, 43);
         this.cbYField.Name = "cbYField";
         this.cbYField.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbYField.Size = new System.Drawing.Size(318, 20);
         this.cbYField.StyleController = this.layoutControl1;
         this.cbYField.TabIndex = 5;
         // 
         // cbXField
         // 
         this.cbXField.Location = new System.Drawing.Point(85, 19);
         this.cbXField.Name = "cbXField";
         this.cbXField.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbXField.Size = new System.Drawing.Size(318, 20);
         this.cbXField.StyleController = this.layoutControl1;
         this.cbXField.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemXField,
            this.layoutItemYField,
            this.layoutItemDescription,
            this.emptySpaceItem2});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(405, 142);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemXField
         // 
         this.layoutItemXField.Control = this.cbXField;
         this.layoutItemXField.CustomizationFormText = "layoutItemXField";
         this.layoutItemXField.Location = new System.Drawing.Point(0, 17);
         this.layoutItemXField.Name = "layoutItemXField";
         this.layoutItemXField.Size = new System.Drawing.Size(405, 24);
         this.layoutItemXField.Text = "layoutItemXField";
         this.layoutItemXField.TextSize = new System.Drawing.Size(80, 13);
         // 
         // layoutItemYField
         // 
         this.layoutItemYField.Control = this.cbYField;
         this.layoutItemYField.CustomizationFormText = "layoutItemYField";
         this.layoutItemYField.Location = new System.Drawing.Point(0, 41);
         this.layoutItemYField.Name = "layoutItemYField";
         this.layoutItemYField.Size = new System.Drawing.Size(405, 24);
         this.layoutItemYField.Text = "layoutItemYField";
         this.layoutItemYField.TextSize = new System.Drawing.Size(80, 13);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutControlItem1";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(405, 17);
         this.layoutItemDescription.Text = "layoutControlItem1";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextToControlDistance = 0;
         this.layoutItemDescription.TextVisible = false;
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 65);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(405, 77);
         this.emptySpaceItem2.Text = "emptySpaceItem2";
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // XAndYNumericFieldsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl1);
         this.Name = "XAndYNumericFieldsView";
         this.Size = new System.Drawing.Size(405, 142);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbYField.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbXField.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemXField)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemYField)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbYField;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbXField;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemXField;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemYField;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
   }
}
