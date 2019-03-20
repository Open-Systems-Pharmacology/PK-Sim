using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSPSuite.UI.Controls;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.UI.Views.Observers
{
   public partial class ImportObserversView : BaseUserControl, IImportObserversView
   {
      private IImportObserversPresenter _presenter;

      public ImportObserversView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IImportObserversPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}
