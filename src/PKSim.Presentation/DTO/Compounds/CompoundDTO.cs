using System.Collections.Generic;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   /// <summary>
   ///    DTO used when creating a new compound. On top of the standard <see cref="ObjectBaseDTO" /> rules, it ensures
   ///    that the compound name is not the name of a model element
   /// </summary>
   public class CompoundDTO : ObjectBaseDTO, IWithForbiddenCompoundNames
   {
      private readonly ForbiddenCompoundNames _forbiddenNames = new ForbiddenCompoundNames();

      public CompoundDTO()
      {
         Rules.Add(ForbiddenCompoundNamesRules.NameIsNotForbidden<CompoundDTO>());
      }

      public void AddForbiddenNames(IEnumerable<string> forbiddenNames) => _forbiddenNames.AddRange(forbiddenNames);

      public bool IsForbidden(string name) => _forbiddenNames.Contains(name);
   }
}
