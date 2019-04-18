using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Protocols
{
   public class SchemaItemTargetDTO: IWithName
   {
      public string Name { get; set; }
      public string Target { get; set; }
   }
}