using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   partial class EditOutputSchemaView
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
         _gridViewBinder.Dispose();         
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.gridOutputInterval = new UxGridControl();
         this.mainView = new UxGridView();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridOutputInterval)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainView)).BeginInit();
         this.SuspendLayout();
         // 
         // gridOutputInterval
         // 
         this.gridOutputInterval.Dock = System.Windows.Forms.DockStyle.Fill;
         this.gridOutputInterval.Location = new System.Drawing.Point(0, 0);
         this.gridOutputInterval.MainView = this.mainView;
         this.gridOutputInterval.Name = "gridOutputInterval";
         this.gridOutputInterval.Size = new System.Drawing.Size(326, 291);
         this.gridOutputInterval.TabIndex = 0;
         this.gridOutputInterval.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.mainView});
         // 
         // mainView
         // 
         this.mainView.GridControl = this.gridOutputInterval;
         this.mainView.Name = "mainView";
         this.mainView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.mainView.RowsInsertable = false;
         // 
         // SimulationOutputSchemaView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.gridOutputInterval);
         this.Name = "EditOutputSchemaView";
         this.Size = new System.Drawing.Size(326, 291);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridOutputInterval)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainView)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private UxGridControl gridOutputInterval;
      private UxGridView mainView;

   }
}
