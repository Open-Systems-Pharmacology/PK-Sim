using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Views.AdvancedParameters;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views.AdvancedParameters
{
   public partial class AdvancedParametersView : BaseUserControl, IAdvancedParametersView
   {
      private IAdvancedParametersPresenter _presenter;

      public AdvancedParametersView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IAdvancedParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         btnAdd.Click += (o, e) => OnEvent(_presenter.AddAdvancedParameter);
         btnRemove.Click += (o, e) => OnEvent(_presenter.RemoveAdvancedParameter);
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.UserDefinedVariability;

      public override string Caption => PKSimConstants.UI.UserDefinedVariability;

      public void AddConstantParameterGroupsView(IView view)
      {
         panelConstantParameters.FillWith(view);
      }

      public void AddAdvancedParameterGroupsView(IView view)
      {
         panelAdvancedParameters.FillWith(view);
      }

      public bool AddEnabled
      {
         set => btnAdd.Enabled = value;
         get => btnAdd.Enabled;
      }

      public bool RemoveEnabled
      {
         set => btnRemove.Enabled = value;
         get => btnRemove.Enabled;
      }

      public void AddAdvancedParameterView(IView advancedParameterView)
      {
         panelDistributions.FillWith(advancedParameterView);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutControl.DoInBatch(() =>
         {
            layoutItemRemoveButton.AsRemoveButton();
            layoutItemAddButton.AsAddButton();
         });
      }
   }
}