using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class TransporterContainerTemplate : WithSynonyms, ITransporterContainer
   {
      /// <summary>
      ///    Internal process names that will be created in the simulation when this transporter is selected
      /// </summary>
      private readonly List<string> _allProcessNames = new List<string>();

      /// <summary>
      ///    Gene associated with the template
      /// </summary>
      public string Gene { get; set; }

      /// <summary>
      ///    Species for which the template is defined
      /// </summary>
      public string Species { get; set; }

      /// <summary>
      ///    Organ where the transporter is defined
      /// </summary>
      public string OrganName { get; set; }

      /// <summary>
      ///    Compartment of the organ named OrganName where the transporter is defined
      /// </summary>
      public string CompartmentName { get; set; }

      public IEnumerable<string> ProcessNames => _allProcessNames;

      public void AddProcessName(string processName)
      {
         _allProcessNames.Add(processName);
      }

      public void ClearProcessNames()
      {
         _allProcessNames.Clear();
      }

      /// <summary>
      ///    Transporter type => Direction of transport
      /// </summary>
      public TransportType TransportType { get; set; }

      /// <summary>
      ///    Transporter type => Direction of transport
      /// </summary>
      public TransportDirection TransportDirection { get; set; }
      
      public override string Name => Gene;

   }
}