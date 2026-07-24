namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundProtocolView
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
         this.panelFormulation = new DevExpress.XtraEditors.PanelControl();
         this.panelEvent = new DevExpress.XtraEditors.PanelControl();
         this.lblEventDescription = new DevExpress.XtraEditors.LabelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemFormulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemEvent = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemEventDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemProtocol = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelFormulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelEvent)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEvent)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEventDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtocol)).BeginInit();
         this.SuspendLayout();
         //
         // layoutControl
         //
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelControl1);
         this.layoutControl.Controls.Add(this.panelFormulation);
         this.layoutControl.Controls.Add(this.panelEvent);
         this.layoutControl.Controls.Add(this.lblEventDescription);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(374, 229);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         //
         // panelFormulation
         //
         this.panelFormulation.Location = new System.Drawing.Point(2, 39);
         this.panelFormulation.Name = "panelFormulation";
         this.panelFormulation.Size = new System.Drawing.Size(370, 77);
         this.panelFormulation.TabIndex = 14;
         //
         // panelEvent
         //
         this.panelEvent.Location = new System.Drawing.Point(2, 120);
         this.panelEvent.Name = "panelEvent";
         this.panelEvent.Size = new System.Drawing.Size(370, 77);
         this.panelEvent.TabIndex = 16;
         //
         // lblEventDescription
         //
         this.lblEventDescription.Location = new System.Drawing.Point(2, 118);
         this.lblEventDescription.Name = "lblEventDescription";
         this.lblEventDescription.Size = new System.Drawing.Size(63, 13);
         this.lblEventDescription.StyleController = this.layoutControl;
         this.lblEventDescription.TabIndex = 17;
         this.lblEventDescription.Text = "lblEventDescription";
         //
         // layoutControlGroup
         //
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemFormulation,
            this.layoutItemEventDescription,
            this.layoutItemEvent,
            this.layoutItemProtocol});
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(374, 229);
         this.layoutControlGroup.Text = "layoutControlGroup1";
         this.layoutControlGroup.TextVisible = false;
         //
         // layoutItemFormulation
         //
         this.layoutItemFormulation.Control = this.panelFormulation;
         this.layoutItemFormulation.Location = new System.Drawing.Point(0, 37);
         this.layoutItemFormulation.Name = "layoutItemFormulation";
         this.layoutItemFormulation.Size = new System.Drawing.Size(374, 81);
         this.layoutItemFormulation.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemFormulation.TextVisible = false;
         //
         // layoutItemEventDescription
         //
         this.layoutItemEventDescription.Control = this.lblEventDescription;
         this.layoutItemEventDescription.Location = new System.Drawing.Point(0, 118);
         this.layoutItemEventDescription.Name = "layoutItemEventDescription";
         this.layoutItemEventDescription.Size = new System.Drawing.Size(374, 30);
         this.layoutItemEventDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemEventDescription.TextToControlDistance = 0;
         this.layoutItemEventDescription.TextVisible = false;
         //
         // layoutItemEvent
         //
         this.layoutItemEvent.Control = this.panelEvent;
         this.layoutItemEvent.Location = new System.Drawing.Point(0, 148);
         this.layoutItemEvent.Name = "layoutItemEvent";
         this.layoutItemEvent.Size = new System.Drawing.Size(374, 81);
         this.layoutItemEvent.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemEvent.TextVisible = false;
         //
         // panelControl1
         //
         this.panelControl1.Location = new System.Drawing.Point(105, 2);
         this.panelControl1.Name = "panelControl1";
         this.panelControl1.Size = new System.Drawing.Size(267, 33);
         this.panelControl1.TabIndex = 15;
         //
         // layoutItemProtocol
         //
         this.layoutItemProtocol.Control = this.panelControl1;
         this.layoutItemProtocol.Location = new System.Drawing.Point(0, 0);
         this.layoutItemProtocol.Name = "layoutItemProtocol";
         this.layoutItemProtocol.Size = new System.Drawing.Size(374, 37);
         this.layoutItemProtocol.TextSize = new System.Drawing.Size(91, 13);
         //
         // SimulationCompoundProtocolView
         //
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationCompoundProtocolView";
         this.Size = new System.Drawing.Size(374, 229);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelFormulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelEvent)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEvent)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEventDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtocol)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelFormulation;
      private DevExpress.XtraEditors.PanelControl panelEvent;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemFormulation;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemEvent;
      private DevExpress.XtraEditors.PanelControl panelControl1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProtocol;
      private DevExpress.XtraEditors.LabelControl lblEventDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemEventDescription;
   }
}
