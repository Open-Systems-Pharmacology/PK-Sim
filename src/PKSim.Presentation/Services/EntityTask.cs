using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using IEditDescriptionPresenter = PKSim.Presentation.Presenters.IEditDescriptionPresenter;

namespace PKSim.Presentation.Services
{
   public class EntityTask : IEntityTask
   {
      private readonly IApplicationController _applicationController;
      private readonly IExecutionContext _executionContext;

      public EntityTask(IApplicationController applicationController, IExecutionContext executionContext)
      {
         _applicationController = applicationController;
         _executionContext = executionContext;
      }

      public ICommand Rename(IEntity elementToRename)
      {
         return rename(elementToRename, false);
      }

      private ICommand rename(IEntity elementToRename, bool isStructuralChange)
      {
         string newName;
         using (var renamePresenter = _applicationController.Start<IRenameObjectPresenter>())
         {
            if (!renamePresenter.Edit(elementToRename))
               return new PKSimEmptyCommand();

            newName = renamePresenter.Name;
         }
         return new RenameEntityCommand(elementToRename, newName, _executionContext) {ShouldChangeVersion = isStructuralChange}.Run(_executionContext);
      }

      public ICommand StructuralRename(IEntity elementToRename)
      {
         return rename(elementToRename, true);
      }

      public string NewNameFor(IWithName withName, IEnumerable<string> forbiddenNames = null, string entityType = null)
      {
         using (var renamePresenter = _applicationController.Start<IRenameObjectPresenter>())
         {
            return renamePresenter.NewNameFrom(withName, forbiddenNames, entityType);
         }
      }

      public string TypeFor(IObjectBase objectBase)
      {
         return _executionContext.TypeFor(objectBase);
      }

      public ICommand EditDescription(IObjectBase objectBase, string title = null)
      {
         using (var editDescriptionPresenter = _applicationController.Start<IEditDescriptionPresenter>())
         {
            editDescriptionPresenter.Title = title;
            if (!editDescriptionPresenter.Edit(objectBase))
               return new PKSimEmptyCommand();

            return UpdateDescription(objectBase, editDescriptionPresenter.Description);
         }
      }

      public ICommand UpdateDescription(IObjectBase objectBase, string newDescription)
      {
         return new EditObjectBaseDescriptionCommand(objectBase, newDescription, _executionContext).Run(_executionContext);
      }
   }
}