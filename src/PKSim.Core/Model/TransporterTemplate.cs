using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class TransporterTemplate : WithSynonyms
   {
      /// <summary>
      ///    Gene associated with the template
      /// </summary>
      public string Gene { get; set; }

      /// <summary>
      ///    Species for which the template is defined
      /// </summary>
      public string Species { get; set; }

      /// <summary>
      ///    Transporter type => Direction of transport
      /// </summary>
      public TransportType TransportType { get; set; }

      public override string Name => Gene;
   }
}