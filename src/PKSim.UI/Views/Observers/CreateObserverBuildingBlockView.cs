using System;
using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.UI.Views.Observers
{
   public partial class CreateObserverBuildingBlockView : BaseModalView, ICreateObserverBuildingBlockView
   {
      public void AttachPresenter(ICreateObserverBuildingBlockPresenter presenter)
      {
         
      }

      public void BindToProperties(ObjectBaseDTO observerBuildingBlockDTO)
      {
         throw new NotImplementedException();
      }
   }
}
