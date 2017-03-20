namespace PKSim.UI.Views.Parameters
{
   partial class ParametersByGroupView
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
         this.gridParameters = new DevExpress.XtraGrid.GridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // gridParameters
         // 
         this.gridParameters.Dock = System.Windows.Forms.DockStyle.Fill;
         this.gridParameters.Location = new System.Drawing.Point(0, 0);
         this.gridParameters.MainView = this.gridViewParameters;
         this.gridParameters.Name = "gridParameters";
         this.gridParameters.Size = new System.Drawing.Size(508, 536);
         this.gridParameters.TabIndex = 5;
         this.gridParameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameters});
         // 
         // gridViewParameters
         // 
         this.gridViewParameters.GridControl = this.gridParameters;
         this.gridViewParameters.Name = "gridViewParameters";
         this.gridViewParameters.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // CompoundProcessParametersView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.gridParameters);
         this.Name = "ParametersByGroupView";
         this.Size = new System.Drawing.Size(508, 536);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraGrid.GridControl gridParameters;
      private PKSim.UI.Views.Core.UxGridView gridViewParameters;

   }
}
