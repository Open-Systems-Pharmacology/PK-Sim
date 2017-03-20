using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Populations
{
   partial class PopulationParameterGroupsView
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
         this.filterTreeView = new FilterTreeView();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         this.SuspendLayout();
         // 
         // filterTreeView
         // 
         this.filterTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.filterTreeView.Location = new System.Drawing.Point(0, 0);
         this.filterTreeView.Name = "filterTreeView";
         this.filterTreeView.ShowDescendantNode = true;
         this.filterTreeView.Size = new System.Drawing.Size(346, 603);
         this.filterTreeView.TabIndex = 0;
         // 
         // AdvancedParameterGroupsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.filterTreeView);
         this.Name = "PopulationParameterGroupsView";
         this.Size = new System.Drawing.Size(346, 603);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private FilterTreeView filterTreeView;

   }
}
