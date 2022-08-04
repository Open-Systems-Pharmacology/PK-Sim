using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Views.Formulations;

namespace PKSim.UI.Views.Formulations
{
   public partial class TableFormulationView : BaseContainerUserControl, ITableFormulationView
   {
      private ITableFormulationPresenter _presenter;

      public TableFormulationView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ITableFormulationPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddTableView(IView view)
      {
         panelTable.FillWith(view);
      }

      public void AddParametersView(IView view)
      {
         AddViewTo(layoutItemParameters, layoutControl, view);
      }
   }
}