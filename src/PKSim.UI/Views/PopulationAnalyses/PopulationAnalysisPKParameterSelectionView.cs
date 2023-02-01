using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisPKParameterSelectionView : BaseUserControl, IPopulationAnalysisPKParameterSelectionView
   {
      private IPopulationAnalysisPKParameterSelectionPresenter _presenter;

      public PopulationAnalysisPKParameterSelectionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IPopulationAnalysisPKParameterSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         btnAdd.Click += (o, e) => OnEvent(_presenter.AddPKParameters);
         btnRemove.Click += (o, e) => OnEvent(_presenter.RemovePKParameters);
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.PKAnalysis;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutControl.DoInBatch(() =>
         {
            layoutItemButtonRemove.AsRemoveButton();
            layoutItemButtonAdd.AsAddButton();
         });
         Caption = PKSimConstants.UI.PKParameters;
      }

      public void AddAllPKParametersView(IView view)
      {
         panelAvailablePKParameters.FillWith(view);
      }

      public void AddSelectedPKParametersView(IView view)
      {
         panelSelectedPKParameters.FillWith(view);
      }

      public void AddDistributionView(IView view)
      {
         panelDistributionView.FillWith(view);
      }
   }
}