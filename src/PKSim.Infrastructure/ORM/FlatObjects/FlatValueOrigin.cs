using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatValueOrigin
   {
      public int Id { get; set; }
      public string Description { get; set; }
      public ValueOriginSourceId Source { get; set; }
      public ValueOriginDeterminationMethodId Method { get; set; }
   }
}