using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class SpeciesPopulation : ObjectBase
   {
      private readonly IList<Gender> _allGenders = new List<Gender>();

      public virtual string Species { get; set; }
      public virtual bool IsAgeDependent { get; set; }
      public virtual bool IsHeightDependent { get; set; }
      public virtual bool IsBodySurfaceAreaDependent { get; set; }
      public virtual string DisplayName { get; set; }
      public virtual int RaceIndex { get; set; }
      public virtual int Sequence { get; set; }

      public virtual bool IsPreterm => string.Equals(Name, CoreConstants.Population.Preterm);

      public virtual void AddGender(Gender gender)
      {
         _allGenders.Add(gender);
      }

      public virtual Gender GenderByName(string name)
      {
         return _allGenders.FindByName(name);
      }

      public virtual IEnumerable<Gender> Genders => _allGenders;
   }
}