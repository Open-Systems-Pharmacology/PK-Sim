using System;
using System.Collections.Generic;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.DTO.Compounds
{
   /// <summary>
   ///    Case insensitive collection of the names that cannot be used for a compound
   /// </summary>
   public class ForbiddenCompoundNames
   {
      private readonly HashSet<string> _names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

      public void AddRange(IEnumerable<string> names) => names.Each(x => _names.Add(x));

      public bool Contains(string name) => !string.IsNullOrEmpty(name) && _names.Contains(name.Trim());
   }
}
