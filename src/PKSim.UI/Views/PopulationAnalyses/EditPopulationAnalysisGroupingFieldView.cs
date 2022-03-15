using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class EditPopulationAnalysisGroupingFieldView : BaseModalView, IEditPopulationAnalysisGroupingFieldView
   {
      public EditPopulationAnalysisGroupingFieldView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditPopulationAnalysisGroupingFieldPresenter presenter)
      {
      }

      public void SetGroupingView(IView view)
      {
         panel.FillWith(view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.Edit;
      }
   }
}