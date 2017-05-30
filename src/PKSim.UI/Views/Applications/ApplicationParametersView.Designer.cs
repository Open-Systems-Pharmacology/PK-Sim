using OSPSuite.Utility.Extensions;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Applications
{
   partial class ApplicationParametersView
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
         _gridApplicationsBinder.Dispose();
         _comboBoxUnit.Dispose();
         _cache.DisposeAll();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
         this.gridApplications = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameters =  new UxGridView();;
         this.mainView =  new UxGridView();;
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridApplications)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainView)).BeginInit();
         this.SuspendLayout();
         // 
         // gridApplications
         // 
         this.gridApplications.Dock = System.Windows.Forms.DockStyle.Fill;
         gridLevelNode1.LevelTemplate = this.gridViewParameters;
         gridLevelNode1.RelationName = "Parameters";
         this.gridApplications.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
         this.gridApplications.Location = new System.Drawing.Point(0, 0);
         this.gridApplications.MainView = this.mainView;
         this.gridApplications.Name = "gridApplications";
         this.gridApplications.Size = new System.Drawing.Size(720, 532);
         this.gridApplications.TabIndex = 2;
         this.gridApplications.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameters,
            this.mainView});
         // 
         // gridViewParameters
         // 
         this.gridViewParameters.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DodgerBlue;
         this.gridViewParameters.Appearance.HeaderPanel.Options.UseBackColor = true;
         this.gridViewParameters.GridControl = this.gridApplications;
         this.gridViewParameters.Name = "gridViewParameters";
         // 
         // mainView
         // 
         this.mainView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.PaleGreen;
         this.mainView.Appearance.HeaderPanel.Options.UseBackColor = true;
         this.mainView.GridControl = this.gridApplications;
         this.mainView.Name = "mainView";
         // 
         // ApplicationParametersView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.gridApplications);
         this.Name = "ApplicationParametersView";
         this.Size = new System.Drawing.Size(720, 532);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridApplications)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainView)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxGridControl gridApplications;
      private UxGridView gridViewParameters;
      private UxGridView mainView;
   }
}
