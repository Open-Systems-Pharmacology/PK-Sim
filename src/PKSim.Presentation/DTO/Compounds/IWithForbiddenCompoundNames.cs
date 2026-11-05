using System.Collections.Generic;

namespace PKSim.Presentation.DTO.Compounds
{
   /// <summary>
   ///    Implemented by DTOs used to name a compound. The forbidden names are the names of model elements that a compound
   ///    cannot use (see <see cref="PKSim.Core.Services.IForbiddenCompoundNamesRetriever" />)
   /// </summary>
   public interface IWithForbiddenCompoundNames
   {
      void AddForbiddenNames(IEnumerable<string> forbiddenNames);
      bool IsForbidden(string name);
   }
}
