using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class MolWeightHalogensView : BaseUserControl, IMolWeightHalogensView
   {
      public MolWeightHalogensView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IMolWeightHalogensPresenter presenter)
      {
      }

      public void FillWithParameterView(IView parameterView)
      {
         this.FillWith(parameterView);
      }
   }
}