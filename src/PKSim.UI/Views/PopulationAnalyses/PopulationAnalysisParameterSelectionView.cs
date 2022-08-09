using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Controls;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisParameterSelectionView : BaseUserControl, IPopulationAnalysisParameterSelectionView
   {
      private IPopulationAnalysisParameterSelectionPresenter _presenter;

      public PopulationAnalysisParameterSelectionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IPopulationAnalysisParameterSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddPopulationParametersView(IView view)
      {
         panelParameters.FillWith(view);
      }

      public void AddSelectedParametersView(IView view)
      {
         panelSelectedParameters.FillWith(view);
      }

      public void AddDistributionView(IView view)
      {
         splitContainerControl.Panel2.FillWith(view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutControl.DoInBatch(() =>
         {
            layoutItemButtonRemove.AsRemoveButton();
            layoutItemButtonAdd.AsAddButton();
         });
         Caption = PKSimConstants.UI.PopulationParameters;
         ApplicationIcon = ApplicationIcons.Population;
      }

      public override void InitializeBinding()
      {
         btnAdd.Click += (o, e) => OnEvent(_presenter.AddParameter);
         btnRemove.Click += (o, e) => OnEvent(_presenter.RemoveParameter);
      }
   }
}