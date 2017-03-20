using System;
using System.Collections.Generic;
using System.Linq;

namespace PKSim.Core.Model
{
   public abstract class WithSynonyms
   {
      private readonly List<string> _allSynonyms = new List<string>();


      public void AddSynonym(string synonym)
      {
         _allSynonyms.Add(synonym);
      }


      public IReadOnlyList<string> Synonyms
      {
         get { return _allSynonyms; }
      }

      public bool IsTemplateFor(string name)
      {
         return areSame(Name, name) || _allSynonyms.Any(s => areSame(s, name));
      }

      public abstract string Name { get;  }

      private bool areSame(string name1, string name2)
      {
         return string.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
      }  
   }
}