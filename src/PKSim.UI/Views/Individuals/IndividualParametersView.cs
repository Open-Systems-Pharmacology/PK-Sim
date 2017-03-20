using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualParametersView : BaseUserControl, IIndividualParametersView
   {
      private IIndividualParametersPresenter _presenter;

      public IndividualParametersView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IIndividualParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddParametersView(IView parameterView)
      {
         this.FillWith(parameterView);
      }

      public override string Caption => PKSimConstants.UI.AnatomyAndPhysiology;

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Parameters;

      protected override int TopicId => HelpId.PKSim_Individuals_SetorChangeIndividualProperties;
   }
}