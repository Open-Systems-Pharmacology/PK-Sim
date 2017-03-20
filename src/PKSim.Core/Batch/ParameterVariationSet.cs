using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Core.Batch
{
   public class ParameterVariationSet
   {
      public string Name { get; set; }
      public List<ParameterValue> ParameterValues { get; set; }

      public ParameterVariationSet()
      {
         ParameterValues = new List<ParameterValue>();
      }
   }
}