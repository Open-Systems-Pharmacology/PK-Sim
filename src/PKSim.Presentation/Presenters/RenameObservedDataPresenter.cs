using System.Linq;
using PKSim.Assets;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.Core;

namespace PKSim.Presentation.Presenters
{
   public interface IRenameObservedDataPresenter : IObjectBasePresenter<DataRepository>
   {
   }

   public class RenameObservedDataPresenter : ObjectBasePresenter<DataRepository>, IRenameObservedDataPresenter
   {
      private readonly ICoreWorkspace _workspace;

      public RenameObservedDataPresenter(IObjectBaseView view, ICoreWorkspace workspace) : base(view, false)
      {
         _workspace = workspace;
      }

      protected override void InitializeResourcesFor(DataRepository dataRepository)
      {
         _view.Caption = PKSimConstants.UI.Rename;
         _view.NameDescription = PKSimConstants.UI.RenameEntityCaption(PKSimConstants.ObjectTypes.ObservedData, dataRepository.Name);
      }

      protected override ObjectBaseDTO CreateDTOFor(DataRepository objectToRename)
      {
         var dto = new ObjectBaseDTO {ContainerType = PKSimConstants.ObjectTypes.Project, Name = objectToRename.Name};
         dto.AddUsedNames(_workspace.Project.AllObservedData.Select(x => x.Name));
         return dto;
      }
   }
}