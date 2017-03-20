using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Core
{
   /// <summary>
   ///    Close all building block presenter that might be open for a building block being removed
   /// </summary>
   public interface ICloseSubjectPresenterInvoker : ICloseSubjectPresenterInvokerBase, IListener<BuildingBlockRemovedEvent>
   {
   }

   public class CloseSubjectPresenterInvoker : CloseSubjectPresenterInvokerBase, ICloseSubjectPresenterInvoker
   {
      public CloseSubjectPresenterInvoker(IApplicationController applicationController) : base(applicationController)
      {
      }

      public void Handle(BuildingBlockRemovedEvent buildingBlockRemovedEvent)
      {
         Close(buildingBlockRemovedEvent.BuildingBlock);
      }
   }
}