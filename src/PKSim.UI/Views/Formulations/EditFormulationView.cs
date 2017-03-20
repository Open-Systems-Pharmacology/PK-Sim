using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Views.Formulations;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Formulations
{
   public partial class EditFormulationView : BaseMdiChildView, IEditFormulationView
   {
      public EditFormulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditFormulationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         this.FillWith(viewToAdd);
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.Formulation; }
      }
   }
}