using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model
{
   public class UserDefinedOntogeny : Ontogeny
   {
      public DistributedTableFormula Table { get; set; }

      public UserDefinedOntogeny()
      {
         SpeciesName = CoreConstants.Species.Human;
      }

      public override string DisplayName
      {
         get => Name;
         set => Name = value;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var userDefinedOntogeny = source as UserDefinedOntogeny;
         if (userDefinedOntogeny == null) return;
         Table = cloneManager.Clone(userDefinedOntogeny.Table);
      }

      public virtual float[] PostmenstrualAges()
      {
         return Table.AllPoints().Select(p => p.X).ToFloatArray();
      }

      public virtual float[] OntogenyFactors()
      {
         return Table.AllPoints().Select(p => p.Y).ToFloatArray();
      }

      public virtual float[] Deviations()
      {
         return Table.AllDistributionMetaData().Select(p => p.Deviation).ToFloatArray();
      }
   }
}