using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.UI.Views.Parameters
{
   public partial class EditTableParameterView : BaseModalView, IEditTableParameterView
   {
      public EditTableParameterView()
      {
      }

      public EditTableParameterView(BaseShell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditTableParameterPresenter presenter)
      {
      }

      public void AddView(IView baseView) => splitContainer.Panel1.FillWith(baseView);

      public void AddChart(IView baseView) => splitContainer.Panel2.FillWith(baseView);
   }
}