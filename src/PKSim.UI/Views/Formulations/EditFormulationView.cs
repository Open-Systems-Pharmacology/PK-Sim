using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Views.Formulations;

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

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Formulation;
      }
   }
}