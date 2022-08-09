namespace PKSim.UI.Views.Formulations
{
   partial class TableFormulationView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.panelParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelTable = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemTable = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelTable)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTable)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl.Controls.Add(this.panelTable);
         this.layoutControl.Controls.Add(this.panelParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(436, 456);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.layoutItemTable});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(436, 456);
         this.layoutControlGroup.TextVisible = false;
         // 
         // panelParameters
         // 
         this.panelParameters.Location = new System.Drawing.Point(2, 2);
         this.panelParameters.Name = "panelParameters";
         this.panelParameters.Size = new System.Drawing.Size(432, 98);
         this.panelParameters.TabIndex = 4;
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.panelParameters;
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Size = new System.Drawing.Size(436, 102);
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextVisible = false;
         // 
         // panelTable
         // 
         this.panelTable.Location = new System.Drawing.Point(2, 104);
         this.panelTable.Name = "panelTable";
         this.panelTable.Size = new System.Drawing.Size(432, 350);
         this.panelTable.TabIndex = 0;
         // 
         // layoutItemTable
         // 
         this.layoutItemTable.Control = this.panelTable;
         this.layoutItemTable.Location = new System.Drawing.Point(0, 102);
         this.layoutItemTable.Name = "layoutItemTable";
         this.layoutItemTable.Size = new System.Drawing.Size(436, 354);
         this.layoutItemTable.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemTable.TextVisible = false;
         // 
         // TableFormulationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "TableFormulationView";
         this.Size = new System.Drawing.Size(436, 456);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelTable)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTable)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.PanelControl panelTable;
      private DevExpress.XtraEditors.PanelControl panelParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTable;
   }
}
