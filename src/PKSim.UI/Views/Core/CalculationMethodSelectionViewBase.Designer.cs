namespace PKSim.UI.Views.Core
{
   partial class CalculationMethodSelectionViewBase<TPresenter>
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
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AutoScroll = false;
         this.layoutControl.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.layoutControl.Size = new System.Drawing.Size(506, 431);
         // 
         // layoutItemGrid
         // 
         this.layoutItemGrid.MaxSize = new System.Drawing.Size(0, 86);
         this.layoutItemGrid.MinSize = new System.Drawing.Size(1, 86);
         this.layoutItemGrid.Size = new System.Drawing.Size(506, 431);
         this.layoutItemGrid.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         // 
         // CalculationMethodSelectionViewBase
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.Name = "CalculationMethodSelectionViewBase";
         this.Size = new System.Drawing.Size(506, 431);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

   }
}
