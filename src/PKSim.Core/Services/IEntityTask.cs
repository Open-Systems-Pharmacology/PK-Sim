using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IEntityTask
   {
      /// <summary>
      ///    Renames the element. Does not trigger a strucutral change of the containing building block
      /// </summary>
      ICommand Rename(IEntity elementToRename);

      /// <summary>
      ///    Renames the element and triggers a strucutral change of the containing building block
      /// </summary>
      ICommand StructuralRename(IEntity elementToRename);

      string NewNameFor(IWithName withName, IEnumerable<string> forbiddenNames = null, string entityType = null);
      string TypeFor(IObjectBase entity);
      ICommand EditDescription(IObjectBase objectBase, string title = null);
      ICommand UpdateDescription(IObjectBase objectBase, string newDescription);
   }
}