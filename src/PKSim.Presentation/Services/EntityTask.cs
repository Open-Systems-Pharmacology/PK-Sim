using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using IEditDescriptionPresenter = PKSim.Presentation.Presenters.IEditDescriptionPresenter;

namespace PKSim.Presentation.Services
{
   public class EntityTask : IEntityTask
   {
      private readonly IApplicationController _applicationController;
      private readonly IExecutionContext _executionContext;
      private readonly IRenameObjectDTOFactory _renameObjectDTOFactory;

      public EntityTask(
         IApplicationController applicationController, 
         IExecutionContext executionContext, 
         IRenameObjectDTOFactory renameObjectDTOFactory)
      {
         _applicationController = applicationController;
         _executionContext = executionContext;
         _renameObjectDTOFactory = renameObjectDTOFactory;
      }

      public ICommand Rename(IEntity elementToRename)
      {
         return rename(elementToRename, isStructuralChange:false);
      }

      private ICommand rename(IEntity elementToRename, bool isStructuralChange)
      {
         var dto = _renameObjectDTOFactory.CreateFor(elementToRename);
         var newName = NewNameFor(elementToRename, dto.UsedNames, TypeFor(elementToRename));
         if(string.IsNullOrEmpty(newName))
            return new PKSimEmptyCommand();

         return new RenameEntityCommand(elementToRename, newName, _executionContext)
         {
            ShouldChangeVersion = isStructuralChange
         }.Run(_executionContext);
      }

      public ICommand StructuralRename(IEntity elementToRename) => rename(elementToRename, isStructuralChange: true);

      public string NewNameFor(IWithName withName, IEnumerable<string> forbiddenNames = null, string entityType = null)
      {
         using (var renamePresenter = getRenameObjectPresenterFor(withName))
         {
            return renamePresenter.NewNameFrom(withName, forbiddenNames, entityType);
         }
      }

      private IRenamePresenter getRenameObjectPresenterFor(IWithName entityToRename)
      {
         switch (entityToRename)
         {
            case ExpressionProfile _:
               return _applicationController.Start<IRenameExpressionProfilePresenter>();
            default:
               return _applicationController.Start<IRenameObjectPresenter>();
         }
      }

      public string TypeFor(IObjectBase objectBase) => _executionContext.TypeFor(objectBase);

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