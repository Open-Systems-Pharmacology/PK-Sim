using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatModelPassiveTransportMoleculeName
   {
      public string Model { get; set; }
      public string Transport { get; set; }
      public string Molecule { get; set; }
      public bool ShouldTransport { get; set; }
   }
}
