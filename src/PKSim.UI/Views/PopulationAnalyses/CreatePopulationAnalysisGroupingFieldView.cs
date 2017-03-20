using PKSim.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class CreatePopulationAnalysisGroupingFieldView : BaseModalView, ICreatePopulationAnalysisGroupingFieldView
   {
      private ICreatePopulationAnalysisGroupingFieldPresenter _presenter;
      private readonly ScreenBinder<GroupingFieldDTO> _screenBinder;

      public CreatePopulationAnalysisGroupingFieldView(Shell shell) : base(shell)
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<GroupingFieldDTO>();
      }

      public void AttachPresenter(ICreatePopulationAnalysisGroupingFieldPresenter presenter)
      {
         _presenter = presenter;
      }

      public void SetGroupingView(IView view)
      {
         panelGroupingView.FillWith(view);
      }

      public void BindTo(GroupingFieldDTO groupingFieldDTO)
      {
         _screenBinder.BindToSource(groupingFieldDTO);
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.Name).To(tbName);
         _screenBinder.Bind(x => x.GroupingDefinitionItem)
            .To(cbGroupingDefinition)
            .WithValues(x => _presenter.AvailableGroupings)
            .Changed += () => OnEvent(_presenter.SelectedGroupingChanged);

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemName.Text = PKSimConstants.UI.Name.FormatForLabel();
         layoutItemGroupingDefinition.Text = PKSimConstants.UI.Groupings.FormatForLabel();
      }

      protected override void SetActiveControl()
      {
         ActiveControl = tbName;
      }
   }
}