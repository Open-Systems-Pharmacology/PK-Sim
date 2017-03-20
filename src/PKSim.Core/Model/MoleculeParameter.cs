using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class MoleculeParameter : WithSynonyms
   {
      public string MoleculeName { get; set; }
      public IDistributedParameter Parameter { get; set; }

      public override string Name
      {
         get { return MoleculeName; }
      }
   }
}