using System.Windows.Forms;
using OSPSuite.UI.Views;

namespace PKSim.BatchTool.Views
{
   public partial class BatchSilentView : BaseView, IBatchSilentView
   {
      public BatchSilentView()
      {
         InitializeComponent();
         FormBorderStyle = FormBorderStyle.None;
      }
   }
}
