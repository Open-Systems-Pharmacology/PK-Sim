using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public abstract class Ontogeny : ObjectBase
   {
      public virtual string SpeciesName { get; set; }
      public virtual string DisplayName { get; set; }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var ontogeny = source as Ontogeny;
         if (ontogeny == null) return;
         SpeciesName = ontogeny.SpeciesName;
         DisplayName = ontogeny.DisplayName;
      }
   }

   public class DatabaseOntogeny : Ontogeny
   {
   }

   public class NullOntogeny : DatabaseOntogeny
   {
      public NullOntogeny()
      {
         Name = CoreConstants.Parameter.UNDEFINED_ONTOGENY;
         DisplayName = Name;
      }

      public override bool Equals(object obj)
      {
         return (obj as NullOntogeny) != null;
      }

      public override string ToString()
      {
         return PKSimConstants.UI.None;
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }
}