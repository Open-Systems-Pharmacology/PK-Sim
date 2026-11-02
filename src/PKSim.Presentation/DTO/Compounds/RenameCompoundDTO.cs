using System.Collections.Generic;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   /// <summary>
   ///    DTO used when renaming or cloning a compound. On top of the standard <see cref="RenameObjectDTO" /> rules, it
   ///    ensures that the compound name is not the name of a model element
   /// </summary>
   public class RenameCompoundDTO : RenameObjectDTO, IWithForbiddenCompoundNames
   {
      private readonly ForbiddenCompoundNames _forbiddenNames = new ForbiddenCompoundNames();

      public RenameCompoundDTO(string name) : base(name)
      {
         Rules.Add(ForbiddenCompoundNamesRules.NameIsNotForbidden<RenameCompoundDTO>());
      }

      public void AddForbiddenNames(IEnumerable<string> forbiddenNames) => _forbiddenNames.AddRange(forbiddenNames);

      public bool IsForbidden(string name) => _forbiddenNames.Contains(name);
   }
}
