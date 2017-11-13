using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Batch
{
   internal class Formulation : IWithName
   {
      public string Name { get; set; }
      public string FormulationType { get; set; }
      public Dictionary<string, double> Parameters { get; set; }

      public Formulation()
      {
         Parameters = new Dictionary<string, double>();
      }
   }
}